# SpatialState

`SpatialState` is meant to hold spatial truth for the simulation.

It should answer questions such as:

- which entities are on which tiles
- where an entity is positioned
- what spatial data other services can query

It should not know about stockpile rules, pawn AI, jobs, or UI.

## Services

`StockpileService` is expected to manage stockpile-specific rules. It may query
`SpatialState` to inspect items or entities on tiles, then publish work requests
to the blackboard.

`Blackboard` is the shared job/request board. Domain services write needs to it,
and pawn/AI systems inspect it to choose work.

## Dependency Direction

The intended direction is one-way:

```text
StockpileService -> SpatialState
StockpileService -> Blackboard
Pawn AI -> Blackboard
Pawn AI -> SpatialState
```

`SpatialState` should stay low-level and should not depend on stockpiles,
blackboard jobs, or pawn behavior.
