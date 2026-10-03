using Essentials;

namespace MapController;

public struct MapEntityActionCommitment
{
    public MapEntityActionType ActionType { get; set; }
    public V2I TargetTile { get; set; }
    public int ApCost { get; set; }
    public bool EndsTurn { get; set; }
    public bool IsMovementLegal { get; set; }
    public bool MovementCancelled { get; set; }

    public bool IsAttackPriority => ActionType is MapEntityActionType.Attack or MapEntityActionType.OtherAttackPriority;
    public bool IsMovementPriority => ActionType is MapEntityActionType.Move or MapEntityActionType.OtherMovementPriority;
    public bool GrantsMovingDodge => ActionType == MapEntityActionType.Move && IsMovementLegal;

    public static MapEntityActionCommitment Wait()
    {
        return new MapEntityActionCommitment
        {
            ActionType = MapEntityActionType.Wait,
            ApCost = 0,
            IsMovementLegal = true
        };
    }
}

