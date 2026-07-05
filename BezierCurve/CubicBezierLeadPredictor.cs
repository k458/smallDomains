using System.Numerics;

public class CubicBezierLeadPredictor
{
    private const float ForwardSampleDistance = 100f;
    private const float BackwardSampleDistance = 10f;
    private const int LengthEstimateSteps = 32;
    private const int BinarySearchIterations = 16;

    public bool TryGetInterceptPoint(
        BezierTrajectoryCubic targetTrajectory,
        float minimumTrajectoryTime,
        Vector2 projectileOrigin,
        float projectileSpeed,
        out Vector2 interceptPoint)
    {
        if (projectileSpeed <= 0f)
        {
            interceptPoint = default;
            return false;
        }

        float startT = Math.Clamp(minimumTrajectoryTime, 0f, 1f);
        float trajectoryLength = EstimateCubicLength(targetTrajectory);
        Vector2 startPoint = GetCubicPoint(targetTrajectory, startT);
        float startDelta = GetProjectileTimeDelta(startPoint, startT, projectileOrigin, projectileSpeed);

        if (startDelta <= 0f)
        {
            interceptPoint = startPoint;
            return true;
        }

        if (trajectoryLength <= 0f)
        {
            interceptPoint = default;
            return false;
        }

        float forwardStepT = Math.Clamp(ForwardSampleDistance / trajectoryLength, 0f, 1f);
        float backwardStepT = Math.Clamp(BackwardSampleDistance / trajectoryLength, 0f, 1f);
        float previousT = startT;
        float reachableT = 1f;
        bool foundReachablePoint = false;

        for (float t = Math.Min(startT + forwardStepT, 1f); t <= 1f; t = Math.Min(t + forwardStepT, 1f))
        {
            Vector2 point = GetCubicPoint(targetTrajectory, t);
            float delta = GetProjectileTimeDelta(point, t, projectileOrigin, projectileSpeed);

            if (delta <= 0f)
            {
                reachableT = t;
                foundReachablePoint = true;
                break;
            }

            if (t >= 1f)
            {
                break;
            }

            previousT = t;
        }

        if (!foundReachablePoint)
        {
            interceptPoint = default;
            return false;
        }

        float latestReachableT = reachableT;

        for (float t = reachableT - backwardStepT; t > previousT; t -= backwardStepT)
        {
            Vector2 point = GetCubicPoint(targetTrajectory, t);
            float delta = GetProjectileTimeDelta(point, t, projectileOrigin, projectileSpeed);

            if (delta <= 0f)
            {
                latestReachableT = t;
            }
            else
            {
                interceptPoint = GetCubicPoint(targetTrajectory, latestReachableT);
                return true;
            }
        }

        interceptPoint = GetCubicPoint(targetTrajectory, latestReachableT);
        return true;
    }

    public bool TryGetInterceptPointBinary(
        BezierTrajectoryCubic targetTrajectory,
        float minimumTrajectoryTime,
        Vector2 projectileOrigin,
        float projectileSpeed,
        out Vector2 interceptPoint)
    {
        if (projectileSpeed <= 0f)
        {
            interceptPoint = default;
            return false;
        }

        float startT = Math.Clamp(minimumTrajectoryTime, 0f, 1f);
        float startDelta = GetProjectileTimeDelta(
            targetTrajectory,
            startT,
            projectileOrigin,
            projectileSpeed);

        if (startDelta <= 0f)
        {
            interceptPoint = GetCubicPoint(targetTrajectory, startT);
            return true;
        }

        float endDelta = GetProjectileTimeDelta(
            targetTrajectory,
            1f,
            projectileOrigin,
            projectileSpeed);

        if (endDelta > 0f)
        {
            interceptPoint = default;
            return false;
        }

        float lowerT = startT;
        float upperT = 1f;

        for (int i = 0; i < BinarySearchIterations; i++)
        {
            float midT = (lowerT + upperT) * 0.5f;
            float midDelta = GetProjectileTimeDelta(
                targetTrajectory,
                midT,
                projectileOrigin,
                projectileSpeed);

            if (midDelta <= 0f)
            {
                upperT = midT;
            }
            else
            {
                lowerT = midT;
            }
        }

        interceptPoint = GetCubicPoint(targetTrajectory, upperT);
        return true;
    }

    private static float GetProjectileTimeDelta(
        BezierTrajectoryCubic trajectory,
        float trajectoryTime,
        Vector2 projectileOrigin,
        float projectileSpeed)
    {
        Vector2 point = GetCubicPoint(trajectory, trajectoryTime);
        return GetProjectileTimeDelta(point, trajectoryTime, projectileOrigin, projectileSpeed);
    }

    private static float GetProjectileTimeDelta(
        Vector2 trajectoryPoint,
        float trajectoryTime,
        Vector2 projectileOrigin,
        float projectileSpeed)
    {
        return Vector2.Distance(projectileOrigin, trajectoryPoint) / projectileSpeed - trajectoryTime;
    }

    private static float EstimateCubicLength(BezierTrajectoryCubic trajectory)
    {
        float length = 0f;
        Vector2 previousPoint = trajectory.start;

        for (int i = 1; i <= LengthEstimateSteps; i++)
        {
            float t = i / (float)LengthEstimateSteps;
            Vector2 point = GetCubicPoint(trajectory, t);
            length += Vector2.Distance(previousPoint, point);
            previousPoint = point;
        }

        return length;
    }

    private static Vector2 GetCubicPoint(BezierTrajectoryCubic trajectory, float t)
    {
        float inverseT = 1f - t;

        return
            inverseT * inverseT * inverseT * trajectory.start +
            3f * inverseT * inverseT * t * trajectory.control0 +
            3f * inverseT * t * t * trajectory.control1 +
            t * t * t * trajectory.end;
    }
}
