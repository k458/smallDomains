using System.Collections.Generic;

namespace DetectionTracking;

public class DetectionService
{
    private readonly List<IDetector> detectors = new();
    private readonly List<IDetectable> detectables = new();
    private readonly List<IDetectable> confirmedDetectables = new();

    public IReadOnlyList<IDetector> Detectors => detectors;
    public IReadOnlyList<IDetectable> Detectables => detectables;
    public IReadOnlyList<IDetectable> ConfirmedDetectables => confirmedDetectables;

    public bool AddDetector(IDetector detector)
    {
        if (detectors.Contains(detector))
        {
            return false;
        }

        detectors.Add(detector);
        return true;
    }

    public bool RemoveDetector(IDetector detector)
    {
        return detectors.Remove(detector);
    }

    public bool AddDetectable(IDetectable detectable)
    {
        if (detectables.Contains(detectable))
        {
            return false;
        }

        detectables.Add(detectable);
        RefreshConfirmedDetectables();
        return true;
    }

    public bool RemoveDetectable(IDetectable detectable)
    {
        bool removed = detectables.Remove(detectable);

        if (removed)
        {
            confirmedDetectables.Remove(detectable);
        }

        return removed;
    }

    public void UpdateDetection()
    {
        foreach (IDetector detector in detectors)
        {
            foreach (IDetectable detectable in detectables)
            {
                detectable.DetectionProgress = System.MathF.Max(
                    0f,
                    detectable.DetectionProgress + detector.DetectionPower);
            }
        }

        RefreshConfirmedDetectables();
    }

    private void RefreshConfirmedDetectables()
    {
        confirmedDetectables.Clear();

        foreach (IDetectable detectable in detectables)
        {
            if (detectable.DetectionProgress > 1f)
            {
                confirmedDetectables.Add(detectable);
            }
        }
    }
}
