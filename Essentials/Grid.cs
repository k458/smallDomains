namespace Essentials;

public class Grid
{
    private readonly bool[,] discovered;
    private readonly bool[,] blocked;
    private readonly long[,] version;

    public Grid(int gridSize)
    {
        if (gridSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(gridSize), gridSize, "Grid size must be greater than zero.");
        }

        SizeX = gridSize;
        SizeY = gridSize;
        discovered = new bool[SizeX, SizeY];
        blocked = new bool[SizeX, SizeY];
        version = new long[SizeX, SizeY];
    }

    public int SizeX { get; }
    public int SizeY { get; }
    public bool[,] Discovered => discovered;
    public bool[,] Blocked => blocked;
    public long[,] Version => version;

    public bool IsInside(V2I position)
    {
        return position.X >= 0
            && position.Y >= 0
            && position.X < SizeX
            && position.Y < SizeY;
    }

    public bool TrySetDiscovered(V2I position, bool value)
    {
        if (!IsInside(position))
        {
            return false;
        }

        discovered[position.X, position.Y] = value;
        version[position.X, position.Y]++;
        return true;
    }

    public bool TrySetBlocked(V2I position, bool value)
    {
        if (!IsInside(position))
        {
            return false;
        }

        blocked[position.X, position.Y] = value;
        version[position.X, position.Y]++;
        return true;
    }

    public bool TryGetDiscovered(V2I position, out bool value)
    {
        value = default;

        if (!IsInside(position))
        {
            return false;
        }

        value = discovered[position.X, position.Y];
        return true;
    }

    public bool TryGetBlocked(V2I position, out bool value)
    {
        value = default;

        if (!IsInside(position))
        {
            return false;
        }

        value = blocked[position.X, position.Y];
        return true;
    }

    public void Clear()
    {
        Array.Clear(discovered);
        Array.Clear(blocked);
        Array.Clear(version);
    }
}
