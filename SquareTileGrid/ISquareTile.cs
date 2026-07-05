namespace SquareTileGrid;

public interface ISquareTile
{
    SquareTileGridPosition Position { get; }
    ISquareTileGrid ParentGrid { get; }
}
