namespace SquareTileGrid;

internal readonly struct CompositionArea
{
    public CompositionArea(
        int minX,
        int minY,
        int maxX,
        int maxY)
    {
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
    }

    public int MinX { get; }
    public int MinY { get; }
    public int MaxX { get; }
    public int MaxY { get; }

    public CompositionArea Expand(int distance)
    {
        return new CompositionArea(
            MinX - distance,
            MinY - distance,
            MaxX + distance,
            MaxY + distance);
    }
}
