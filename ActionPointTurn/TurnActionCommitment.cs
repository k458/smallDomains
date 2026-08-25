using Shared;

namespace ActionPointTurn;

public class TurnActionCommitment
{
    public TurnActionCommitment(ITurnActor actor, TurnActionType actionType)
    {
        Actor = actor;
        ActionType = actionType;
    }

    public ITurnActor Actor { get; }
    public TurnActionType ActionType { get; set; }
    public V2I? Destination { get; set; }
    public int ApCost { get; set; } = 1;
    public bool EndsTurn { get; set; }
    public bool IsMovementLegal { get; set; } = true;
    public bool MovementCancelled { get; set; }

    public bool IsAttackPriority => ActionType is TurnActionType.Attack or TurnActionType.OtherAttackPriority;
    public bool IsMovementPriority => ActionType is TurnActionType.Move or TurnActionType.OtherMovementPriority;
    public bool GrantsMovingDodge => ActionType == TurnActionType.Move && IsMovementLegal && Destination.HasValue;
}
