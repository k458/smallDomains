using System.Collections.Generic;

namespace NodeMap;

public class Node : INode
{
    public Node(
        NodeType nodeType,
        int maxPreviousConnections,
        int maxNextConnections,
        IReadOnlyList<DependencyNode>? dependencyNodes = null)
    {
        NodeType = nodeType;
        MaxPreviousConnections = maxPreviousConnections;
        MaxNextConnections = maxNextConnections;
        DependencyNodes = dependencyNodes ?? System.Array.Empty<DependencyNode>();
    }

    public NodeType NodeType { get; }
    public int MaxPreviousConnections { get; }
    public int MaxNextConnections { get; }
    public IReadOnlyList<DependencyNode> DependencyNodes { get; }
    public List<INode> PreviousNodes { get; } = new();
    public List<INode> NextNodes { get; } = new();

    public virtual void OnAfterAdded(
        int x,
        int y,
        INodeMap parent)
    {
        _ = x;
        _ = y;
        _ = parent;
    }
}
