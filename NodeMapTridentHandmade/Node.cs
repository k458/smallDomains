namespace NodeMapTridentHandmade;

public class Node : INode
{
    public Node(
        NodeType nodeType,
        int depth,
        int y)
    {
        NodeType = nodeType;
        Depth = depth;
        Y = y;
    }

    public NodeType NodeType { get; }
    public int Depth { get; }
    public int Y { get; }
}
