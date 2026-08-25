namespace ActionPointTurn;

public class TurnActorStepState
{
    public TurnActorStepState(ITurnActor actor)
    {
        Actor = actor;
    }

    public ITurnActor Actor { get; }
    public int RemainingAP { get; set; }
    public bool MovedThisTurn { get; set; }
    public int TilesMovedThisTurn { get; set; }
    public bool MovedPreviousStep { get; set; }
    public int AttacksThisTurn { get; set; }
    public float Recoil { get; set; }
    public float Momentum { get; set; }
    public TurnActionType LastAction { get; set; }
    public bool WasHitThisTurn { get; set; }
    public bool MovingDodgeActive { get; set; }
    public TurnActionCommitment? Commitment { get; set; }
}
