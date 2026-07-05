using System.Collections.Generic;

namespace NodeMapTridentHandmade;

public class NodeMap : INodeMap
{
    public NodeMap()
    {
        INode initialNode = new Node(NodeType.Undefined, depth: 0, y: 1);
        List<INode?> initialLayer = new();

        for (int y = 0; y <= YMax; y++)
        {
            initialLayer.Add(null);
        }

        initialLayer[initialNode.Y] = initialNode;
        CurrentDepth.Add(initialNode);
        NodeLayers.Add(initialLayer);
    }

    public int YMax { get; set; } = 2;
    public List<INode> CurrentDepth { get; } = new();
    public List<List<INode?>> NodeLayers { get; } = new();
    public Dictionary<INode, List<INode>> ForwardConnections { get; } = new();
}
