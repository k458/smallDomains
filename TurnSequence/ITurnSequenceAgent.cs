namespace TurnSequence;

public interface ITurnSequenceAgent
{
    int RolledInitiative { get; }

    bool HasActionFinished { get; }

    void RollInitiative();

    void RoundStarted();

    void TurnStarted();
}
