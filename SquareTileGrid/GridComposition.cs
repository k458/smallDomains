using System.Collections.Generic;

namespace SquareTileGrid;

public class GridComposition : IGridComposition
{
    private static readonly int[] adjacentOffsetX = { -1, 0, 1, -1, 1, -1, 0, 1 };
    private static readonly int[] adjacentOffsetY = { -1, -1, -1, 0, 0, 1, 1, 1 };

    private readonly Dictionary<int, SquareTileGridCompositionLayer> layersByIndex = new();
    private readonly Dictionary<SquareTileGridPosition, ISquareTile> tilesByPosition = new();
    private readonly Dictionary<ISquareTile, SquareTileGridPosition> positionsByTile = new();
    private readonly Dictionary<ISquareTile, ISquareTile?[][]> adjacentTilesByTile = new();
    private readonly Stack<ISquareTile?[][]> storedAdjacentTilesByType = new();

    public IReadOnlyDictionary<int, SquareTileGridCompositionLayer> LayersByIndex => layersByIndex;

    public bool TryAddLayer(
        int layerIndex,
        ISquareTileGrid grid,
        SquareTileGridPosition startPosition)
    {
        if (layersByIndex.ContainsKey(layerIndex))
        {
            return false;
        }

        SquareTileGridCompositionLayer layer =
            new SquareTileGridCompositionLayer(grid, startPosition);
        layersByIndex[layerIndex] = layer;
        RecomposeArea(GetLayerArea(layer));
        return true;
    }

    public bool TryRemoveLayer(int layerIndex)
    {
        if (!layersByIndex.TryGetValue(
                layerIndex,
                out SquareTileGridCompositionLayer layer))
        {
            return false;
        }

        if (!layersByIndex.Remove(layerIndex))
        {
            return false;
        }

        RecomposeArea(GetLayerArea(layer));
        return true;
    }

    public void Recompose()
    {
        ReturnAllAdjacentTilesByType();
        tilesByPosition.Clear();
        positionsByTile.Clear();
        adjacentTilesByTile.Clear();

        List<int> layerIndices = new(layersByIndex.Keys);
        layerIndices.Sort();

        foreach (int layerIndex in layerIndices)
        {
            SquareTileGridCompositionLayer layer = layersByIndex[layerIndex];

            for (int x = 0; x < layer.Grid.SizeX; x++)
            {
                for (int y = 0; y < layer.Grid.SizeY; y++)
                {
                    if (!layer.Grid.TryGetTile(
                            new SquareTileGridPosition(x, y),
                            out ISquareTile? tile))
                    {
                        continue;
                    }

                    SquareTileGridPosition compositionPosition =
                        new SquareTileGridPosition(
                            layer.StartPosition.X + x,
                            layer.StartPosition.Y + y);

                    if (tilesByPosition.TryGetValue(
                            compositionPosition,
                            out ISquareTile? previousTile))
                    {
                        positionsByTile.Remove(previousTile);
                    }

                    tilesByPosition[compositionPosition] = tile!;
                    positionsByTile[tile!] = compositionPosition;
                }
            }
        }

        foreach (KeyValuePair<ISquareTile, SquareTileGridPosition> tilePosition in positionsByTile)
        {
            adjacentTilesByTile[tilePosition.Key] =
                GetAdjacentTilesByType(tilePosition.Value);
        }
    }

    public bool TryGetTile(
        SquareTileGridPosition compositionPosition,
        out ISquareTile? tile)
    {
        return tilesByPosition.TryGetValue(compositionPosition, out tile);
    }

    public bool TryGetTilePosition(
        ISquareTile tile,
        out SquareTileGridPosition compositionPosition)
    {
        return positionsByTile.TryGetValue(tile, out compositionPosition);
    }

    public bool TryGetAdjacentTile(
        ISquareTile tile,
        SquareTileAdjacencyType adjacencyType,
        int adjacentTileIndex,
        out ISquareTile? adjacentTile)
    {
        adjacentTile = null;

        if (adjacentTileIndex < 0
            || adjacentTileIndex >= IGridComposition.AdjacentTileCount
            || !adjacentTilesByTile.TryGetValue(
                tile,
                out ISquareTile?[][]? adjacentTilesByType))
        {
            return false;
        }

        int adjacencyTypeIndex = (int)adjacencyType;

        if (adjacencyTypeIndex < 0
            || adjacencyTypeIndex >= adjacentTilesByType.Length)
        {
            return false;
        }

        adjacentTile = adjacentTilesByType[adjacencyTypeIndex][adjacentTileIndex];
        return adjacentTile is not null;
    }

    private void RecomposeArea(CompositionArea area)
    {
        for (int x = area.MinX; x <= area.MaxX; x++)
        {
            for (int y = area.MinY; y <= area.MaxY; y++)
            {
                UpdateVisibleTile(new SquareTileGridPosition(x, y));
            }
        }

        CompositionArea adjacentArea = area.Expand(1);

        for (int x = adjacentArea.MinX; x <= adjacentArea.MaxX; x++)
        {
            for (int y = adjacentArea.MinY; y <= adjacentArea.MaxY; y++)
            {
                RebuildAdjacentTilesByType(new SquareTileGridPosition(x, y));
            }
        }
    }

    private void UpdateVisibleTile(SquareTileGridPosition position)
    {
        if (tilesByPosition.TryGetValue(position, out ISquareTile? previousTile))
        {
            tilesByPosition.Remove(position);
            positionsByTile.Remove(previousTile);
            ReturnAdjacentTilesByType(previousTile);
        }

        if (!TryResolveVisibleTile(position, out ISquareTile? visibleTile))
        {
            return;
        }

        tilesByPosition[position] = visibleTile!;
        positionsByTile[visibleTile!] = position;
    }

    private bool TryResolveVisibleTile(
        SquareTileGridPosition compositionPosition,
        out ISquareTile? tile)
    {
        tile = null;
        bool foundTile = false;
        int foundLayerIndex = int.MinValue;

        foreach (KeyValuePair<int, SquareTileGridCompositionLayer> layerByIndex in layersByIndex)
        {
            if (layerByIndex.Key < foundLayerIndex
                || !TryGetLayerTile(
                    layerByIndex.Value,
                    compositionPosition,
                    out ISquareTile? layerTile))
            {
                continue;
            }

            tile = layerTile;
            foundTile = true;
            foundLayerIndex = layerByIndex.Key;
        }

        return foundTile;
    }

    private static bool TryGetLayerTile(
        SquareTileGridCompositionLayer layer,
        SquareTileGridPosition compositionPosition,
        out ISquareTile? tile)
    {
        SquareTileGridPosition layerPosition =
            new SquareTileGridPosition(
                compositionPosition.X - layer.StartPosition.X,
                compositionPosition.Y - layer.StartPosition.Y);

        return layer.Grid.TryGetTile(layerPosition, out tile);
    }

    private void RebuildAdjacentTilesByType(SquareTileGridPosition position)
    {
        if (!TryGetTile(position, out ISquareTile? tile))
        {
            return;
        }

        ReturnAdjacentTilesByType(tile!);
        adjacentTilesByTile[tile!] = GetAdjacentTilesByType(position);
    }

    private ISquareTile?[][] GetAdjacentTilesByType(SquareTileGridPosition position)
    {
        ISquareTile?[][] adjacentTilesByType =
            storedAdjacentTilesByType.Count > 0
                ? storedAdjacentTilesByType.Pop()
                : CreateAdjacentTilesByType();

        ClearAdjacentTilesByType(adjacentTilesByType);
        FillAdjacentTilesByType(
            position,
            adjacentTilesByType,
            SquareTileAdjacencyType.Near);
        FillAdjacentTilesByType(
            position,
            adjacentTilesByType,
            SquareTileAdjacencyType.Passable);
        FillAdjacentTilesByType(
            position,
            adjacentTilesByType,
            SquareTileAdjacencyType.PassableByForce);

        return adjacentTilesByType;
    }

    private static ISquareTile?[][] CreateAdjacentTilesByType()
    {
        int adjacencyTypeCount = System.Enum.GetValues<SquareTileAdjacencyType>().Length;
        ISquareTile?[][] adjacentTilesByType = new ISquareTile?[adjacencyTypeCount][];

        for (int i = 0; i < adjacentTilesByType.Length; i++)
        {
            adjacentTilesByType[i] = new ISquareTile?[IGridComposition.AdjacentTileCount];
        }

        return adjacentTilesByType;
    }

    private static void ClearAdjacentTilesByType(ISquareTile?[][] adjacentTilesByType)
    {
        for (int i = 0; i < adjacentTilesByType.Length; i++)
        {
            System.Array.Clear(adjacentTilesByType[i]);
        }
    }

    private void ReturnAllAdjacentTilesByType()
    {
        foreach (ISquareTile?[][] adjacentTilesByType in adjacentTilesByTile.Values)
        {
            ClearAdjacentTilesByType(adjacentTilesByType);
            storedAdjacentTilesByType.Push(adjacentTilesByType);
        }
    }

    private void ReturnAdjacentTilesByType(ISquareTile tile)
    {
        if (!adjacentTilesByTile.Remove(tile, out ISquareTile?[][]? adjacentTilesByType))
        {
            return;
        }

        ClearAdjacentTilesByType(adjacentTilesByType);
        storedAdjacentTilesByType.Push(adjacentTilesByType);
    }

    private void FillAdjacentTilesByType(
        SquareTileGridPosition position,
        ISquareTile?[][] adjacentTilesByType,
        SquareTileAdjacencyType adjacencyType)
    {
        ISquareTile?[] adjacentTiles = adjacentTilesByType[(int)adjacencyType];

        for (int i = 0; i < IGridComposition.AdjacentTileCount; i++)
        {
            SquareTileGridPosition adjacentPosition = GetAdjacentPosition(position, i);
            TryGetTile(adjacentPosition, out ISquareTile? adjacentTile);
            adjacentTiles[i] = adjacentTile;
        }
    }

    private static SquareTileGridPosition GetAdjacentPosition(
        SquareTileGridPosition position,
        int adjacentTileIndex)
    {
        return new SquareTileGridPosition(
            position.X + adjacentOffsetX[adjacentTileIndex],
            position.Y + adjacentOffsetY[adjacentTileIndex]);
    }

    private static CompositionArea GetLayerArea(SquareTileGridCompositionLayer layer)
    {
        return new CompositionArea(
            layer.StartPosition.X,
            layer.StartPosition.Y,
            layer.StartPosition.X + layer.Grid.SizeX - 1,
            layer.StartPosition.Y + layer.Grid.SizeY - 1);
    }
}
