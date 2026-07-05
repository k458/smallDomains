# NodeMap

Defines contracts for a coordinate-based node map.

## Main Types

- `NodeType` identifies what kind of node should be created. It currently only
  has `Undefined`.
- `DependencyNode` describes a node dependency by node type and relative `x`,
  `y` deltas.
- `INode` exposes connection limits, dependency nodes, previous connections,
  next connections, and receives an add callback.
- `INodeMap` owns node placement, dependency expansion, and connection
  generation.
- `Node` is the default node implementation.
- `NodeMap` is the default map implementation.

## Behavior

`TryAddNode` takes an `INode` and integer `x`, `y` coordinates. It returns
whether the node was added.

`TryAddDependencyNodesToColumn` scans nodes in the given column, creates missing
dependency nodes from their `DependencyNodes`, places them at the relative
coordinates, and connects each head node to its dependency node.

`TryAddConnectionsToPreviousNodesForColumn` tries to fill each node's previous
connections from nodes in column `x - 1` where the `y` delta is no more than
`1`. It also respects the previous node's maximum next connections.

`Expand` scans column `x` for nodes that can have next connections but currently
have none. For each such node, it adds or reuses a node in column `x + 1` and
connects to it. It tries `y`, then `y - 1`, then `y + 1`.

`MaxPreviousConnections` limits how many incoming or previous connections a node
can have.

`MaxNextConnections` limits how many outgoing or next connections a node can
have.

`OnAfterAdded` is called after a node is added to an `INodeMap`. It receives the
node's `x`, `y` coordinates and parent map.
