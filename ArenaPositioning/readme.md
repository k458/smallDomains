# ArenaPositioning

Owns the pure rules for a one-dimensional arena line.

## Main Types

- `ArenaPositionLine` creates and manages a fixed number of rank positions.
- `ArenaPosition` represents one rank and may contain an occupant.
- `IArenaPositionOccupant` is the role required for anything that can occupy a position.
- `ArenaTeam` identifies the `Left` and `Right` teams.
- `ArenaPositionDirection` is used by push and pull operations.

## Behavior

The system can place and remove occupants, inspect positions by rank index, and move occupants with push/pull rules.

`GetFrontmostAlly(rankIndex, out ally)` follows the contiguous allied formation toward the opposing side. It returns the last ally's rank and provides the occupant through `ally`. It searches right for the left team and left for the right team. An empty starting rank returns `null` and outputs `null`; an isolated occupant returns its own rank and itself.

`GetOccupantsInRange` returns occupants from an origin rank toward a direction, constrained by minimum and maximum range. Empty ranks are returned as `null`, so the result preserves range slots instead of only returning occupied targets. `ArenaPositionDirection.All` checks both sides from the origin.

Team-aware push and pull methods use the occupant `Team` value to decide whether a chain can move through allied occupants. Untyped push ignores team and only needs an occupied target plus an empty destination somewhere in the push direction.

`PullFormation(targetRankIndex)` starts at the target and selects contiguous same-team occupants toward the team's pull direction. The left team selects and moves left, while the right team selects and moves right.

`PushFormation(targetRankIndex)` starts at the target and selects contiguous same-team occupants toward the team's push direction. The left team selects and moves right, while the right team selects and moves left.

Only the selected side moves, so targeting the middle of a larger formation can split it. Formation movement fails if the target rank is empty or if an enemy or arena boundary blocks the destination.

## Boundary

This system should not know about Godot nodes, animations, abilities, or turn flow. It only owns position data and position movement rules.
