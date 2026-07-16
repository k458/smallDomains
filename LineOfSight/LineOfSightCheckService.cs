using System;
using System.Numerics;

namespace LineOfSight;

public static class LineOfSightCheckService
{
    public static ILineOfSightChecker? LineOfSightChecker { get; set; }

    public static bool IsLineOfSightClear(
        Vector2 from,
        Vector2 to,
        bool bypassObstacles)
    {
        ILineOfSightChecker checker = LineOfSightChecker
            ?? throw new InvalidOperationException("Line of sight checker is not assigned.");

        return bypassObstacles
            ? checker.IsLineOfSightClearBypassingObstacles(from, to)
            : checker.IsLineOfSightClear(from, to);
    }
}
