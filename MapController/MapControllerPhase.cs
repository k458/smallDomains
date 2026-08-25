namespace MapController;

public enum MapControllerPhase
{
    Undefined,
    TurnNotStarted,
    WaitForPlayerInput,
    CommitmentUpdate,
    AttackPhase,
    MovementPhase
}
