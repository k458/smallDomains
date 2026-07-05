using System;

namespace SquareTileGrid;

public readonly struct SquareTileGridPosition : IEquatable<SquareTileGridPosition>
{
    public SquareTileGridPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }

    public bool Equals(SquareTileGridPosition other)
    {
        return X == other.X
            && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is SquareTileGridPosition other
            && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}
