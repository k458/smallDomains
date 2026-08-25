namespace MapController;

public class MapEntityTurnState
{
    public MapEntityTurnState(IMapEntity mapEntity)
    {
        MapEntity = mapEntity;
    }

    public IMapEntity MapEntity { get; }
    public int RemainingAP { get; set; }
    public bool MovedThisTurn { get; set; }
    public int TilesMovedThisTurn { get; set; }
    public bool MovedPreviousStep { get; set; }
    public int AttacksThisTurn { get; set; }
    public float Recoil { get; set; }
    public float Momentum { get; set; }
    public MapEntityActionType LastAction { get; set; }
    public bool WasHitThisTurn { get; set; }
    public bool MovingDodgeActive { get; set; }
    public MapEntityActionCommitment? Commitment { get; set; }
}
