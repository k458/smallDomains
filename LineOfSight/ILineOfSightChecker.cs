using System.Numerics;

namespace LineOfSight;

public interface ILineOfSightChecker
{
    bool IsLineOfSightClear(Vector2 from, Vector2 to);
    bool IsLineOfSightClearBypassingObstacles(Vector2 from, Vector2 to);
}
