using Shared;

namespace ActionPointTurn;

public class MovementContest
{
    public MovementContest(V2I destination, IReadOnlyCollection<TurnActionCommitment> commitments)
    {
        Destination = destination;
        Commitments = commitments;
    }

    public V2I Destination { get; }
    public IReadOnlyCollection<TurnActionCommitment> Commitments { get; }
}
