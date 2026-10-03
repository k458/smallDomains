using Essentials;

namespace ActionPointTurn;

public class ActionPointTurnService
{
    public void StartTurn(ActionPointTurnState state, IEnumerable<ITurnActor> actors)
    {
        state.ActorStates.Clear();
        state.AttackPhaseCommitments.Clear();
        state.MovementPhaseCommitments.Clear();
        state.MovementContests.Clear();
        state.CurrentStep = 3;
        state.AttackPhaseResolved = false;
        state.MovementPhaseResolved = false;
        state.TurnFinished = false;

        foreach (ITurnActor actor in actors)
        {
            TurnActorStepState actorState = new(actor)
            {
                RemainingAP = Math.Clamp(actor.Speed, 1, 3),
                MovedThisTurn = false,
                TilesMovedThisTurn = 0,
                MovedPreviousStep = false,
                AttacksThisTurn = 0,
                Recoil = 0f,
                Momentum = 0f,
                LastAction = TurnActionType.Undefined,
                WasHitThisTurn = false,
                MovingDodgeActive = false,
                Commitment = null
            };

            state.ActorStates.Add(actor, actorState);
        }
    }

    public IReadOnlyCollection<ITurnActor> GetEligibleActors(ActionPointTurnState state)
    {
        List<ITurnActor> eligibleActors = [];

        foreach (TurnActorStepState actorState in state.ActorStates.Values)
        {
            if (IsEligibleForCurrentStep(state, actorState))
            {
                eligibleActors.Add(actorState.Actor);
            }
        }

        return eligibleActors;
    }

    public bool TryCommitAction(ActionPointTurnState state, TurnActionCommitment commitment)
    {
        if (!state.ActorStates.TryGetValue(commitment.Actor, out TurnActorStepState? actorState))
        {
            return false;
        }

        if (!IsEligibleForCurrentStep(state, actorState))
        {
            return false;
        }

        if (actorState.Commitment is not null)
        {
            return false;
        }

        actorState.Commitment = commitment;
        actorState.MovingDodgeActive = commitment.GrantsMovingDodge;
        actorState.LastAction = commitment.ActionType;
        return true;
    }

    public bool TryResolveAttackPhase(ActionPointTurnState state)
    {
        if (state.TurnFinished || state.AttackPhaseResolved || !AllEligibleActorsCommitted(state))
        {
            return false;
        }

        state.AttackPhaseCommitments.Clear();

        foreach (TurnActorStepState actorState in GetCommittedCurrentStepStates(state))
        {
            TurnActionCommitment commitment = actorState.Commitment!;
            if (!commitment.IsAttackPriority)
            {
                continue;
            }

            state.AttackPhaseCommitments.Add(commitment);
            actorState.AttacksThisTurn++;
        }

        state.AttackPhaseResolved = true;
        return true;
    }

    public bool ResolveMovementPhaseAndSpendAp(ActionPointTurnState state)
    {
        if (state.TurnFinished || !state.AttackPhaseResolved || state.MovementPhaseResolved)
        {
            return false;
        }

        state.MovementPhaseCommitments.Clear();
        state.MovementContests.Clear();

        foreach (TurnActorStepState actorState in state.ActorStates.Values)
        {
            actorState.MovedPreviousStep = false;
        }

        List<TurnActionCommitment> movementCommitments = [];

        foreach (TurnActorStepState actorState in GetCommittedCurrentStepStates(state))
        {
            TurnActionCommitment commitment = actorState.Commitment!;

            if (commitment.IsMovementPriority
                && commitment.IsMovementLegal
                && !commitment.MovementCancelled
                && commitment.Destination.HasValue
                && actorState.Actor.IsAlive
                && !actorState.Actor.IsDisabled)
            {
                movementCommitments.Add(commitment);
            }
        }

        foreach (IGrouping<V2I, TurnActionCommitment> destinationGroup in movementCommitments.GroupBy(commitment => commitment.Destination!.Value))
        {
            TurnActionCommitment[] destinationCommitments = destinationGroup.ToArray();
            if (destinationCommitments.Length > 1)
            {
                state.MovementContests.Add(new MovementContest(destinationGroup.Key, destinationCommitments));
                continue;
            }

            TurnActionCommitment movementCommitment = destinationCommitments[0];
            state.MovementPhaseCommitments.Add(movementCommitment);

            TurnActorStepState actorState = state.ActorStates[movementCommitment.Actor];
            actorState.MovedThisTurn = true;
            actorState.MovedPreviousStep = true;
            actorState.TilesMovedThisTurn++;
            actorState.Momentum += 1f;
        }

        foreach (TurnActorStepState actorState in GetCommittedCurrentStepStates(state))
        {
            SpendActionPoint(actorState);
            actorState.MovingDodgeActive = false;
            actorState.Commitment = null;
        }

        state.MovementPhaseResolved = true;
        AdvanceStep(state);
        return true;
    }

    private void AdvanceStep(ActionPointTurnState state)
    {
        state.AttackPhaseResolved = false;
        state.MovementPhaseResolved = false;

        state.CurrentStep--;
        while (state.CurrentStep > 0 && GetEligibleActors(state).Count == 0)
        {
            state.CurrentStep--;
        }

        if (state.CurrentStep <= 0)
        {
            state.TurnFinished = true;
        }
    }

    private void SpendActionPoint(TurnActorStepState actorState)
    {
        if (actorState.Commitment is null)
        {
            return;
        }

        if (actorState.Commitment.EndsTurn)
        {
            actorState.RemainingAP = 0;
            return;
        }

        actorState.RemainingAP = Math.Max(0, actorState.RemainingAP - Math.Max(0, actorState.Commitment.ApCost));
    }

    private bool AllEligibleActorsCommitted(ActionPointTurnState state)
    {
        foreach (TurnActorStepState actorState in state.ActorStates.Values)
        {
            if (IsEligibleForCurrentStep(state, actorState) && actorState.Commitment is null)
            {
                return false;
            }
        }

        return true;
    }

    private IEnumerable<TurnActorStepState> GetCommittedCurrentStepStates(ActionPointTurnState state)
    {
        foreach (TurnActorStepState actorState in state.ActorStates.Values)
        {
            if (actorState.RemainingAP == state.CurrentStep && actorState.Commitment is not null)
            {
                yield return actorState;
            }
        }
    }

    private bool IsEligibleForCurrentStep(ActionPointTurnState state, TurnActorStepState actorState)
    {
        return !state.TurnFinished
            && actorState.RemainingAP == state.CurrentStep
            && actorState.Actor.IsAlive
            && !actorState.Actor.IsDisabled;
    }
}
