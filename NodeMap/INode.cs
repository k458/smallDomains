using System.Collections.Generic;

namespace NodeMap;

public interface INode
{
    NodeType NodeType { get; }
    int MaxPreviousConnections { get; }
    int MaxNextConnections { get; }
    IReadOnlyList<DependencyNode> DependencyNodes { get; }
    List<INode> PreviousNodes { get; }
    List<INode> NextNodes { get; }

    void OnAfterAdded(
        int x,
        int y,
        INodeMap parent);
}
