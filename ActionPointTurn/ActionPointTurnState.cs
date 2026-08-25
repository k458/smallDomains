namespace ActionPointTurn;

public class ActionPointTurnState
{
    public Dictionary<ITurnActor, TurnActorStepState> ActorStates { get; } = new();
    public List<TurnActionCommitment> AttackPhaseCommitments { get; } = new();
    public List<TurnActionCommitment> MovementPhaseCommitments { get; } = new();
    public List<MovementContest> MovementContests { get; } = new();

    public int CurrentStep { get; set; } = 3;
    public bool AttackPhaseResolved { get; set; }
    public bool MovementPhaseResolved { get; set; }
    public bool TurnFinished { get; set; }
}
