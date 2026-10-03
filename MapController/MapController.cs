using Essentials;

namespace MapController;

public class MapController
{
    private readonly List<IMapEntity> mapEntities = [];
    private readonly MapControllerState state = new();

    public IReadOnlyCollection<IMapEntity> MapEntities => mapEntities;
    public MapControllerState State => state;
    public MapControllerPhase Phase { get; private set; } = MapControllerPhase.WaitingForInput;

    public void AddMapEntity(IMapEntity mapEntity)
    {
        if (mapEntities.Contains(mapEntity))
        {
            return;
        }

        mapEntities.Add(mapEntity);
    }

    public bool RemoveMapEntity(IMapEntity mapEntity)
    {
        state.EntityStates.Remove(mapEntity);
        state.ProposedCommitments.Remove(mapEntity);
        return mapEntities.Remove(mapEntity);
    }

    public void SetCommitment(IMapEntity mapEntity, MapEntityActionCommitment commitment)
    {
        if (!mapEntities.Contains(mapEntity))
        {
            return;
        }

        NormalizeCommitment(ref commitment);
        state.ProposedCommitments[mapEntity] = commitment;
    }

    public void StartOrContinueTurn()
    {
        if (Phase != MapControllerPhase.WaitingForInput)
        {
            return;
        }

        if (state.CurrentActionStep <= 0)
        {
            StartTurn();
            return;
        }

        Phase = MapControllerPhase.CommitmentUpdate;
    }
    public void Process(float deltaTime)
    {
        ProcessMapEntities(deltaTime);

        if (AnyMapEntityBusy())
        {
            return;
        }

        ProgressPhase();
    }

    protected virtual void ResolveAttacks(IReadOnlyCollection<IMapEntity> attackEntities)
    {
        _ = attackEntities;
    }

    protected virtual void ResolveMovement(
        IReadOnlyCollection<IMapEntity> movementEntities,
        IReadOnlyCollection<MapMovementContest> movementContests)
    {
        _ = movementEntities;
        _ = movementContests;
    }

    private void StartTurn()
    {
        state.EntityStates.Clear();
        state.AttackPhaseEntities.Clear();
        state.MovementPhaseEntities.Clear();
        state.MovementContests.Clear();
        state.CurrentActionStep = 1;

        foreach (IMapEntity mapEntity in mapEntities)
        {
            MapEntityTurnState entityState = new(mapEntity)
            {
                RemainingAP = Math.Clamp(mapEntity.Speed, 1, 3),
                MovedThisTurn = false,
                TilesMovedThisTurn = 0,
                MovedPreviousStep = false,
                AttacksThisTurn = 0,
                Recoil = 0f,
                Momentum = 0f,
                LastAction = MapEntityActionType.Undefined,
                WasHitThisTurn = false,
                MovingDodgeActive = false,
                Commitment = null
            };

            state.EntityStates.Add(mapEntity, entityState);
        }

        Phase = MapControllerPhase.CommitmentUpdate;
    }

    private void ProcessMapEntities(float deltaTime)
    {
        foreach (IMapEntity mapEntity in mapEntities.ToArray())
        {
            mapEntity.Process(deltaTime, Phase);
        }
    }

    private bool AnyMapEntityBusy()
    {
        foreach (IMapEntity mapEntity in mapEntities)
        {
            if (mapEntity.IsBusy)
            {
                return true;
            }
        }

        return false;
    }

    private void ProgressPhase()
    {
        switch (Phase)
        {

            case MapControllerPhase.WaitingForInput:

                return;
            case MapControllerPhase.CommitmentUpdate:
                UpdateCommitmentsForCurrentStep();
                Phase = MapControllerPhase.AttackPhase;
                break;
            case MapControllerPhase.AttackPhase:
                ResolveAttackPhase();
                Phase = MapControllerPhase.MovementPhase;
                break;
            case MapControllerPhase.MovementPhase:
                ResolveMovementPhase();
                ProgressAfterActionStep();
                break;
        }
    }

    private void UpdateCommitmentsForCurrentStep()
    {
        foreach (MapEntityTurnState entityState in state.EntityStates.Values)
        {
            if (!CanActInCurrentStep(entityState))
            {
                continue;
            }

            if (entityState.Commitment.HasValue)
            {
                ApplyDeclarationState(entityState);
                continue;
            }

            if (!state.ProposedCommitments.TryGetValue(entityState.MapEntity, out MapEntityActionCommitment commitment))
            {
                if (!entityState.MapEntity.TryUpdateCommitment(out commitment))
                {
                    commitment = MapEntityActionCommitment.Wait();
                }
            }

            NormalizeCommitment(ref commitment);
            entityState.Commitment = commitment;
            state.ProposedCommitments.Remove(entityState.MapEntity);
            ApplyDeclarationState(entityState);
        }
    }

    private void ApplyDeclarationState(MapEntityTurnState entityState)
    {
        if (!entityState.Commitment.HasValue)
        {
            return;
        }

        MapEntityActionCommitment commitment = entityState.Commitment.Value;
        entityState.MovingDodgeActive = commitment.GrantsMovingDodge;
        entityState.LastAction = commitment.ActionType;
    }

    private void ResolveAttackPhase()
    {
        state.AttackPhaseEntities.Clear();

        foreach (MapEntityTurnState entityState in GetCommittedActionStates())
        {
            MapEntityActionCommitment commitment = entityState.Commitment!.Value;
            if (!commitment.IsAttackPriority)
            {
                continue;
            }

            state.AttackPhaseEntities.Add(entityState.MapEntity);
            entityState.AttacksThisTurn++;
        }

        ResolveAttacks(state.AttackPhaseEntities);
    }

    private void ResolveMovementPhase()
    {
        state.MovementPhaseEntities.Clear();
        state.MovementContests.Clear();

        foreach (MapEntityTurnState entityState in state.EntityStates.Values)
        {
            entityState.MovedPreviousStep = false;
        }

        List<MapEntityTurnState> movementStates = [];

        foreach (MapEntityTurnState entityState in GetCommittedActionStates())
        {
            MapEntityActionCommitment commitment = entityState.Commitment!.Value;

            if (commitment.IsMovementPriority
                && commitment.IsMovementLegal
                && !commitment.MovementCancelled
                && entityState.MapEntity.IsAlive
                && !entityState.MapEntity.IsDisabled)
            {
                movementStates.Add(entityState);
            }
        }

        foreach (IGrouping<V2I, MapEntityTurnState> destinationGroup in movementStates.GroupBy(entityState => entityState.Commitment!.Value.TargetTile))
        {
            MapEntityTurnState[] destinationStates = destinationGroup.ToArray();
            if (destinationStates.Length > 1)
            {
                state.MovementContests.Add(new MapMovementContest(
                    destinationGroup.Key,
                    destinationStates.Select(entityState => entityState.MapEntity).ToArray()));
                continue;
            }

            MapEntityTurnState movementState = destinationStates[0];
            state.MovementPhaseEntities.Add(movementState.MapEntity);
            movementState.MovedThisTurn = true;
            movementState.MovedPreviousStep = true;
            movementState.TilesMovedThisTurn++;
            movementState.Momentum += 1f;
        }

        ResolveMovement(state.MovementPhaseEntities, state.MovementContests);
        SpendCommittedActionPoints();
    }

    private void SpendCommittedActionPoints()
    {
        foreach (MapEntityTurnState entityState in GetCommittedActionStates())
        {
            MapEntityActionCommitment commitment = entityState.Commitment!.Value;

            if (commitment.EndsTurn)
            {
                entityState.RemainingAP = 0;
            }
            else if (commitment.ActionType != MapEntityActionType.Wait)
            {
                entityState.RemainingAP = Math.Max(0, entityState.RemainingAP - Math.Max(0, commitment.ApCost));
            }

            entityState.MovingDodgeActive = false;
            entityState.Commitment = null;
        }
    }

    private void ProgressAfterActionStep()
    {
        state.CurrentActionStep++;

        while (state.CurrentActionStep <= 3 && !HasActorsWithActionPoints())
        {
            state.CurrentActionStep++;
        }

        if (state.CurrentActionStep > 3)
        {
            state.CurrentActionStep = 0;
            Phase = MapControllerPhase.WaitingForInput;
            return;
        }

        Phase = MapControllerPhase.WaitingForInput;
    }

    private IEnumerable<MapEntityTurnState> GetCommittedActionStates()
    {
        foreach (MapEntityTurnState entityState in state.EntityStates.Values)
        {
            if (entityState.RemainingAP > 0 && entityState.Commitment.HasValue)
            {
                yield return entityState;
            }
        }
    }

    private bool HasActorsWithActionPoints()
    {
        foreach (MapEntityTurnState entityState in state.EntityStates.Values)
        {
            if (CanActInCurrentStep(entityState))
            {
                return true;
            }
        }

        return false;
    }

    private bool CanActInCurrentStep(MapEntityTurnState entityState)
    {
        return entityState.RemainingAP > 0
            && entityState.MapEntity.IsAlive
            && !entityState.MapEntity.IsDisabled;
    }

    private void NormalizeCommitment(ref MapEntityActionCommitment commitment)
    {
        if (commitment.ActionType == MapEntityActionType.Wait)
        {
            commitment.ApCost = 0;
            return;
        }

        if (commitment.ApCost <= 0)
        {
            commitment.ApCost = 1;
        }
    }
}

