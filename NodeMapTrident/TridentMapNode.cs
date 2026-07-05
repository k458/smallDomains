using System.Collections.Generic;

namespace NodeMapTrident;

public class TridentMapNode
{
    private readonly List<TridentMapNode> incomingNodes = new();
    private readonly List<TridentMapNode> outgoingNodes = new();

    public TridentMapNode(
        int depth,
        int y)
    {
        TridentLaneRules.ValidateY(y);

        Depth = depth;
        Y = y;
        Lane = TridentLaneRules.GetLane(y);
        LaneModifiers = TridentLaneRules.GetLaneModifiers(y);
    }

    public int Depth { get; }
    public int Y { get; }
    public TridentLane Lane { get; }
    public TridentLaneModifiers LaneModifiers { get; }
    public IReadOnlyList<TridentMapNode> IncomingNodes => incomingNodes;
    public IReadOnlyList<TridentMapNode> OutgoingNodes => outgoingNodes;

    internal bool TryConnectTo(TridentMapNode nextNode)
    {
        if (nextNode.Depth != Depth + 1
            || !TridentLaneRules.CanConnect(Y, nextNode.Y)
            || outgoingNodes.Contains(nextNode)
            || outgoingNodes.Count >= TridentLaneRules.MaxOutgoingConnections)
        {
            return false;
        }

        outgoingNodes.Add(nextNode);
        nextNode.incomingNodes.Add(this);
        return true;
    }
}
