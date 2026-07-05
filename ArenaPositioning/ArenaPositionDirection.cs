using System;

namespace ArenaPositioning;

public enum ArenaPositionDirection
{
    Left = -1,
    Right = 1,
    All = 0
}

internal static class ArenaPositionDirectionExtensions
{
    public static int ToStep(this ArenaPositionDirection direction)
    {
        return direction switch
        {
            ArenaPositionDirection.Left => -1,
            ArenaPositionDirection.Right => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unsupported arena position direction.")
        };
    }
}
