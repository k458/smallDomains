namespace SquareTileGrid;

public class SquareTile : ISquareTile
{
    public SquareTile(SquareTileGridPosition position)
    {
        Position = position;
    }

    public SquareTileGridPosition Position { get; }
    public ISquareTileGrid ParentGrid { get; private set; } = null!;

    public void AttachToGrid(ISquareTileGrid parentGrid)
    {
        ParentGrid = parentGrid;
    }
}
