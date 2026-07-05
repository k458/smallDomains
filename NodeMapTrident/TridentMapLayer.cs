using System;
using System.Collections.Generic;

namespace NodeMapTrident;

public class TridentMapLayer
{
    public TridentMapLayer(
        int depth,
        IReadOnlyList<TridentMapNode> nodes)
    {
        if (nodes.Count < 1 || nodes.Count > TridentLaneRules.MaxNodesPerDepth)
        {
            throw new ArgumentOutOfRangeException(
                nameof(nodes),
                nodes.Count,
                "A trident layer must contain 1 to 3 nodes.");
        }

        bool[] occupiedY = new bool[TridentLaneRules.MaxY + 1];

        foreach (TridentMapNode node in nodes)
        {
            if (node.Depth != depth)
            {
                throw new ArgumentException(
                    "Every node in a trident layer must match the layer depth.",
                    nameof(nodes));
            }

            if (occupiedY[node.Y])
            {
                throw new ArgumentException(
                    "Nodes cannot share the same Y in a trident layer.",
                    nameof(nodes));
            }

            occupiedY[node.Y] = true;
        }

        Depth = depth;
        Nodes = nodes;
    }

    public int Depth { get; }
    public IReadOnlyList<TridentMapNode> Nodes { get; }
}
