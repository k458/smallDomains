namespace NodeMap;

public readonly struct DependencyNode
{
    public DependencyNode(
        NodeType nodeType,
        int deltaX,
        int deltaY)
    {
        NodeType = nodeType;
        DeltaX = deltaX;
        DeltaY = deltaY;
    }

    public NodeType NodeType { get; }
    public int DeltaX { get; }
    public int DeltaY { get; }
}
