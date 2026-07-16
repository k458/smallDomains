using System.Numerics;

namespace DetectionTracking;

public interface IDetector
{
    Vector2 Position2D { get; }
    float DetectionPower { get; }
}
