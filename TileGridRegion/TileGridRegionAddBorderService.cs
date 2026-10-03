using Essentials;

namespace TileGridRegion;

public class TileGridRegionAddBorderService
{
    private static readonly V2I[] ClockwiseDirections =
    [
        new V2I(0, -1),
        new V2I(1, -1),
        new V2I(1, 0),
        new V2I(1, 1),
        new V2I(0, 1),
        new V2I(-1, 1),
        new V2I(-1, 0),
        new V2I(-1, -1)
    ];

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

        List<V2I> potentialRegionTiles = GetPotentialRegionTiles(state, position);
        if (potentialRegionTiles.Count < 2)
        {
            return true;
        }

        HashSet<HashSet<V2I>> removedRegions = RemoveRegionsForTiles(state, potentialRegionTiles);
        HashSet<V2I>? rebuildScope = GetRebuildScope(removedRegions);
        HashSet<V2I> rebuiltTiles = [];

        foreach (V2I potentialRegionTile in potentialRegionTiles)
        {
            if (rebuiltTiles.Contains(potentialRegionTile)
                || state.BorderTiles.Contains(potentialRegionTile)
                || !CanUseTile(potentialRegionTile, rebuildScope))
            {
                continue;
            }

            HashSet<V2I> region = BuildRegion(state, potentialRegionTile, rebuildScope);
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

    private List<V2I> GetPotentialRegionTiles(TileGridRegionState state, V2I position)
    {
        List<V2I> potentialRegionTiles = [];
        HashSet<V2I> uniqueTiles = [];

        for (int i = 0; i < ClockwiseDirections.Length; i++)
        {
            V2I borderCandidate = position + ClockwiseDirections[i];
            if (!state.BorderTiles.Contains(borderCandidate))
            {
                continue;
            }

            V2I potentialRegionTile = position + ClockwiseDirections[(i + 1) % ClockwiseDirections.Length];
            if (state.BorderTiles.Contains(potentialRegionTile)
                || !uniqueTiles.Add(potentialRegionTile))
            {
                continue;
            }

            potentialRegionTiles.Add(potentialRegionTile);
        }

        return potentialRegionTiles;
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

    private HashSet<V2I>? GetRebuildScope(IEnumerable<HashSet<V2I>> regions)
    {
        HashSet<V2I> scope = [];

        foreach (HashSet<V2I> region in regions)
        {
            scope.UnionWith(region);
        }

        return scope.Count > 0 ? scope : null;
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
