# SquareTileGrid

Defines contracts and default implementations for a grid made of square tiles.

## Position

`System.Numerics.Vector2` stores floating-point coordinates, so this system uses
`SquareTileGridPosition` for integer tile coordinates.

## Main Types

- `ISquareTileGrid` exposes grid dimensions, tile storage, movement costs, and
  tile lookup.
- `ISquareTile` exposes tile position and parent grid.
- `SquareTileGrid` is the default grid implementation.
- `SquareTile` is the default tile implementation.
- `SquareTileAdjacencyType` describes how a tile can be considered adjacent:
  `Undefined`, `Near`, `Passable`, or `PassableByForce`.
- `IGridComposition` exposes a layered view of one or more grids in composition
  coordinates.
- `GridComposition` is the default composition implementation.
- `SquareTileGridPathfindingService` finds paths across a grid composition.

## Tile Storage

`ISquareTileGrid.TilesByPosition` is a get-only jagged array indexed as
`[x][y]`. A jagged array is used instead of a rectangular multidimensional array
because it keeps indexing simple and is generally the faster array shape in C#.

`SquareTileGrid` takes `sizeX` and `sizeY` in its constructor, creates all
tiles, and attaches them to the parent grid. `TryGetTile` returns a tile by
`SquareTileGridPosition` using the try pattern.

## Grid Composition

`GridComposition` stores grid layers in a dictionary keyed by layer index.
Negative layer indices are allowed. The base grid should use index `0`.

Each layer stores an `ISquareTileGrid` and the position of that grid's local
`0,0` tile in composition coordinates. Adding or removing a layer triggers
partial recomposition for the affected layer area and a 1-tile border around
it. `Recompose()` is still available for an explicit full rebuild.

When multiple layers cover the same composition position, the layer with the
highest index wins. Recomposition rebuilds lookup tables from composition
positions to visible tiles and from visible tiles back to composition positions.

`TryGetAdjacentTile` resolves neighbors from a cache built during recomposition.
The cache is stored by visible tile, then by adjacency type, then by one of the
eight neighbor slots. Grids and tiles do not own adjacency data directly; they
are intended to be used through a composition. The default composition returns
visible neighboring tiles for `Near`, `Passable`, and `PassableByForce`.
`Undefined` returns no neighbors.

Neighbor cache arrays are pooled and reused internally because they are only
used by the composition system.

## Pathfinding

`SquareTileGridPathfindingService.TryFindPath` takes a grid composition, start
position, end position, adjacency type, maximum path cost, and a caller-owned
`List<ISquareTile>` to fill with the result.

`SquareTileGridPathfindingService` clears the provided path list before each
search. The caller owns that list. The service only caches its temporary search
collections.

`PathCost` and `DiagonalPathCost` are settable so implementations can tune
orthogonal and diagonal movement. Implementations should initialize them from
`ISquareTileGrid.DefaultPathCost` and
`ISquareTileGrid.DefaultDiagonalPathCost`, which are `2` and `3`.

## Tile Neighbors

Each tile exposes its `Position` and `ParentGrid`, but neighbor lookup belongs
to `IGridComposition`. This keeps overlapping grids from mutating each other's
tiles or caching stale cross-grid references. `GridComposition` caches neighbors
as a jagged array shaped like `[adjacencyType][neighborIndex]`.
