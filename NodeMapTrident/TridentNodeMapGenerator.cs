using System;
using System.Collections.Generic;

namespace NodeMapTrident;

public class TridentNodeMapGenerator
{
    private readonly Random random;
    private readonly TridentConnectionWeights connectionWeights;

    public TridentNodeMapGenerator(
        Random? random = null,
        TridentConnectionWeights? connectionWeights = null)
    {
        this.random = random ?? new Random();
        this.connectionWeights = connectionWeights ?? TridentConnectionWeights.Default;
    }

    public TridentMapLayer CreateInitialLayer(
        int depth = 0,
        IReadOnlyList<int>? yPositions = null)
    {
        int[] initialYPositions = yPositions is null
            ? new[] { TridentLaneRules.CentralY }
            : CopyYPositions(yPositions);

        return CreateLayer(depth, initialYPositions);
    }

    public TridentMapLayer GenerateNextLayer(TridentMapLayer currentLayer)
    {
        int nextDepth = currentLayer.Depth + 1;
        int[] nextYPositions = GenerateNextYPositions(currentLayer.Nodes);
        TridentMapLayer nextLayer = CreateLayer(nextDepth, nextYPositions);

        AddForwardConnections(currentLayer.Nodes, nextLayer.Nodes);
        ValidateIncomingConnections(currentLayer.Nodes, nextLayer.Nodes);

        return nextLayer;
    }

    private int[] GenerateNextYPositions(IReadOnlyList<TridentMapNode> currentNodes)
    {
        bool[] possibleY = GetPossibleNextYPositions(currentNodes);
        int possibleCount = Count(possibleY);
        int targetCount = random.Next(1, possibleCount + 1);
        List<int> selectedYPositions = new();
        int[] shuffledPossibleYPositions = ToShuffledYPositions(possibleY);

        for (int i = 0; i < shuffledPossibleYPositions.Length
            && selectedYPositions.Count < targetCount;
            i++)
        {
            selectedYPositions.Add(shuffledPossibleYPositions[i]);
        }

        EnsureEveryCurrentNodeHasFutureTarget(currentNodes, selectedYPositions, possibleY);
        selectedYPositions.Sort();
        return selectedYPositions.ToArray();
    }

    private static bool[] GetPossibleNextYPositions(IReadOnlyList<TridentMapNode> currentNodes)
    {
        if (currentNodes.Count < 1 || currentNodes.Count > TridentLaneRules.MaxNodesPerDepth)
        {
            throw new ArgumentOutOfRangeException(
                nameof(currentNodes),
                currentNodes.Count,
                "Current layer must contain 1 to 3 nodes.");
        }

        bool[] occupiedCurrentY = new bool[TridentLaneRules.MaxY + 1];
        bool[] possibleY = new bool[TridentLaneRules.MaxY + 1];

        foreach (TridentMapNode node in currentNodes)
        {
            TridentLaneRules.ValidateY(node.Y);

            if (occupiedCurrentY[node.Y])
            {
                throw new ArgumentException(
                    "Current nodes cannot share the same Y.",
                    nameof(currentNodes));
            }

            occupiedCurrentY[node.Y] = true;

            for (int y = node.Y - 1; y <= node.Y + 1; y++)
            {
                if (y >= TridentLaneRules.MinY && y <= TridentLaneRules.MaxY)
                {
                    possibleY[y] = true;
                }
            }
        }

        return possibleY;
    }

    private void EnsureEveryCurrentNodeHasFutureTarget(
        IReadOnlyList<TridentMapNode> currentNodes,
        List<int> selectedYPositions,
        bool[] possibleY)
    {
        foreach (TridentMapNode currentNode in currentNodes)
        {
            if (HasValidYTarget(currentNode.Y, selectedYPositions))
            {
                continue;
            }

            int[] repairCandidates = ToShuffledYPositions(possibleY);

            for (int i = 0; i < repairCandidates.Length; i++)
            {
                int candidateY = repairCandidates[i];

                if (TridentLaneRules.CanConnect(currentNode.Y, candidateY)
                    && !selectedYPositions.Contains(candidateY))
                {
                    selectedYPositions.Add(candidateY);
                    break;
                }
            }
        }
    }

    private void AddForwardConnections(
        IReadOnlyList<TridentMapNode> currentNodes,
        IReadOnlyList<TridentMapNode> nextNodes)
    {
        foreach (TridentMapNode currentNode in currentNodes)
        {
            TridentMapNode[] validTargets = GetValidNextNodes(currentNode, nextNodes);
            Shuffle(validTargets);

            int desiredConnectionCount = Math.Min(
                RollConnectionCount(),
                validTargets.Length);

            for (int i = 0; i < desiredConnectionCount; i++)
            {
                currentNode.TryConnectTo(validTargets[i]);
            }
        }
    }

    private void ValidateIncomingConnections(
        IReadOnlyList<TridentMapNode> currentNodes,
        IReadOnlyList<TridentMapNode> nextNodes)
    {
        foreach (TridentMapNode nextNode in nextNodes)
        {
            if (nextNode.IncomingNodes.Count > 0)
            {
                continue;
            }

            TridentMapNode[] validSources = GetValidPreviousNodes(nextNode, currentNodes);
            Shuffle(validSources);

            for (int i = 0; i < validSources.Length; i++)
            {
                if (validSources[i].TryConnectTo(nextNode))
                {
                    break;
                }
            }
        }
    }

    private int RollConnectionCount()
    {
        int totalWeight = connectionWeights.OneConnectionWeight
            + connectionWeights.TwoConnectionWeight
            + connectionWeights.ThreeConnectionWeight;
        int roll = random.Next(totalWeight);

        if (roll < connectionWeights.OneConnectionWeight)
        {
            return 1;
        }

        roll -= connectionWeights.OneConnectionWeight;

        if (roll < connectionWeights.TwoConnectionWeight)
        {
            return 2;
        }

        return 3;
    }

    private TridentMapLayer CreateLayer(
        int depth,
        IReadOnlyList<int> yPositions)
    {
        TridentMapNode[] nodes = new TridentMapNode[yPositions.Count];

        for (int i = 0; i < yPositions.Count; i++)
        {
            nodes[i] = new TridentMapNode(depth, yPositions[i]);
        }

        return new TridentMapLayer(depth, nodes);
    }

    private TridentMapNode[] GetValidNextNodes(
        TridentMapNode currentNode,
        IReadOnlyList<TridentMapNode> nextNodes)
    {
        List<TridentMapNode> validTargets = new();

        foreach (TridentMapNode nextNode in nextNodes)
        {
            if (TridentLaneRules.CanConnect(currentNode.Y, nextNode.Y))
            {
                validTargets.Add(nextNode);
            }
        }

        return validTargets.ToArray();
    }

    private TridentMapNode[] GetValidPreviousNodes(
        TridentMapNode nextNode,
        IReadOnlyList<TridentMapNode> currentNodes)
    {
        List<TridentMapNode> validSources = new();

        foreach (TridentMapNode currentNode in currentNodes)
        {
            if (TridentLaneRules.CanConnect(currentNode.Y, nextNode.Y))
            {
                validSources.Add(currentNode);
            }
        }

        return validSources.ToArray();
    }

    private int[] ToShuffledYPositions(bool[] yPositions)
    {
        int[] values = new int[Count(yPositions)];
        int index = 0;

        for (int y = TridentLaneRules.MinY; y <= TridentLaneRules.MaxY; y++)
        {
            if (yPositions[y])
            {
                values[index] = y;
                index++;
            }
        }

        Shuffle(values);
        return values;
    }

    private void Shuffle<T>(T[] values)
    {
        for (int i = values.Length - 1; i > 0; i--)
        {
            int swapIndex = random.Next(i + 1);
            (values[i], values[swapIndex]) = (values[swapIndex], values[i]);
        }
    }

    private static bool HasValidYTarget(
        int currentY,
        List<int> selectedYPositions)
    {
        foreach (int selectedY in selectedYPositions)
        {
            if (TridentLaneRules.CanConnect(currentY, selectedY))
            {
                return true;
            }
        }

        return false;
    }

    private static int Count(bool[] values)
    {
        int count = 0;

        foreach (bool value in values)
        {
            if (value)
            {
                count++;
            }
        }

        return count;
    }

    private static int[] CopyYPositions(IReadOnlyList<int> yPositions)
    {
        int[] values = new int[yPositions.Count];

        for (int i = 0; i < yPositions.Count; i++)
        {
            values[i] = yPositions[i];
        }

        return values;
    }
}
