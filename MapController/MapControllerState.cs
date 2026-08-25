namespace MapController;

public class MapControllerState
{
    public Dictionary<IMapEntity, MapEntityTurnState> EntityStates { get; } = new();
    public Dictionary<IMapEntity, MapEntityActionCommitment> ProposedCommitments { get; } = new();
    public List<IMapEntity> AttackPhaseEntities { get; } = new();
    public List<IMapEntity> MovementPhaseEntities { get; } = new();
    public List<MapMovementContest> MovementContests { get; } = new();

    public int CurrentSpeed { get; set; }
}
