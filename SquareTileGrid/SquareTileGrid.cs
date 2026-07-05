namespace SquareTileGrid;

public class SquareTileGrid : ISquareTileGrid
{
    public SquareTileGrid(int sizeX, int sizeY)
    {
        if (sizeX <= 0)
        {
            throw new System.ArgumentOutOfRangeException(
                nameof(sizeX),
                "Grid size X must be greater than zero.");
        }

        if (sizeY <= 0)
        {
            throw new System.ArgumentOutOfRangeException(
                nameof(sizeY),
                "Grid size Y must be greater than zero.");
        }

        SizeX = sizeX;
        SizeY = sizeY;
        TilesByPosition = new ISquareTile?[sizeX][];

        for (int x = 0; x < sizeX; x++)
        {
            TilesByPosition[x] = new ISquareTile?[sizeY];

            for (int y = 0; y < sizeY; y++)
            {
                SquareTile tile = new(new SquareTileGridPosition(x, y));
                tile.AttachToGrid(this);
                TilesByPosition[x][y] = tile;
            }
        }
    }

    public int SizeX { get; }
    public int SizeY { get; }
    public ISquareTile?[][] TilesByPosition { get; }

    public int PathCost { get; set; } = ISquareTileGrid.DefaultPathCost;
    public int DiagonalPathCost { get; set; } = ISquareTileGrid.DefaultDiagonalPathCost;

    public bool TryGetTile(
        SquareTileGridPosition position,
        out ISquareTile? tile)
    {
        tile = null;

        if (position.X < 0
            || position.Y < 0
            || position.X >= SizeX
            || position.Y >= SizeY)
        {
            return false;
        }

        tile = TilesByPosition[position.X][position.Y];
        return tile is not null;
    }
}
