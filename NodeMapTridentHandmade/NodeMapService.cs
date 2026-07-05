using System;
using System.Collections.Generic;

namespace NodeMapTridentHandmade;

public class NodeMapService : INodeMapService
{
    private readonly Random random;

    public NodeMapService(Random? random = null)
    {
        this.random = random ?? new Random();
    }

    public IReadOnlyList<INode> GenerateNextDepth(INodeMap nodeMap)
    {
        List<INode> nextDepth = AddDepthLayer(nodeMap);

        ConnectToLast(nodeMap, nextDepth);
        ConnectFromLast(nodeMap, nextDepth);

        return nextDepth;
    }

    public IReadOnlyList<INode> Grow(INodeMap nodeMap)
    {
        IReadOnlyList<INode> nextDepth = GenerateNextDepth(nodeMap);
        MoveToDepth(nodeMap, nextDepth);
        return nodeMap.CurrentDepth;
    }

    public IReadOnlyList<INode> GrowLooped(INodeMap nodeMap)
    {
        List<INode> nextDepth = AddDepthLayerLooped(nodeMap);

        ConnectToLast(nodeMap, nextDepth);
        ConnectFromLast(nodeMap, nextDepth);
        MoveToDepth(nodeMap, nextDepth);

        return nodeMap.CurrentDepth;
    }

    public IReadOnlyList<INode> GrowRandom(INodeMap nodeMap)
    {
        List<INode> nextDepth = AddDepthLayerRandom(nodeMap);

        MoveToDepth(nodeMap, nextDepth);

        return nodeMap.CurrentDepth;
    }

    public IReadOnlyList<INode> GrowRandomNoCross(INodeMap nodeMap)
    {
        List<INode> nextDepth = AddDepthLayerRandomNoCross(nodeMap);

        MoveToDepth(nodeMap, nextDepth);

        return nodeMap.CurrentDepth;
    }

    public IReadOnlyList<INode> GrowRandomNoCrossV2(INodeMap nodeMap)
    {
        Dictionary<int, int> possibleYPositions = new();

        foreach (INode node in nodeMap.CurrentDepth)
        {
            if (!possibleYPositions.TryAdd(node.Y, 1))
            {
                possibleYPositions[node.Y]++;
            }
        }

        HashSet<int> possibleNodeCounts = new();

        _ = possibleYPositions;
        _ = possibleNodeCounts;

        return nodeMap.CurrentDepth;
    }

    public IReadOnlyList<INode> GrowByBranches(INodeMap nodeMap)
    {
        List<INode> nextDepth = AddDepthLayerByBranches(nodeMap);

        MoveToDepth(nodeMap, nextDepth);

        return nodeMap.CurrentDepth;
    }

    public void MoveToDepth(
        INodeMap nodeMap,
        IReadOnlyList<INode> nextDepth)
    {
        List<INode> nextDepthLayer = new(nextDepth);

        nodeMap.CurrentDepth.Clear();
        nodeMap.CurrentDepth.AddRange(nextDepthLayer);
        nodeMap.NodeLayers.Add(CreateNodeSlotLayer(nodeMap, nextDepthLayer));
    }

    public void SetNodeSlot(
        INodeMap nodeMap,
        int depth,
        int y,
        INode node)
    {
        ValidateNodeSlot(nodeMap, depth, y, node);
        EnsureNodeLayerExists(nodeMap, depth);

        List<INode?> nodeLayer = nodeMap.NodeLayers[depth];
        INode? previousNode = nodeLayer[y];

        if (previousNode is not null)
        {
            ReplaceNodeConnections(nodeMap, previousNode, node);
        }

        nodeLayer[y] = node;

        if (depth == nodeMap.NodeLayers.Count - 1)
        {
            RebuildCurrentDepth(nodeMap, nodeLayer);
        }
    }

    private List<INode> AddDepthLayer(INodeMap nodeMap)
    {
        List<int> lastYPositions = new();

        foreach (INode node in nodeMap.CurrentDepth)
        {
            lastYPositions.Add(node.Y);
        }

        int rolledNodeCount = RollNodeCount();
        List<int> possibleYPositions = GetPossibleYPositions(
            nodeMap,
            lastYPositions);
        int nodesToAdd = Math.Min(rolledNodeCount, possibleYPositions.Count);
        List<int> selectedYPositions = SelectNextYPositions(
            lastYPositions,
            possibleYPositions,
            nodesToAdd);

        return CreateDepthLayer(nodeMap, selectedYPositions);
    }

    private List<INode> AddDepthLayerLooped(INodeMap nodeMap)
    {
        List<int> lastYPositions = new();
        Dictionary<int, int> yPositionWeights = new();

        foreach (INode node in nodeMap.CurrentDepth)
        {
            lastYPositions.Add(node.Y);
            AddYPositionWeight(yPositionWeights, node.Y - 1);
            AddYPositionWeight(yPositionWeights, node.Y);
            AddYPositionWeight(yPositionWeights, node.Y + 1);
        }

        int rolledNodeCount = RollNodeCount();
        List<int> possibleYPositions = GetPossibleYPositions(
            nodeMap,
            yPositionWeights);
        int nodesToAdd = Math.Min(rolledNodeCount, possibleYPositions.Count);
        List<int> selectedYPositions = SelectNextYPositions(
            lastYPositions,
            possibleYPositions,
            yPositionWeights,
            nodesToAdd);

        return CreateDepthLayer(nodeMap, selectedYPositions);
    }

    private List<INode> AddDepthLayerRandom(INodeMap nodeMap)
    {
        List<int> lastYPositions = new();

        foreach (INode node in nodeMap.CurrentDepth)
        {
            lastYPositions.Add(node.Y);
        }

        List<int> possibleYPositions = GetPossibleYPositions(
            nodeMap,
            lastYPositions);
        List<INode> nextDepth = new();
        int nextDepthIndex = nodeMap.CurrentDepth.Count == 0
            ? 0
            : nodeMap.CurrentDepth[0].Depth + 1;

        while (possibleYPositions.Count > 0)
        {
            int y = PopRandomY(possibleYPositions);
            INode nextNode = new Node(
                NodeType.Undefined,
                nextDepthIndex,
                y);

            nextDepth.Add(nextNode);
            ConnectUnconnectedLastNodesToAddedNode(
                nodeMap,
                nextDepth,
                nextNode);

            if (AllLastNodesConnectToNextDepth(nodeMap, nextDepth))
            {
                break;
            }
        }

        TryAddRandomExtraNode(
            nodeMap,
            possibleYPositions,
            nextDepth,
            nextDepthIndex);

        return nextDepth;
    }

    private List<INode> AddDepthLayerRandomNoCross(INodeMap nodeMap)
    {
        List<int> lastYPositions = new();

        foreach (INode node in nodeMap.CurrentDepth)
        {
            lastYPositions.Add(node.Y);
        }

        List<int> possibleYPositions = GetPossibleYPositions(
            nodeMap,
            lastYPositions);
        List<INode> nextDepth = new();
        int rolledNodeCount = Math.Min(
            RollNodeCount(),
            possibleYPositions.Count);
        int nextDepthIndex = nodeMap.CurrentDepth.Count == 0
            ? 0
            : nodeMap.CurrentDepth[0].Depth + 1;

        if (rolledNodeCount == 1)
        {
            RemoveUncoveringSingleNodeYPositions(
                nodeMap,
                possibleYPositions);
        }

        while (possibleYPositions.Count > 0
            && nextDepth.Count < rolledNodeCount)
        {
            INode nextNode = new Node(
                NodeType.Undefined,
                nextDepthIndex,
                PopRandomY(possibleYPositions));
            INode? lastNode = GetMatchingLastNode(nodeMap, nextNode)
                ?? GetNearestLastNode(nodeMap, nextNode, nextDepth);

            nextDepth.Add(nextNode);

            if (lastNode is not null)
            {
                Connect(nodeMap, lastNode, nextNode);
            }

            if (AllLastNodesConnectToNextDepth(nodeMap, nextDepth))
            {
                break;
            }
        }

        TryAddRandomExtraNodeNoCross(
            nodeMap,
            possibleYPositions,
            nextDepth,
            nextDepthIndex);
        TryAddRandomForwardConnectionsNoCross(nodeMap, nextDepth);

        return nextDepth;
    }

    private static void RemoveUncoveringSingleNodeYPositions(
        INodeMap nodeMap,
        List<int> possibleYPositions)
    {
        for (int i = possibleYPositions.Count - 1; i >= 0; i--)
        {
            int y = possibleYPositions[i];

            if (!CanSingleYCoverLastDepth(nodeMap, y))
            {
                possibleYPositions.RemoveAt(i);
            }
        }
    }

    private static bool CanSingleYCoverLastDepth(
        INodeMap nodeMap,
        int y)
    {
        foreach (INode lastNode in nodeMap.CurrentDepth)
        {
            if (Math.Abs(lastNode.Y - y) > 1)
            {
                return false;
            }
        }

        return true;
    }

    private List<INode> AddDepthLayerByBranches(INodeMap nodeMap)
    {
        List<INode> nextDepth = new();
        List<INode> lastNodes = new(nodeMap.CurrentDepth);
        int nextDepthIndex = nodeMap.CurrentDepth.Count == 0
            ? 0
            : nodeMap.CurrentDepth[0].Depth + 1;

        Shuffle(lastNodes);

        foreach (INode lastNode in lastNodes)
        {
            TryExtendBranch(
                nodeMap,
                nextDepth,
                nextDepthIndex,
                lastNode);
        }

        TryAddExtraBranchNode(
            nodeMap,
            nextDepth,
            nextDepthIndex);

        return nextDepth;
    }

    private void TryExtendBranch(
        INodeMap nodeMap,
        List<INode> nextDepth,
        int nextDepthIndex,
        INode lastNode)
    {
        List<int> candidateYPositions = GetBranchYPositions(nodeMap, lastNode);

        Shuffle(candidateYPositions);

        foreach (int y in candidateYPositions)
        {
            INode nextNode = GetOrCreateNextDepthNode(
                nextDepth,
                nextDepthIndex,
                y);

            if (!WouldCrossForwardConnection(nodeMap, lastNode, nextNode, nextDepth))
            {
                Connect(nodeMap, lastNode, nextNode);
                return;
            }

            RemoveNextDepthNodeIfUnused(
                nodeMap,
                nextDepth,
                nextNode);
        }
    }

    private void TryAddExtraBranchNode(
        INodeMap nodeMap,
        List<INode> nextDepth,
        int nextDepthIndex)
    {
        if (random.Next(4) != 0)
        {
            return;
        }

        List<int> availableYPositions = GetUnusedNextDepthYPositions(
            nodeMap,
            nextDepth);

        if (availableYPositions.Count == 0)
        {
            return;
        }

        INode nextNode = new Node(
            NodeType.Undefined,
            nextDepthIndex,
            availableYPositions[random.Next(availableYPositions.Count)]);
        List<INode> candidates = GetExtraBranchConnectionCandidates(
            nodeMap,
            nextDepth,
            nextNode);

        if (candidates.Count == 0)
        {
            return;
        }

        nextDepth.Add(nextNode);
        Shuffle(candidates);

        int connectionCount = Math.Min(
            random.Next(1, 4),
            candidates.Count);

        for (int i = 0; i < candidates.Count && connectionCount > 0; i++)
        {
            INode lastNode = candidates[i];

            if (!WouldCrossForwardConnection(nodeMap, lastNode, nextNode, nextDepth))
            {
                Connect(nodeMap, lastNode, nextNode);
                connectionCount--;
            }
        }

        RemoveNextDepthNodeIfUnused(
            nodeMap,
            nextDepth,
            nextNode);
    }

    private static List<int> GetBranchYPositions(
        INodeMap nodeMap,
        INode lastNode)
    {
        List<int> candidateYPositions = new();

        for (int y = lastNode.Y - 1; y <= lastNode.Y + 1; y++)
        {
            if (y >= 0 && y <= nodeMap.YMax)
            {
                candidateYPositions.Add(y);
            }
        }

        return candidateYPositions;
    }

    private static INode GetOrCreateNextDepthNode(
        List<INode> nextDepth,
        int nextDepthIndex,
        int y)
    {
        foreach (INode node in nextDepth)
        {
            if (node.Y == y)
            {
                return node;
            }
        }

        INode nextNode = new Node(
            NodeType.Undefined,
            nextDepthIndex,
            y);

        nextDepth.Add(nextNode);
        return nextNode;
    }

    private static void RemoveNextDepthNodeIfUnused(
        INodeMap nodeMap,
        List<INode> nextDepth,
        INode nextNode)
    {
        foreach (List<INode> connectedNodes in nodeMap.ForwardConnections.Values)
        {
            if (connectedNodes.Contains(nextNode))
            {
                return;
            }
        }

        nextDepth.Remove(nextNode);
    }

    private static List<int> GetUnusedNextDepthYPositions(
        INodeMap nodeMap,
        List<INode> nextDepth)
    {
        List<int> yPositions = new();

        for (int y = 0; y <= nodeMap.YMax; y++)
        {
            bool isUsed = false;

            foreach (INode nextNode in nextDepth)
            {
                if (nextNode.Y == y)
                {
                    isUsed = true;
                    break;
                }
            }

            if (!isUsed)
            {
                yPositions.Add(y);
            }
        }

        return yPositions;
    }

    private static List<INode> GetExtraBranchConnectionCandidates(
        INodeMap nodeMap,
        IReadOnlyList<INode> nextDepth,
        INode nextNode)
    {
        List<INode> candidates = new();

        foreach (INode lastNode in nodeMap.CurrentDepth)
        {
            if (CanConnect(lastNode, nextNode)
                && !WouldCrossForwardConnection(nodeMap, lastNode, nextNode, nextDepth))
            {
                candidates.Add(lastNode);
            }
        }

        return candidates;
    }

    private void Shuffle<T>(List<T> values)
    {
        for (int i = values.Count - 1; i > 0; i--)
        {
            int swapIndex = random.Next(i + 1);
            (values[i], values[swapIndex]) = (values[swapIndex], values[i]);
        }
    }

    private static List<INode> CreateDepthLayer(
        INodeMap nodeMap,
        IReadOnlyList<int> selectedYPositions)
    {
        List<INode> nextDepth = new();
        int nextDepthIndex = nodeMap.CurrentDepth.Count == 0
            ? 0
            : nodeMap.CurrentDepth[0].Depth + 1;

        foreach (int selectedY in selectedYPositions)
        {
            nextDepth.Add(new Node(
                NodeType.Undefined,
                nextDepthIndex,
                selectedY));
        }

        return nextDepth;
    }

    private static List<INode?> CreateNodeSlotLayer(
        INodeMap nodeMap,
        IReadOnlyList<INode> nodes)
    {
        List<INode?> nodeLayer = new();

        for (int y = 0; y <= nodeMap.YMax; y++)
        {
            nodeLayer.Add(null);
        }

        foreach (INode node in nodes)
        {
            nodeLayer[node.Y] = node;
        }

        return nodeLayer;
    }

    private static void ValidateNodeSlot(
        INodeMap nodeMap,
        int depth,
        int y,
        INode node)
    {
        if (depth < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(depth),
                depth,
                "Node depth cannot be negative.");
        }

        if (y < 0 || y > nodeMap.YMax)
        {
            throw new ArgumentOutOfRangeException(
                nameof(y),
                y,
                "Node Y is outside the map lane range.");
        }

        if (node.Depth != depth)
        {
            throw new ArgumentException(
                "Node depth must match the target slot depth.",
                nameof(node));
        }

        if (node.Y != y)
        {
            throw new ArgumentException(
                "Node Y must match the target slot Y.",
                nameof(node));
        }
    }

    private static void EnsureNodeLayerExists(
        INodeMap nodeMap,
        int depth)
    {
        while (nodeMap.NodeLayers.Count <= depth)
        {
            nodeMap.NodeLayers.Add(CreateEmptyNodeSlotLayer(nodeMap));
        }

        EnsureNodeLayerWidth(nodeMap, nodeMap.NodeLayers[depth]);
    }

    private static List<INode?> CreateEmptyNodeSlotLayer(INodeMap nodeMap)
    {
        List<INode?> nodeLayer = new();

        for (int y = 0; y <= nodeMap.YMax; y++)
        {
            nodeLayer.Add(null);
        }

        return nodeLayer;
    }

    private static void EnsureNodeLayerWidth(
        INodeMap nodeMap,
        List<INode?> nodeLayer)
    {
        while (nodeLayer.Count <= nodeMap.YMax)
        {
            nodeLayer.Add(null);
        }
    }

    private static void RebuildCurrentDepth(
        INodeMap nodeMap,
        List<INode?> nodeLayer)
    {
        nodeMap.CurrentDepth.Clear();

        foreach (INode? node in nodeLayer)
        {
            if (node is not null)
            {
                nodeMap.CurrentDepth.Add(node);
            }
        }
    }

    private static void ReplaceNodeConnections(
        INodeMap nodeMap,
        INode previousNode,
        INode replacementNode)
    {
        if (ReferenceEquals(previousNode, replacementNode))
        {
            return;
        }

        if (nodeMap.ForwardConnections.Remove(previousNode, out List<INode>? nextNodes))
        {
            nodeMap.ForwardConnections[replacementNode] = nextNodes;
        }

        foreach (List<INode> connectedNodes in nodeMap.ForwardConnections.Values)
        {
            for (int i = 0; i < connectedNodes.Count; i++)
            {
                if (ReferenceEquals(connectedNodes[i], previousNode))
                {
                    connectedNodes[i] = replacementNode;
                }
            }
        }
    }

    private void ConnectToLast(
        INodeMap nodeMap,
        IReadOnlyList<INode> nextDepth)
    {
        foreach (INode lastNode in nodeMap.CurrentDepth)
        {
            List<INode> validNextNodes = GetValidNextNodes(lastNode, nextDepth);

            if (validNextNodes.Count == 0)
            {
                continue;
            }

            Connect(
                nodeMap,
                lastNode,
                validNextNodes[random.Next(validNextNodes.Count)]);
        }
    }

    private void ConnectFromLast(
        INodeMap nodeMap,
        IReadOnlyList<INode> nextDepth)
    {
        foreach (INode nextNode in nextDepth)
        {
            if (HasIncomingFromLast(nodeMap, nextNode))
            {
                continue;
            }

            List<INode> validLastNodes = GetValidLastNodes(nodeMap, nextNode);

            if (validLastNodes.Count == 0)
            {
                continue;
            }

            Connect(
                nodeMap,
                validLastNodes[random.Next(validLastNodes.Count)],
                nextNode);
        }
    }

    private int PopRandomY(List<int> possibleYPositions)
    {
        int index = random.Next(possibleYPositions.Count);
        int y = possibleYPositions[index];
        possibleYPositions.RemoveAt(index);
        return y;
    }

    private void ConnectUnconnectedLastNodesToAddedNode(
        INodeMap nodeMap,
        IReadOnlyList<INode> nextDepth,
        INode addedNode)
    {
        foreach (INode lastNode in nodeMap.CurrentDepth)
        {
            if (!HasConnectionToNextDepth(nodeMap, lastNode, nextDepth)
                && CanConnect(lastNode, addedNode))
            {
                Connect(nodeMap, lastNode, addedNode);
            }
        }
    }

    private void TryAddRandomExtraNode(
        INodeMap nodeMap,
        List<int> possibleYPositions,
        List<INode> nextDepth,
        int nextDepthIndex)
    {
        if (possibleYPositions.Count == 0 || random.Next(2) != 0)
        {
            return;
        }

        INode nextNode = new Node(
            NodeType.Undefined,
            nextDepthIndex,
            PopRandomY(possibleYPositions));
        List<INode> validLastNodes = GetValidLastNodes(nodeMap, nextNode);

        if (validLastNodes.Count == 0)
        {
            return;
        }

        nextDepth.Add(nextNode);
        Connect(
            nodeMap,
            validLastNodes[random.Next(validLastNodes.Count)],
            nextNode);
    }

    private void TryAddRandomExtraNodeNoCross(
        INodeMap nodeMap,
        List<int> possibleYPositions,
        List<INode> nextDepth,
        int nextDepthIndex)
    {
        if (possibleYPositions.Count == 0 || random.Next(2) != 0)
        {
            return;
        }

        INode nextNode = new Node(
            NodeType.Undefined,
            nextDepthIndex,
            PopRandomY(possibleYPositions));
        INode? lastNode = GetMatchingLastNode(nodeMap, nextNode)
            ?? GetNearestLastNode(nodeMap, nextNode, nextDepth);

        if (lastNode is null)
        {
            return;
        }

        nextDepth.Add(nextNode);
        Connect(nodeMap, lastNode, nextNode);
    }

    private void TryAddRandomForwardConnectionsNoCross(
        INodeMap nodeMap,
        IReadOnlyList<INode> nextDepth)
    {
        foreach (INode lastNode in nodeMap.CurrentDepth)
        {
            if (random.Next(3) != 0)
            {
                continue;
            }

            List<INode> candidates = GetNoCrossForwardConnectionCandidates(
                nodeMap,
                lastNode,
                nextDepth);

            if (candidates.Count > 0)
            {
                Connect(
                    nodeMap,
                    lastNode,
                    candidates[random.Next(candidates.Count)]);
            }
        }
    }

    private static List<INode> GetNoCrossForwardConnectionCandidates(
        INodeMap nodeMap,
        INode lastNode,
        IReadOnlyList<INode> nextDepth)
    {
        List<INode> candidates = new();

        foreach (INode nextNode in nextDepth)
        {
            if (CanConnect(lastNode, nextNode)
                && !HasForwardConnection(nodeMap, lastNode, nextNode)
                && !WouldCrossForwardConnection(nodeMap, lastNode, nextNode, nextDepth))
            {
                candidates.Add(nextNode);
            }
        }

        return candidates;
    }

    private static INode? GetMatchingLastNode(
        INodeMap nodeMap,
        INode nextNode)
    {
        foreach (INode lastNode in nodeMap.CurrentDepth)
        {
            if (lastNode.Y == nextNode.Y && CanConnect(lastNode, nextNode))
            {
                return lastNode;
            }
        }

        return null;
    }

    private static INode? GetNearestLastNode(
        INodeMap nodeMap,
        INode nextNode,
        IReadOnlyList<INode> nextDepth)
    {
        INode? nearestNode = null;
        int nearestDistance = int.MaxValue;

        foreach (INode lastNode in nodeMap.CurrentDepth)
        {
            int distance = Math.Abs(lastNode.Y - nextNode.Y);

            if (CanConnect(lastNode, nextNode)
                && distance < nearestDistance
                && !WouldCrossForwardConnection(nodeMap, lastNode, nextNode, nextDepth))
            {
                nearestNode = lastNode;
                nearestDistance = distance;
            }
        }

        return nearestNode;
    }

    private static bool HasForwardConnection(
        INodeMap nodeMap,
        INode lastNode,
        INode nextNode)
    {
        return nodeMap.ForwardConnections.TryGetValue(lastNode, out List<INode>? nextNodes)
            && nextNodes.Contains(nextNode);
    }

    private static bool WouldCrossForwardConnection(
        INodeMap nodeMap,
        INode candidateLastNode,
        INode candidateNextNode,
        IReadOnlyList<INode> nextDepth)
    {
        foreach (INode existingLastNode in nodeMap.CurrentDepth)
        {
            if (ReferenceEquals(existingLastNode, candidateLastNode)
                || !nodeMap.ForwardConnections.TryGetValue(existingLastNode, out List<INode>? existingNextNodes))
            {
                continue;
            }

            foreach (INode existingNextNode in existingNextNodes)
            {
                if (!nextDepth.Contains(existingNextNode)
                    || ReferenceEquals(existingNextNode, candidateNextNode))
                {
                    continue;
                }

                bool candidateStartsAbove = candidateLastNode.Y < existingLastNode.Y;
                bool candidateEndsBelow = candidateNextNode.Y > existingNextNode.Y;
                bool candidateStartsBelow = candidateLastNode.Y > existingLastNode.Y;
                bool candidateEndsAbove = candidateNextNode.Y < existingNextNode.Y;

                if ((candidateStartsAbove && candidateEndsBelow)
                    || (candidateStartsBelow && candidateEndsAbove))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool AllLastNodesConnectToNextDepth(
        INodeMap nodeMap,
        IReadOnlyList<INode> nextDepth)
    {
        foreach (INode lastNode in nodeMap.CurrentDepth)
        {
            if (!HasConnectionToNextDepth(nodeMap, lastNode, nextDepth))
            {
                return false;
            }
        }

        return true;
    }

    private static bool HasConnectionToNextDepth(
        INodeMap nodeMap,
        INode lastNode,
        IReadOnlyList<INode> nextDepth)
    {
        if (!nodeMap.ForwardConnections.TryGetValue(lastNode, out List<INode>? nextNodes))
        {
            return false;
        }

        foreach (INode nextNode in nextDepth)
        {
            if (nextNodes.Contains(nextNode))
            {
                return true;
            }
        }

        return false;
    }

    private int RollNodeCount()
    {
        int roll = random.Next(100);

        if (roll < 15)
        {
            return 1;
        }

        if (roll < 85)
        {
            return 2;
        }

        return 3;
    }

    private static List<int> GetPossibleYPositions(
        INodeMap nodeMap,
        IReadOnlyList<int> lastYPositions)
    {
        List<int> possibleYPositions = new();

        foreach (int lastY in lastYPositions)
        {
            for (int y = lastY - 1; y <= lastY + 1; y++)
            {
                if (y >= 0 && y <= nodeMap.YMax && !possibleYPositions.Contains(y))
                {
                    possibleYPositions.Add(y);
                }
            }
        }

        return possibleYPositions;
    }

    private static List<int> GetPossibleYPositions(
        INodeMap nodeMap,
        Dictionary<int, int> yPositionWeights)
    {
        List<int> possibleYPositions = new();

        foreach (int y in yPositionWeights.Keys)
        {
            if (y >= 0 && y <= nodeMap.YMax && !possibleYPositions.Contains(y))
            {
                possibleYPositions.Add(y);
            }
        }

        return possibleYPositions;
    }

    private List<int> SelectNextYPositions(
        IReadOnlyList<int> lastYPositions,
        List<int> possibleYPositions,
        int nodesToAdd)
    {
        List<List<int>> validCombinations = GetValidCombinations(
            lastYPositions,
            possibleYPositions,
            nodesToAdd);

        if (validCombinations.Count == 0)
        {
            return new List<int>();
        }

        return validCombinations[random.Next(validCombinations.Count)];
    }

    private List<int> SelectNextYPositions(
        IReadOnlyList<int> lastYPositions,
        List<int> possibleYPositions,
        Dictionary<int, int> yPositionWeights,
        int nodesToAdd)
    {
        List<List<int>> validCombinations = GetValidCombinations(
            lastYPositions,
            possibleYPositions,
            nodesToAdd);

        if (validCombinations.Count == 0)
        {
            return new List<int>();
        }

        return validCombinations[RollWeightedCombinationIndex(
            validCombinations,
            yPositionWeights)];
    }

    private static List<List<int>> GetValidCombinations(
        IReadOnlyList<int> lastYPositions,
        List<int> possibleYPositions,
        int nodesToAdd)
    {
        List<List<int>> validCombinations = new();

        CollectValidCombinations(
            lastYPositions,
            possibleYPositions,
            nodesToAdd,
            startIndex: 0,
            selectedYPositions: new List<int>(),
            validCombinations);

        return validCombinations;
    }

    private static void CollectValidCombinations(
        IReadOnlyList<int> lastYPositions,
        List<int> possibleYPositions,
        int nodesToAdd,
        int startIndex,
        List<int> selectedYPositions,
        List<List<int>> validCombinations)
    {
        if (selectedYPositions.Count == nodesToAdd)
        {
            if (CoversLastYPositions(lastYPositions, selectedYPositions))
            {
                validCombinations.Add(new List<int>(selectedYPositions));
            }

            return;
        }

        for (int i = startIndex; i < possibleYPositions.Count; i++)
        {
            selectedYPositions.Add(possibleYPositions[i]);

            CollectValidCombinations(
                lastYPositions,
                possibleYPositions,
                nodesToAdd,
                i + 1,
                selectedYPositions,
                validCombinations);

            selectedYPositions.RemoveAt(selectedYPositions.Count - 1);
        }
    }

    private static bool CoversLastYPositions(
        IReadOnlyList<int> lastYPositions,
        List<int> selectedYPositions)
    {
        foreach (int lastY in lastYPositions)
        {
            bool isCovered = false;

            foreach (int selectedY in selectedYPositions)
            {
                if (Math.Abs(lastY - selectedY) <= 1)
                {
                    isCovered = true;
                    break;
                }
            }

            if (!isCovered)
            {
                return false;
            }
        }

        return true;
    }

    private int RollWeightedCombinationIndex(
        List<List<int>> validCombinations,
        Dictionary<int, int> yPositionWeights)
    {
        int totalWeight = 0;

        foreach (List<int> combination in validCombinations)
        {
            totalWeight += GetCombinationWeight(combination, yPositionWeights);
        }

        int roll = random.Next(totalWeight);

        for (int i = 0; i < validCombinations.Count; i++)
        {
            roll -= GetCombinationWeight(validCombinations[i], yPositionWeights);

            if (roll < 0)
            {
                return i;
            }
        }

        return validCombinations.Count - 1;
    }

    private static int GetCombinationWeight(
        List<int> combination,
        Dictionary<int, int> yPositionWeights)
    {
        int weight = 0;

        foreach (int y in combination)
        {
            weight += yPositionWeights[y];
        }

        return weight;
    }

    private static void AddYPositionWeight(
        Dictionary<int, int> yPositionWeights,
        int y)
    {
        if (!yPositionWeights.TryAdd(y, 1))
        {
            yPositionWeights[y]++;
        }
    }

    private static List<INode> GetValidNextNodes(
        INode lastNode,
        IReadOnlyList<INode> nextDepth)
    {
        List<INode> validNextNodes = new();

        foreach (INode nextNode in nextDepth)
        {
            if (CanConnect(lastNode, nextNode))
            {
                validNextNodes.Add(nextNode);
            }
        }

        return validNextNodes;
    }

    private static List<INode> GetValidLastNodes(
        INodeMap nodeMap,
        INode nextNode)
    {
        List<INode> validLastNodes = new();

        foreach (INode lastNode in nodeMap.CurrentDepth)
        {
            if (CanConnect(lastNode, nextNode))
            {
                validLastNodes.Add(lastNode);
            }
        }

        return validLastNodes;
    }

    private static bool HasIncomingFromLast(
        INodeMap nodeMap,
        INode nextNode)
    {
        foreach (INode lastNode in nodeMap.CurrentDepth)
        {
            if (nodeMap.ForwardConnections.TryGetValue(lastNode, out List<INode>? nextNodes)
                && nextNodes.Contains(nextNode))
            {
                return true;
            }
        }

        return false;
    }

    private static bool CanConnect(
        INode lastNode,
        INode nextNode)
    {
        return nextNode.Depth == lastNode.Depth + 1
            && Math.Abs(lastNode.Y - nextNode.Y) <= 1;
    }

    private static void Connect(
        INodeMap nodeMap,
        INode lastNode,
        INode nextNode)
    {
        if (!nodeMap.ForwardConnections.TryGetValue(lastNode, out List<INode>? nextNodes))
        {
            nextNodes = new List<INode>();
            nodeMap.ForwardConnections[lastNode] = nextNodes;
        }

        if (!nextNodes.Contains(nextNode))
        {
            nextNodes.Add(nextNode);
        }
    }
}
