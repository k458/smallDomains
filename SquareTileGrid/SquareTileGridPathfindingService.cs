using System.Collections.Generic;

namespace SquareTileGrid;

public class SquareTileGridPathfindingService
{
    private readonly Stack<Dictionary<ISquareTile, ISquareTile?>> storedCameFrom = new();
    private readonly Stack<Dictionary<ISquareTile, int>> storedPathCosts = new();
    private readonly Stack<PriorityQueue<ISquareTile, int>> storedOpenTiles = new();

    public bool TryFindPath(
        IGridComposition gridComposition,
        SquareTileGridPosition startPosition,
        SquareTileGridPosition endPosition,
        SquareTileAdjacencyType adjacencyType,
        int maxPathCost,
        List<ISquareTile> path)
    {
        path.Clear();

        if (!gridComposition.TryGetTile(startPosition, out ISquareTile? startTile)
            || !gridComposition.TryGetTile(endPosition, out ISquareTile? endTile))
        {
            return false;
        }

        ISquareTile checkedStartTile = startTile!;
        ISquareTile checkedEndTile = endTile!;

        Dictionary<ISquareTile, ISquareTile?> cameFrom = GetCameFrom();
        Dictionary<ISquareTile, int> pathCosts = GetPathCosts();
        PriorityQueue<ISquareTile, int> openTiles = GetOpenTiles();

        try
        {
            cameFrom[checkedStartTile] = null;
            pathCosts[checkedStartTile] = 0;
            openTiles.Enqueue(checkedStartTile, 0);

            while (openTiles.Count > 0)
            {
                ISquareTile currentTile = openTiles.Dequeue();

                if (currentTile == checkedEndTile)
                {
                    BuildPath(checkedEndTile, cameFrom, path);
                    return true;
                }

                if (!gridComposition.TryGetTilePosition(
                        currentTile,
                        out SquareTileGridPosition currentPosition))
                {
                    continue;
                }

                for (int i = 0; i < IGridComposition.AdjacentTileCount; i++)
                {
                    if (!gridComposition.TryGetAdjacentTile(
                            currentTile,
                            adjacencyType,
                            i,
                            out ISquareTile? visibleAdjacentTile))
                    {
                        continue;
                    }

                    if (!gridComposition.TryGetTilePosition(
                            visibleAdjacentTile!,
                            out SquareTileGridPosition adjacentPosition))
                    {
                        continue;
                    }

                    int newPathCost = pathCosts[currentTile]
                        + GetStepCost(currentPosition, adjacentPosition, currentTile.ParentGrid);

                    if (newPathCost > maxPathCost)
                    {
                        continue;
                    }

                    if (pathCosts.TryGetValue(visibleAdjacentTile!, out int existingPathCost)
                        && existingPathCost <= newPathCost)
                    {
                        continue;
                    }

                    cameFrom[visibleAdjacentTile!] = currentTile;
                    pathCosts[visibleAdjacentTile!] = newPathCost;
                    int priority = newPathCost
                        + GetEstimatedCost(
                            visibleAdjacentTile!,
                            checkedEndTile,
                            gridComposition);
                    openTiles.Enqueue(visibleAdjacentTile!, priority);
                }
            }

            return false;
        }
        finally
        {
            ReturnCameFrom(cameFrom);
            ReturnPathCosts(pathCosts);
            ReturnOpenTiles(openTiles);
        }
    }

    private Dictionary<ISquareTile, ISquareTile?> GetCameFrom()
    {
        return storedCameFrom.Count > 0
            ? storedCameFrom.Pop()
            : new Dictionary<ISquareTile, ISquareTile?>();
    }

    private void ReturnCameFrom(Dictionary<ISquareTile, ISquareTile?> cameFrom)
    {
        cameFrom.Clear();
        storedCameFrom.Push(cameFrom);
    }

    private Dictionary<ISquareTile, int> GetPathCosts()
    {
        return storedPathCosts.Count > 0
            ? storedPathCosts.Pop()
            : new Dictionary<ISquareTile, int>();
    }

    private void ReturnPathCosts(Dictionary<ISquareTile, int> pathCosts)
    {
        pathCosts.Clear();
        storedPathCosts.Push(pathCosts);
    }

    private PriorityQueue<ISquareTile, int> GetOpenTiles()
    {
        return storedOpenTiles.Count > 0
            ? storedOpenTiles.Pop()
            : new PriorityQueue<ISquareTile, int>();
    }

    private void ReturnOpenTiles(PriorityQueue<ISquareTile, int> openTiles)
    {
        openTiles.Clear();
        storedOpenTiles.Push(openTiles);
    }

    private static int GetStepCost(
        SquareTileGridPosition currentPosition,
        SquareTileGridPosition adjacentPosition,
        ISquareTileGrid costGrid)
    {
        int deltaX = System.Math.Abs(adjacentPosition.X - currentPosition.X);
        int deltaY = System.Math.Abs(adjacentPosition.Y - currentPosition.Y);

        return deltaX != 0 && deltaY != 0
            ? costGrid.DiagonalPathCost
            : costGrid.PathCost;
    }

    private static int GetEstimatedCost(
        ISquareTile currentTile,
        ISquareTile endTile,
        IGridComposition gridComposition)
    {
        if (!gridComposition.TryGetTilePosition(
                currentTile,
                out SquareTileGridPosition currentPosition)
            || !gridComposition.TryGetTilePosition(
                endTile,
                out SquareTileGridPosition endPosition))
        {
            return 0;
        }

        int deltaX = System.Math.Abs(endPosition.X - currentPosition.X);
        int deltaY = System.Math.Abs(endPosition.Y - currentPosition.Y);
        int diagonalSteps = System.Math.Min(deltaX, deltaY);
        int straightSteps = System.Math.Max(deltaX, deltaY) - diagonalSteps;
        ISquareTileGrid costGrid = currentTile.ParentGrid;

        return diagonalSteps * costGrid.DiagonalPathCost
            + straightSteps * costGrid.PathCost;
    }

    private static void BuildPath(
        ISquareTile endTile,
        Dictionary<ISquareTile, ISquareTile?> cameFrom,
        List<ISquareTile> path)
    {
        ISquareTile? currentTile = endTile;

        while (currentTile is not null)
        {
            path.Add(currentTile);
            currentTile = cameFrom[currentTile];
        }

        path.Reverse();
    }
}
