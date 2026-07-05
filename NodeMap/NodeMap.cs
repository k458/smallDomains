using System;
using System.Collections.Generic;

namespace NodeMap;

public class NodeMap : INodeMap
{
    private static readonly int[] expandYOffsets = { 0, -1, 1 };

    private readonly Dictionary<int, Dictionary<int, INode>> nodesByColumn = new();
    private readonly Func<NodeType, INode> nodeFactory;

    public NodeMap(Func<NodeType, INode>? nodeFactory = null)
    {
        this.nodeFactory = nodeFactory ?? CreateDefaultNode;
    }

    public bool TryAddNode(
        INode node,
        int x,
        int y)
    {
        Dictionary<int, INode> column = GetOrCreateColumn(x);

        if (column.ContainsKey(y))
        {
            return false;
        }

        column[y] = node;
        node.OnAfterAdded(x, y, this);
        return true;
    }

    public bool TryAddDependencyNodesToColumn(int x)
    {
        if (!nodesByColumn.TryGetValue(x, out Dictionary<int, INode>? column))
        {
            return false;
        }

        bool allAdded = true;
        KeyValuePair<int, INode>[] columnNodes = CopyColumnNodes(column);

        foreach (KeyValuePair<int, INode> nodeByY in columnNodes)
        {
            INode node = nodeByY.Value;

            foreach (DependencyNode dependencyNode in node.DependencyNodes)
            {
                int dependencyX = x + dependencyNode.DeltaX;
                int dependencyY = nodeByY.Key + dependencyNode.DeltaY;

                if (TryGetNode(dependencyX, dependencyY, out _))
                {
                    continue;
                }

                INode dependency = nodeFactory(dependencyNode.NodeType);

                if (!TryAddNode(dependency, dependencyX, dependencyY)
                    || !TryConnectNodesByPosition(
                        node,
                        x,
                        dependency,
                        dependencyX))
                {
                    allAdded = false;
                }
            }
        }

        return allAdded;
    }

    public bool TryAddConnectionsToPreviousNodesForColumn(int x)
    {
        if (!nodesByColumn.TryGetValue(x, out Dictionary<int, INode>? column)
            || !nodesByColumn.TryGetValue(x - 1, out Dictionary<int, INode>? previousColumn))
        {
            return false;
        }

        bool allConnectionTargetsReached = true;
        KeyValuePair<int, INode>[] columnNodes = CopyColumnNodes(column);

        foreach (KeyValuePair<int, INode> nodeByY in columnNodes)
        {
            INode node = nodeByY.Value;

            for (int deltaY = -1;
                deltaY <= 1 && node.PreviousNodes.Count < node.MaxPreviousConnections;
                deltaY++)
            {
                int previousY = nodeByY.Key + deltaY;

                if (!previousColumn.TryGetValue(previousY, out INode? previousNode))
                {
                    continue;
                }

                TryConnectNodes(previousNode, node);
            }

            if (node.PreviousNodes.Count < node.MaxPreviousConnections)
            {
                allConnectionTargetsReached = false;
            }
        }

        return allConnectionTargetsReached;
    }

    public bool Expand(int x)
    {
        if (!nodesByColumn.TryGetValue(x, out Dictionary<int, INode>? column))
        {
            return false;
        }

        bool allExpanded = true;
        KeyValuePair<int, INode>[] columnNodes = CopyColumnNodes(column);

        foreach (KeyValuePair<int, INode> nodeByY in columnNodes)
        {
            INode node = nodeByY.Value;

            if (node.MaxNextConnections <= 0
                || node.NextNodes.Count > 0)
            {
                continue;
            }

            if (!TryExpandNode(node, x, nodeByY.Key))
            {
                allExpanded = false;
            }
        }

        return allExpanded;
    }

    private bool TryGetNode(
        int x,
        int y,
        out INode? node)
    {
        node = null;

        return nodesByColumn.TryGetValue(x, out Dictionary<int, INode>? column)
            && column.TryGetValue(y, out node);
    }

    private Dictionary<int, INode> GetOrCreateColumn(int x)
    {
        if (!nodesByColumn.TryGetValue(x, out Dictionary<int, INode>? column))
        {
            column = new Dictionary<int, INode>();
            nodesByColumn[x] = column;
        }

        return column;
    }

    private bool TryExpandNode(
        INode node,
        int x,
        int y)
    {
        int nextX = x + 1;

        for (int i = 0; i < expandYOffsets.Length; i++)
        {
            int nextY = y + expandYOffsets[i];

            if (!TryGetNode(nextX, nextY, out INode? nextNode))
            {
                nextNode = nodeFactory(NodeType.Undefined);

                if (!TryAddNode(nextNode, nextX, nextY))
                {
                    continue;
                }
            }

            if (TryConnectNodes(node, nextNode!))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryConnectNodes(
        INode previousNode,
        INode nextNode)
    {
        if (previousNode.NextNodes.Contains(nextNode)
            || nextNode.PreviousNodes.Contains(previousNode)
            || previousNode.NextNodes.Count >= previousNode.MaxNextConnections
            || nextNode.PreviousNodes.Count >= nextNode.MaxPreviousConnections)
        {
            return false;
        }

        previousNode.NextNodes.Add(nextNode);
        nextNode.PreviousNodes.Add(previousNode);
        return true;
    }

    private static bool TryConnectNodesByPosition(
        INode node,
        int nodeX,
        INode dependency,
        int dependencyX)
    {
        if (dependencyX < nodeX)
        {
            return TryConnectNodes(dependency, node);
        }

        return TryConnectNodes(node, dependency);
    }

    private static KeyValuePair<int, INode>[] CopyColumnNodes(Dictionary<int, INode> column)
    {
        KeyValuePair<int, INode>[] nodes = new KeyValuePair<int, INode>[column.Count];
        int i = 0;

        foreach (KeyValuePair<int, INode> nodeByY in column)
        {
            nodes[i] = nodeByY;
            i++;
        }

        return nodes;
    }

    private static INode CreateDefaultNode(NodeType nodeType)
    {
        return new Node(
            nodeType,
            maxPreviousConnections: 1,
            maxNextConnections: 1);
    }
}
