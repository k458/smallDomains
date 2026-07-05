using System.Numerics;

public static class BezierCurveControls
{
    public static float ControlDistance { get; set; } = 100f;
    public static float ControlDistanceCap { get; set; } = 0.5f;
    public static float ControlSecondRotationMult { get; set; } = 0.25f;

    public static void GetCubicControls(
        Vector2 start,
        Vector2 end,
        float initialRotation,
        out Vector2 control0,
        out Vector2 control1)
    {
        Vector2 delta = end - start;
        float controlDistance = MathF.Min(ControlDistance, delta.Length() * ControlDistanceCap);
        float angleToEnd = MathF.Atan2(delta.Y, delta.X);
        float angleDeltaInitial = NormalizeRadians(angleToEnd - initialRotation);

        Vector2 initialDirection = DirectionFromRadians(initialRotation);
        Vector2 secondDirection = DirectionFromRadians(
            initialRotation + angleDeltaInitial * ControlSecondRotationMult);

        control0 = start + initialDirection * controlDistance * 0.7f;
        control1 = start + secondDirection * controlDistance;
    }

    private static Vector2 DirectionFromRadians(float radians)
    {
        return new Vector2(MathF.Cos(radians), MathF.Sin(radians));
    }

    private static float NormalizeRadians(float radians)
    {
        const float fullTurn = MathF.PI * 2f;

        while (radians > MathF.PI)
        {
            radians -= fullTurn;
        }

        while (radians < -MathF.PI)
        {
            radians += fullTurn;
        }

        return radians;
    }
}
