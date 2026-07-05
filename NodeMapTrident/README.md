# NodeMapTrident

Forward-only node map generation for a compact Void expedition route.

## Structure

- The map advances one depth at a time.
- Each depth has 1 to 3 nodes.
- The only lane positions are `Y = 0`, `Y = 1`, and `Y = 2`.
- Nodes in the same depth cannot share a `Y` position.
- Connections only move from the current depth to the next depth.
- A connection is valid when `abs(current.Y - next.Y) <= 1`.

## Lane Identity

- `Y = 0` is the upper route: more military, combat, and rare faction events.
- `Y = 1` is the central route: safer, more stable, and more flexible.
- `Y = 2` is the lower route: more salvage, pirates, anomalies, and reward
  variance.

## Main Types

- `TridentNodeMapGenerator` creates the initial layer and expands from one
  layer to the next.
- `TridentMapLayer` owns the nodes at one depth.
- `TridentMapNode` exposes depth, lane position, lane modifiers, incoming
  nodes, and outgoing nodes.
- `TridentConnectionWeights` controls whether a node prefers 1, 2, or 3
  outgoing connections.
- `TridentLaneRules` contains lane limits, lane modifiers, and connection
  validation.

## Generation Pipeline

`GenerateNextLayer` calculates valid future lane positions from the current
nodes, creates 1 to 3 unique next-depth nodes from those positions, performs a
forward connection pass, then performs a reverse validation pass.

The reverse validation pass does not create backward movement. It only adds a
missing forward edge from a valid current node so every next node is reachable.

Old layers can be discarded after the player moves forward. The next layer
retains incoming references for validation and inspection, but traversal remains
forward-only through `OutgoingNodes`.
