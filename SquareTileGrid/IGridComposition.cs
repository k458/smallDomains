using System.Collections.Generic;

namespace SquareTileGrid;

public interface IGridComposition
{
    const int AdjacentTileCount = 8;

    IReadOnlyDictionary<int, SquareTileGridCompositionLayer> LayersByIndex { get; }

    bool TryAddLayer(
        int layerIndex,
        ISquareTileGrid grid,
        SquareTileGridPosition startPosition);

    bool TryRemoveLayer(int layerIndex);
    void Recompose();

    bool TryGetTile(
        SquareTileGridPosition compositionPosition,
        out ISquareTile? tile);

    bool TryGetTilePosition(
        ISquareTile tile,
        out SquareTileGridPosition compositionPosition);

    bool TryGetAdjacentTile(
        ISquareTile tile,
        SquareTileAdjacencyType adjacencyType,
        int adjacentTileIndex,
        out ISquareTile? adjacentTile);
}
