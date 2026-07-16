namespace DetectionTracking;

public interface IDetectable
{
    bool Lit { get; set; }
    float DetectionProgress { get; set; }
}
