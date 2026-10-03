using Essentials;

namespace TileGridRegion;

public class TileGridRegionService
{
    public bool AddBorderTile(TileGridRegionState state, V2I position)
    {
        return state.BorderTiles.Add(position);
    }

    public bool RemoveBorderTile(TileGridRegionState state, V2I position)
    {
        return state.BorderTiles.Remove(position);
    }
}
