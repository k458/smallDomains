# Bezier Curve

This folder contains cubic Bezier curve configuration, trajectory data, and
projectile lead prediction utilities.

## Curve Controls

`BezierCurveControls` calculates two control points for a cubic Bezier curve.

The curve is built from:

```csharp
start -> control0 -> control1 -> end
```

Both generated control points are placed relative to `start`. This makes the
curve leave the start point in the supplied initial rotation, then bend toward
the direction from `start` to `end`.

### Settings

```csharp
public static float ControlDistance { get; set; } = 100f;
public static float ControlDistanceCap { get; set; } = 0.5f;
public static float ControlSecondRotationMult { get; set; } = 0.25f;
```

`ControlDistance` is the preferred distance used to place the control points.

`ControlDistanceCap` limits the temporary control distance based on the distance
from `start` to `end`.

`ControlSecondRotationMult` controls how strongly `control1` rotates from the
initial rotation toward the direction from `start` to `end`.

### Method

```csharp
BezierCurveControls.GetCubicControls(
    start,
    end,
    initialRotation,
    out Vector2 control0,
    out Vector2 control1);
```

`initialRotation` is expected to be in radians.

The method first calculates:

```csharp
delta = end - start;
controlDistance = Min(ControlDistance, delta.Length() * ControlDistanceCap);
angleToEnd = Atan2(delta.Y, delta.X);
angleDeltaInitial = angleToEnd - initialRotation;
```

Then it places:

```csharp
control0 = start + initialDirection * controlDistance * 0.7f;
control1 = start + secondDirection * controlDistance;
```

`secondDirection` starts from `initialRotation` and rotates toward `angleToEnd`
by `angleDeltaInitial * ControlSecondRotationMult`.

### Example

For:

```text
start = (100, 200)
end = (500, 200)
initialRotation = -PI / 2
```

The result with the default settings is approximately:

```text
control0 = (100, 130)
control1 = (138.268, 107.612)
```

### Note

`ControlDistance` and `ControlSecondRotationMult` can depend on the initial
angle delta and the initial distance from `start` to `end`.

## Cubic Bezier Lead Prediction

`BezierTrajectoryCubic` stores a cubic Bezier trajectory:

```csharp
public Vector2 start;
public Vector2 end;
public Vector2 control0;
public Vector2 control1;
public float initialAngle;
```

The curve is evaluated as normalized trajectory time from `0` to `1`.

### Intercept Prediction

`CubicBezierLeadPredictor` predicts where a projectile should aim to intercept
a target moving along a cubic Bezier trajectory.

```csharp
public bool TryGetInterceptPoint(
    BezierTrajectoryCubic targetTrajectory,
    float minimumTrajectoryTime,
    Vector2 projectileOrigin,
    float projectileSpeed,
    out Vector2 interceptPoint);
```

`minimumTrajectoryTime` skips the beginning of the trajectory. It is clamped to
the `0..1` range.

`projectileOrigin` is the point the projectile starts from.

`projectileSpeed` is the projectile movement speed. If it is `0` or lower, the
method returns `false`.

`interceptPoint` receives the predicted point when the method returns `true`.

### Two-Stage Sampling

`TryGetInterceptPoint` uses distance-based sampling so short and long curves do
not use the same fixed number of samples.

```csharp
private const float ForwardSampleDistance = 100f;
private const float BackwardSampleDistance = 10f;
private const int LengthEstimateSteps = 32;
```

The method:

1. Estimates the cubic curve length.
2. Converts `100f` world units into a normalized forward trajectory step.
3. Samples forward until it finds a reachable intercept point.
4. Converts `10f` world units into a normalized backward trajectory step.
5. Samples backward from the reachable point.
6. Returns the latest reachable point found by the backward pass, which is the
   refined point closest to the start of the reachable region.

Reachability is tested by comparing projectile travel time to trajectory time:

```csharp
projectileTravelTime = distance(projectileOrigin, trajectoryPoint) / projectileSpeed;
delta = projectileTravelTime - trajectoryTime;
```

If `delta <= 0`, the projectile can reach that trajectory point in time.

### Binary Intercept Prediction

`TryGetInterceptPointBinary` uses binary search between `minimumTrajectoryTime`
and `1`.

```csharp
public bool TryGetInterceptPointBinary(
    BezierTrajectoryCubic targetTrajectory,
    float minimumTrajectoryTime,
    Vector2 projectileOrigin,
    float projectileSpeed,
    out Vector2 interceptPoint);
```

This is cheaper when the reachable state changes only once across the interval.
For curves that loop or move closer and farther in complex ways, the two-stage
sampling method is more robust.
