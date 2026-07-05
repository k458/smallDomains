using System;
using System.Collections.Generic;

namespace ArenaPositioning;

public class ArenaPositionLine
{
    private readonly ArenaPosition[] positions;

    public ArenaPositionLine(int positionCount)
    {
        if (positionCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(positionCount), "Arena must contain at least one position.");
        }

        positions = new ArenaPosition[positionCount];

        for (var index = 0; index < positions.Length; index++)
        {
            positions[index] = new ArenaPosition(index);
        }
    }

    public int Count => positions.Length;

    public IReadOnlyList<ArenaPosition> Positions => positions;

    public ArenaPosition this[int rankIndex] => positions[ValidateRankIndex(rankIndex)];

    public bool TryPlaceOccupant(int rankIndex, IArenaPositionOccupant occupant)
    {
        ArgumentNullException.ThrowIfNull(occupant);

        var position = positions[ValidateRankIndex(rankIndex)];

        if (position.IsOccupied)
        {
            return false;
        }

        position.Occupant = occupant;
        return true;
    }

    public IArenaPositionOccupant? RemoveOccupant(int rankIndex)
    {
        var position = positions[ValidateRankIndex(rankIndex)];
        var occupant = position.Occupant;
        position.Occupant = null;
        return occupant;
    }

    public int? GetFrontmostAlly(
        int rankIndex,
        out IArenaPositionOccupant? ally)
    {
        IArenaPositionOccupant? occupant = positions[ValidateRankIndex(rankIndex)].Occupant;

        if (occupant is null)
        {
            ally = null;
            return null;
        }

        int step = occupant.Team == ArenaTeam.Left ? 1 : -1;
        int frontmostRankIndex = rankIndex;

        while (IsInBounds(frontmostRankIndex + step)
            && IsSameTeam(positions[frontmostRankIndex + step].Occupant, occupant.Team))
        {
            frontmostRankIndex += step;
        }

        ally = positions[frontmostRankIndex].Occupant;
        return frontmostRankIndex;
    }

    public IReadOnlyList<IArenaPositionOccupant?> GetOccupantsInRange(
        int originRankIndex,
        ArenaPositionDirection direction,
        int minRange,
        int maxRange)
    {
        ValidateRankIndex(originRankIndex);

        if (minRange < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minRange), minRange, "Minimum range must not be negative.");
        }

        if (maxRange < minRange)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRange), maxRange, "Maximum range must be greater than or equal to minimum range.");
        }

        var occupants = new List<IArenaPositionOccupant?>();

        if (direction == ArenaPositionDirection.All)
        {
            AddOccupantsInRange(occupants, originRankIndex, ArenaPositionDirection.Left, minRange, maxRange);
            AddOccupantsInRange(occupants, originRankIndex, ArenaPositionDirection.Right, minRange, maxRange);

            return occupants;
        }

        AddOccupantsInRange(occupants, originRankIndex, direction, minRange, maxRange);

        return occupants;
    }

    private void AddOccupantsInRange(
        List<IArenaPositionOccupant?> occupants,
        int originRankIndex,
        ArenaPositionDirection direction,
        int minRange,
        int maxRange)
    {
        var step = direction.ToStep();

        for (var distance = minRange; distance <= maxRange; distance++)
        {
            var rankIndex = originRankIndex + step * distance;

            if (!IsInBounds(rankIndex))
            {
                break;
            }

            occupants.Add(positions[rankIndex].Occupant);
        }
    }

    public int Pull(int anchorRankIndex, ArenaTeam initiatorTeam, ArenaPositionDirection direction)
    {
        ValidateRankIndex(anchorRankIndex);
        direction.ToStep();

        var movedCount = 0;
        int? emptyRankIndex = null;

        foreach (var rankIndex in EnumerateRanksFromAnchor(anchorRankIndex, direction))
        {
            var position = positions[rankIndex];

            if (position.Occupant is null)
            {
                emptyRankIndex ??= rankIndex;
                continue;
            }

            if (!IsSameTeam(position.Occupant, initiatorTeam))
            {
                break;
            }

            if (emptyRankIndex is null)
            {
                continue;
            }

            positions[emptyRankIndex.Value].Occupant = position.Occupant;
            position.Occupant = null;
            emptyRankIndex = rankIndex;
            movedCount++;
        }

        return movedCount;
    }

    public bool CanPull(int anchorRankIndex, ArenaTeam initiatorTeam, ArenaPositionDirection direction)
    {
        ValidateRankIndex(anchorRankIndex);
        direction.ToStep();

        int? emptyRankIndex = null;

        foreach (var rankIndex in EnumerateRanksFromAnchor(anchorRankIndex, direction))
        {
            var position = positions[rankIndex];

            if (position.Occupant is null)
            {
                emptyRankIndex ??= rankIndex;
                continue;
            }

            if (!IsSameTeam(position.Occupant, initiatorTeam))
            {
                return false;
            }

            if (emptyRankIndex is not null)
            {
                return true;
            }
        }

        return false;
    }

    public bool Push(int targetRankIndex, ArenaTeam initiatorTeam, ArenaPositionDirection direction)
    {
        ValidateRankIndex(targetRankIndex);

        var targetOccupant = positions[targetRankIndex].Occupant;

        if (targetOccupant is null || !IsSameTeam(targetOccupant, initiatorTeam))
        {
            return false;
        }

        var step = direction.ToStep();
        var emptyRankIndex = targetRankIndex + step;

        while (IsInBounds(emptyRankIndex) && positions[emptyRankIndex].IsOccupied)
        {
            if (!IsSameTeam(positions[emptyRankIndex].Occupant!, initiatorTeam))
            {
                return false;
            }

            emptyRankIndex += step;
        }

        if (!IsInBounds(emptyRankIndex))
        {
            return false;
        }

        PushChain(targetRankIndex, emptyRankIndex, step);
        return true;
    }

    public bool Push(int targetRankIndex, ArenaPositionDirection direction)
    {
        ValidateRankIndex(targetRankIndex);

        if (positions[targetRankIndex].Occupant is null)
        {
            return false;
        }

        var step = direction.ToStep();
        var emptyRankIndex = FindEmptyRankIndex(targetRankIndex + step, step);

        if (emptyRankIndex is null)
        {
            return false;
        }

        PushChain(targetRankIndex, emptyRankIndex.Value, step);
        return true;
    }

    public bool CanPush(int targetRankIndex, ArenaTeam initiatorTeam, ArenaPositionDirection direction)
    {
        ValidateRankIndex(targetRankIndex);

        var targetOccupant = positions[targetRankIndex].Occupant;

        if (targetOccupant is null || !IsSameTeam(targetOccupant, initiatorTeam))
        {
            return false;
        }

        var step = direction.ToStep();
        var rankIndex = targetRankIndex + step;

        while (IsInBounds(rankIndex))
        {
            var occupant = positions[rankIndex].Occupant;

            if (occupant is null)
            {
                return true;
            }

            if (!IsSameTeam(occupant, initiatorTeam))
            {
                return false;
            }

            rankIndex += step;
        }

        return false;
    }

    public bool CanPush(int targetRankIndex, ArenaPositionDirection direction)
    {
        ValidateRankIndex(targetRankIndex);

        if (positions[targetRankIndex].Occupant is null)
        {
            return false;
        }

        var step = direction.ToStep();
        return FindEmptyRankIndex(targetRankIndex + step, step) is not null;
    }

    public bool PullFormation(int targetRankIndex)
    {
        IArenaPositionOccupant? targetOccupant = positions[ValidateRankIndex(targetRankIndex)].Occupant;

        if (targetOccupant is null)
        {
            return false;
        }

        ArenaPositionDirection direction = targetOccupant.Team == ArenaTeam.Left
            ? ArenaPositionDirection.Left
            : ArenaPositionDirection.Right;

        return MoveFormationChain(targetRankIndex, targetOccupant.Team, direction);
    }

    public bool PushFormation(int targetRankIndex)
    {
        IArenaPositionOccupant? targetOccupant = positions[ValidateRankIndex(targetRankIndex)].Occupant;

        if (targetOccupant is null)
        {
            return false;
        }

        ArenaPositionDirection direction = targetOccupant.Team == ArenaTeam.Left
            ? ArenaPositionDirection.Right
            : ArenaPositionDirection.Left;

        return MoveFormationChain(targetRankIndex, targetOccupant.Team, direction);
    }

    private IEnumerable<int> EnumerateRanksFromAnchor(int anchorRankIndex, ArenaPositionDirection direction)
    {
        if (direction == ArenaPositionDirection.Left)
        {
            for (var rankIndex = anchorRankIndex; rankIndex < positions.Length; rankIndex++)
            {
                yield return rankIndex;
            }
        }
        else
        {
            for (var rankIndex = anchorRankIndex; rankIndex >= 0; rankIndex--)
            {
                yield return rankIndex;
            }
        }
    }

    private int ValidateRankIndex(int rankIndex)
    {
        if (!IsInBounds(rankIndex))
        {
            throw new ArgumentOutOfRangeException(nameof(rankIndex), rankIndex, "Rank index is outside the arena.");
        }

        return rankIndex;
    }

    private bool IsInBounds(int rankIndex)
    {
        return rankIndex >= 0 && rankIndex < positions.Length;
    }

    private int? FindEmptyRankIndex(int startRankIndex, int step)
    {
        for (var rankIndex = startRankIndex; IsInBounds(rankIndex); rankIndex += step)
        {
            if (!positions[rankIndex].IsOccupied)
            {
                return rankIndex;
            }
        }

        return null;
    }

    private void PushChain(int targetRankIndex, int emptyRankIndex, int step)
    {
        for (var rankIndex = emptyRankIndex; rankIndex != targetRankIndex; rankIndex -= step)
        {
            positions[rankIndex].Occupant = positions[rankIndex - step].Occupant;
        }

        positions[targetRankIndex].Occupant = null;
    }

    private bool MoveFormationChain(
        int targetRankIndex,
        ArenaTeam team,
        ArenaPositionDirection direction)
    {
        int step = direction.ToStep();
        int chainEndRankIndex = targetRankIndex;

        while (IsInBounds(chainEndRankIndex + step)
            && IsSameTeam(positions[chainEndRankIndex + step].Occupant, team))
        {
            chainEndRankIndex += step;
        }

        int destinationRankIndex = chainEndRankIndex + step;

        if (!IsInBounds(destinationRankIndex) || positions[destinationRankIndex].IsOccupied)
        {
            return false;
        }

        PushChain(targetRankIndex, destinationRankIndex, step);
        return true;
    }

    private static bool IsSameTeam(IArenaPositionOccupant? occupant, ArenaTeam team)
    {
        return occupant?.Team == team;
    }
}
