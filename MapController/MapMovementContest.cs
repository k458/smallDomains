using Essentials;

namespace MapController;

public class MapMovementContest
{
    public MapMovementContest(V2I destination, IReadOnlyCollection<IMapEntity> mapEntities)
    {
        Destination = destination;
        MapEntities = mapEntities;
    }

    public V2I Destination { get; }
    public IReadOnlyCollection<IMapEntity> MapEntities { get; }
}
