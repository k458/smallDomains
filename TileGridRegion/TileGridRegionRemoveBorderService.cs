using Essentials;

namespace TileGridRegion;

public class TileGridRegionRemoveBorderService
{
    private static readonly V2I[] CardinalDirections =
    [
        new V2I(0, -1),
        new V2I(1, 0),
        new V2I(0, 1),
        new V2I(-1, 0)
    ];

    public ITileValidator? TileValidator { get; set; }

    public bool RemoveBorderTile(TileGridRegionState state, V2I position)
    {
        if (!state.BorderTiles.Remove(position))
        {
            return false;
        }

        List<V2I> rebuildSeeds = GetRebuildSeeds(position);
        HashSet<HashSet<V2I>> removedRegions = RemoveRegionsForTiles(state, rebuildSeeds);
        HashSet<V2I>? rebuildScope = GetRebuildScope(removedRegions, position);
        HashSet<V2I> rebuiltTiles = [];

        foreach (V2I rebuildSeed in rebuildSeeds)
        {
            if (rebuiltTiles.Contains(rebuildSeed)
                || state.BorderTiles.Contains(rebuildSeed)
                || !CanUseTile(rebuildSeed, rebuildScope))
            {
                continue;
            }

            HashSet<V2I> region = BuildRegion(state, rebuildSeed, rebuildScope);
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

    private List<V2I> GetRebuildSeeds(V2I position)
    {
        List<V2I> rebuildSeeds = [position];

        foreach (V2I direction in CardinalDirections)
        {
            rebuildSeeds.Add(position + direction);
        }

        return rebuildSeeds;
    }

    private HashSet<HashSet<V2I>> RemoveRegionsForTiles(TileGridRegionState state, IEnumerable<V2I> tiles)
    {
        HashSet<HashSet<V2I>> regions = [];

        foreach (V2I tile in tiles)
        {
            if (state.RegionByTile.TryGetValue(tile, out HashSet<V2I>? region))
            {
                regions.Add(region);
            }
        }

        foreach (HashSet<V2I> region in regions)
        {
            state.Regions.Remove(region);
            foreach (V2I tile in region)
            {
                state.RegionByTile.Remove(tile);
            }
        }

        return regions;
    }

    private HashSet<V2I>? GetRebuildScope(IEnumerable<HashSet<V2I>> regions, V2I openedTile)
    {
        HashSet<V2I> scope = [openedTile];

        foreach (HashSet<V2I> region in regions)
        {
            scope.UnionWith(region);
        }

        return scope.Count > 1 || TileValidator is null ? scope : null;
    }

    private HashSet<V2I> BuildRegion(TileGridRegionState state, V2I start, HashSet<V2I>? rebuildScope)
    {
        HashSet<V2I> region = [];
        Queue<V2I> openTiles = [];

        openTiles.Enqueue(start);

        while (openTiles.Count > 0)
        {
            V2I tile = openTiles.Dequeue();
            if (region.Contains(tile)
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
