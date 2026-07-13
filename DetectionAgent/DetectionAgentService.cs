namespace DetectionAgent;

public class DetectionAgentService
{
    public void UpdateDetectionProgress(
        IDetectionAgentViewer viewer,
        IDetectionAgentObject target,
        float distance)
    {
        target.DetectionProgress = System.MathF.Max(
            0f,
            target.DetectionProgress + viewer.DetectionPower);
    }
}
