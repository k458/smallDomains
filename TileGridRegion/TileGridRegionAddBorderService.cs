using Essentials;

namespace TileGridRegion;

public class TileGridRegionAddBorderService
{
    private static readonly V2I[] CardinalDirections =
    [
        new V2I(0, -1),
        new V2I(1, 0),
        new V2I(0, 1),
        new V2I(-1, 0)
    ];

    public ITileValidator? TileValidator { get; set; }

    public bool AddBorderTile(TileGridRegionState state, V2I position)
    {
        if (!state.BorderTiles.Add(position))
        {
            return false;
        }

        RemoveTileFromRegion(state, position);

        List<V2I> regionSeeds = GetEmptyAdjacentTiles(state, position);
        if (regionSeeds.Count == 0)
        {
            return true;
        }

        HashSet<V2I>? rebuildScope = RemoveRegionsForTiles(state, regionSeeds);
        HashSet<V2I> rebuiltTiles = [];

        foreach (V2I regionSeed in regionSeeds)
        {
            if (rebuiltTiles.Contains(regionSeed)
                || state.BorderTiles.Contains(regionSeed)
                || !CanUseTile(regionSeed, rebuildScope))
            {
                continue;
            }

            HashSet<V2I> region = BuildRegion(state, regionSeed, rebuildScope, rebuiltTiles);
            if (region.Count == 0)
            {
                continue;
            }

            state.Regions.Add(region);
            foreach (V2I tile in region)
            {
                state.RegionByTile[tile] = region;
                rebuiltTiles.Add(tile);
            }
        }

        return true;
    }

    private void RemoveTileFromRegion(TileGridRegionState state, V2I position)
    {
        if (!state.RegionByTile.TryGetValue(position, out HashSet<V2I>? region))
        {
            return;
        }

        region.Remove(position);
        state.RegionByTile.Remove(position);

        if (region.Count == 0)
        {
            state.Regions.Remove(region);
        }
    }

    private List<V2I> GetEmptyAdjacentTiles(TileGridRegionState state, V2I position)
    {
        List<V2I> adjacentTiles = [];

        foreach (V2I direction in CardinalDirections)
        {
            V2I adjacentTile = position + direction;
            if (state.BorderTiles.Contains(adjacentTile)
                || TileValidator?.Validate(adjacentTile.X, adjacentTile.Y) == false)
            {
                continue;
            }

            adjacentTiles.Add(adjacentTile);
        }

        return adjacentTiles;
    }

    private HashSet<V2I>? RemoveRegionsForTiles(TileGridRegionState state, IEnumerable<V2I> tiles)
    {
        HashSet<HashSet<V2I>> regionsToRemove = [];

        foreach (V2I tile in tiles)
        {
            if (state.RegionByTile.TryGetValue(tile, out HashSet<V2I>? region))
            {
                regionsToRemove.Add(region);
            }
        }

        HashSet<V2I> rebuildScope = [];
        foreach (HashSet<V2I> region in regionsToRemove)
        {
            rebuildScope.UnionWith(region);
            state.Regions.Remove(region);
            foreach (V2I tile in region)
            {
                state.RegionByTile.Remove(tile);
            }
        }

        return rebuildScope.Count > 0 ? rebuildScope : null;
    }

    private HashSet<V2I> BuildRegion(
        TileGridRegionState state,
        V2I start,
        HashSet<V2I>? rebuildScope,
        HashSet<V2I> rebuiltTiles)
    {
        HashSet<V2I> region = [];
        Queue<V2I> openTiles = [];

        openTiles.Enqueue(start);

        while (openTiles.Count > 0)
        {
            V2I tile = openTiles.Dequeue();
            if (region.Contains(tile)
                || rebuiltTiles.Contains(tile)
                || state.BorderTiles.Contains(tile)
                || !CanUseTile(tile, rebuildScope))
            {
                continue;
            }

            region.Add(tile);

            foreach (V2I direction in CardinalDirections)
            {
                openTiles.Enqueue(tile + direction);
            }
        }

        return region;
    }

    private bool CanUseTile(V2I tile, HashSet<V2I>? rebuildScope)
    {
        if (rebuildScope is not null)
        {
            return rebuildScope.Contains(tile);
        }

        return TileValidator?.Validate(tile.X, tile.Y) == true;
    }
}
