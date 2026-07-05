# FlowControl

Defines contracts for pausing gameplay flow while scene-side work is still active.

## Main Types

- `IFlowBlocker` represents something that can block flow while it is active.
- `IFlowController` tracks blockers with `AddFlowBlocker` and `RemoveFlowBlocker`.

## Intended Use

A pawn or child node can receive an `IFlowController` reference and register itself or another object while an animation, effect, prompt, or other blocking operation is active.

The scene controller can then avoid advancing gameplay systems while blockers remain.

## Boundary

This system intentionally does not define how blockers are stored or when the scene resumes. That belongs to the concrete scene controller.
