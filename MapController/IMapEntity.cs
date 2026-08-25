namespace MapController;

public interface IMapEntity
{
    int Speed { get; }
    bool IsAlive { get; }
    bool IsDisabled { get; }
    bool IsBusy { get; }

    void Process(float deltaTime, MapControllerPhase phase);

    bool TryUpdateCommitment(out MapEntityActionCommitment commitment);
}
