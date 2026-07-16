using System.Numerics;

namespace DetectionTracking;

public interface IDetectable
{
    Vector2 Position2D { get; }
    bool Lit { get; set; }
    float DetectionProgress { get; set; }
}
