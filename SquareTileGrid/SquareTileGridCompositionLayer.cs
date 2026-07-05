namespace SquareTileGrid;

public readonly struct SquareTileGridCompositionLayer
{
    public SquareTileGridCompositionLayer(
        ISquareTileGrid grid,
        SquareTileGridPosition startPosition)
    {
        Grid = grid;
        StartPosition = startPosition;
    }

    public ISquareTileGrid Grid { get; }
    public SquareTileGridPosition StartPosition { get; }
}
