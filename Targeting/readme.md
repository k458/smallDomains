# Targeting

Owns target scoring and weighted random target selection.

## Main Types

- `TargetScoringSystem` builds nullable scores by arena position.
- `TargetPickerSystem` picks an index from weighted nullable scores.
- `ITargetState` is the state boundary exposed by possible targets.
- `ITargetingBiases` is the bias boundary exposed by the attacker or attacker data.

## Scoring

`TargetScoringSystem` takes:

- hit chances by position
- target states by position
- targeting biases

If a hit chance is null, non-positive, or the target state is null, the resulting score for that position is null.

Otherwise, the hit chance is passed through `ITargetingBiases.ApplyTo` to produce the final score.

## Picking

`TargetPickerSystem` performs weighted random selection from final scores.

- `null` means unavailable.
- `0` or negative scores are ignored.
- positive scores participate in weighted random selection.
- returns `null` if no valid target exists.

## Boundary

The `Targeting` folder should stay independent from concrete pawn classes. Concrete pawn data can implement `ITargetState` and `ITargetingBiases` outside this folder.
