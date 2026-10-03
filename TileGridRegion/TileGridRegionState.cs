using Essentials;

namespace TileGridRegion;

public class TileGridRegionState
{
    public HashSet<V2I> BorderTiles { get; } = [];
    public List<HashSet<V2I>> Regions { get; } = [];
    public Dictionary<V2I, HashSet<V2I>> RegionByTile { get; } = new();
}
