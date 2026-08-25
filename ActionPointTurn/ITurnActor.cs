namespace ActionPointTurn;

public interface ITurnActor
{
    int Speed { get; }
    bool IsAlive { get; }
    bool IsDisabled { get; }
}
