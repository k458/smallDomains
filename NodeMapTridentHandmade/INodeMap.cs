using System.Collections.Generic;

namespace NodeMapTridentHandmade;

public interface INodeMap
{
    int YMax { get; set; }
    List<INode> CurrentDepth { get; }
    List<List<INode?>> NodeLayers { get; }
    Dictionary<INode, List<INode>> ForwardConnections { get; }
}
