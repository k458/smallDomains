namespace SquareTileGrid;

public interface ISquareTileGrid
{
    const int DefaultPathCost = 2;
    const int DefaultDiagonalPathCost = 3;

    int SizeX { get; }
    int SizeY { get; }
    ISquareTile?[][] TilesByPosition { get; }

    int PathCost { get; set; }
    int DiagonalPathCost { get; set; }

    bool TryGetTile(
        SquareTileGridPosition position,
        out ISquareTile? tile);
}
