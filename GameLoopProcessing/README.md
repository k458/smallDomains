# GameLoopProcessing

Coordinates gameplay objects that need fixed-tick processing, per-frame
processing, or both.

## Main Types

- `IFixedRateProcessable` marks an object that should process on fixed
  simulation ticks with `ProcessFixedTick()`.
- `IVariableRateProcessable` marks an object that should process once per frame
  with `ProcessFrame(float deltaTime, float tickProgress)`.
- `IProcessingController` owns read-only views of registered fixed-rate and
  variable-rate processables, exposes add and remove methods for both groups,
  and advances processing.
- `ProcessingController` is the default controller implementation.

## Behavior

`TickTime` is the duration of one fixed simulation tick. `TickProgress` reports
how far the controller is between the previous fixed tick and the next fixed
tick.

`AutomaticFixedTick` controls whether `ProcessFrame` triggers fixed ticks
automatically as enough frame time accumulates. When it is enabled,
`ProcessFrame(deltaTime)` advances the internal clock, runs
`ProcessFixedTick()` once for each completed fixed tick, then calls
`ProcessFrame(deltaTime, tickProgress)` on each variable-rate processable.

`TriggerFixedTick()` manually runs one fixed tick and resets tick progress. This
allows an external clock or engine loop to own fixed simulation timing while the
controller still provides variable-rate interpolation progress.

The controller implementation calculates tick progress from the previous tick
time and current tick time, then passes that progress to variable-rate
processables so they can interpolate or smooth between fixed simulation ticks.

Duplicate processables are rejected by add methods. Removing a processable
returns whether it was present.
