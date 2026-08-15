namespace Shared;

public readonly record struct V2I(int X, int Y) : IComparable<V2I>
{
    public static V2I operator +(V2I left, V2I right)
    {
        return new V2I(left.X + right.X, left.Y + right.Y);
    }

    public static V2I operator -(V2I left, V2I right)
    {
        return new V2I(left.X - right.X, left.Y - right.Y);
    }

    public int CompareTo(V2I other)
    {
        int xComparison = X.CompareTo(other.X);
        if (xComparison != 0)
        {
            return xComparison;
        }

        return Y.CompareTo(other.Y);
    }

    public bool IsSamePosition(V2I other)
    {
        return this == other;
    }

    public bool IsAdjacentCardinal(V2I other)
    {
        V2I delta = this - other;
        return Math.Abs(delta.X) + Math.Abs(delta.Y) == 1;
    }
}
