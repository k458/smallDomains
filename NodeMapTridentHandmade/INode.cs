namespace NodeMapTridentHandmade;

public interface INode
{
    NodeType NodeType { get; }
    int Depth { get; }
    int Y { get; }
}
