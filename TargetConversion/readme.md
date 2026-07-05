# TargetConversion

Adapts arena-position data into targeting data.

## Main Types

- `TargetStateConverter` is a stateless static converter from arena occupants to target states.

## Behavior

`ConvertOccupantsToTargetStates` takes a list of `IArenaPositionOccupant?` values and returns a list of `ITargetState?` values with the same slot count.

- Empty arena ranks stay `null`.
- Occupied ranks are directly cast to `ITargetState`.
- If an occupied rank does not implement `ITargetState`, the cast throws. That is intentional because occupied combat pawns are expected to be targetable.

## Boundary

Arena positioning owns where occupants are. Targeting owns scoring and picking. This converter is the explicit bridge between those systems.
