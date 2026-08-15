using Shared;

namespace ExpeditionTileMap;

public class ExpeditionTileMapService
{
    public bool TryProposeWaypoint(ExpeditionTileMap map, int x, int y)
    {
        return TryProposeWaypoint(map, new V2I(x, y));
    }

    public bool TryProposeWaypoint(ExpeditionTileMap map, V2I waypoint)
    {
        if (!map.TilesByPosition.ContainsKey(waypoint))
        {
            return false;
        }

        if (map.ProposedPath.Count == 0)
        {
            if (!map.RelaysByPosition.TryGetValue(waypoint, out ExpeditionRelay? startRelay) || !startRelay.Connected)
            {
                return false;
            }

            AddRelayOrder(map, waypoint);
            map.ProposedPath.Add(waypoint);
            map.CompletedProposedPathIndex = -1;
            MarkProposedPathChanged(map);
            MarkProposedPathProgressChanged(map);
            MarkDirty(map, waypoint);
            return true;
        }

        V2I previous = map.ProposedPath[^1];
        if (previous == waypoint)
        {
            return false;
        }

        if (previous.X != waypoint.X && previous.Y != waypoint.Y)
        {
            return false;
        }

        if (!TryBuildStraightSegment(previous, waypoint, out List<V2I> segment))
        {
            return false;
        }

        for (int i = 1; i < segment.Count - 1; i++)
        {
            V2I position = segment[i];

            if (!map.TilesByPosition.ContainsKey(position))
            {
                return false;
            }

            if (HasWire(map, position))
            {
                return false;
            }
        }

        for (int i = 1; i < segment.Count; i++)
        {
            V2I position = segment[i];

            if (!map.TilesByPosition.ContainsKey(position))
            {
                return false;
            }

            map.ProposedPath.Add(position);
            MarkDirty(map, position);
        }

        map.ProposedRelays.Add(waypoint);
        MarkProposedPathChanged(map);
        MarkProposedRelaysChanged(map);
        MarkDirty(map, previous);
        return true;
    }

    public void ClearProposedPath(ExpeditionTileMap map)
    {
        foreach (V2I position in map.ProposedPath)
        {
            MarkDirty(map, position);
        }

        foreach (V2I position in map.ProposedRelays)
        {
            MarkDirty(map, position);
        }

        map.ProposedPath.Clear();
        map.ProposedRelays.Clear();
        map.CompletedProposedPathIndex = -1;
        MarkProposedPathChanged(map);
        MarkProposedRelaysChanged(map);
        MarkProposedPathProgressChanged(map);
    }

    public bool AdvanceProposedPath(ExpeditionTileMap map)
    {
        int nextIndex = map.CompletedProposedPathIndex + 1;
        if (nextIndex < 0 || nextIndex >= map.ProposedPath.Count)
        {
            return false;
        }

        if (map.CompletedProposedPathIndex >= 0)
        {
            MarkDirty(map, map.ProposedPath[map.CompletedProposedPathIndex]);
        }

        map.CompletedProposedPathIndex = nextIndex;
        V2I completedPosition = map.ProposedPath[nextIndex];
        map.CurrentPosition = completedPosition;
        MarkDirty(map, completedPosition);
        MarkProposedPathProgressChanged(map);

        if (map.ProposedRelays.Contains(completedPosition))
        {
            IntegrateProposedRelaySegment(map, nextIndex);
        }

        return true;
    }

    private void IntegrateProposedRelaySegment(ExpeditionTileMap map, int relayIndex)
    {
        if (!TryFindPreviousPersistentRelayIndex(map, relayIndex, out int startIndex))
        {
            return;
        }

        V2I endRelayPosition = map.ProposedPath[relayIndex];
        GetOrCreateRelay(map, endRelayPosition);
        AddRelayOrder(map, endRelayPosition);
        MarkRelayChanged(map, endRelayPosition);

        for (int i = startIndex; i < relayIndex; i++)
        {
            V2I from = map.ProposedPath[i];
            V2I to = map.ProposedPath[i + 1];
            TrySetWire(map, from, to, true);
        }

        map.ProposedRelays.Remove(endRelayPosition);
        MarkProposedRelaysChanged(map);
        MarkStructureChanged(map, endRelayPosition);
    }

    private bool TryFindPreviousPersistentRelayIndex(ExpeditionTileMap map, int relayIndex, out int previousRelayIndex)
    {
        for (int i = relayIndex - 1; i >= 0; i--)
        {
            if (map.RelaysByPosition.ContainsKey(map.ProposedPath[i]))
            {
                previousRelayIndex = i;
                return true;
            }
        }

        previousRelayIndex = -1;
        return false;
    }
    private ExpeditionRelay GetOrCreateRelay(ExpeditionTileMap map, V2I position)
    {
        if (map.RelaysByPosition.TryGetValue(position, out ExpeditionRelay? relay))
        {
            return relay;
        }

        relay = new ExpeditionRelay(position);
        map.RelaysByPosition.Add(position, relay);
        return relay;
    }

    private bool TryBuildStraightSegment(V2I from, V2I to, out List<V2I> segment)
    {
        segment = new List<V2I>();

        int dx = Math.Sign(to.X - from.X);
        int dy = Math.Sign(to.Y - from.Y);

        if (dx != 0 && dy != 0)
        {
            return false;
        }

        V2I current = from;
        segment.Add(current);

        while (current != to)
        {
            current = new V2I(current.X + dx, current.Y + dy);
            segment.Add(current);
        }

        return true;
    }

    private bool HasWire(ExpeditionTileMap map, V2I position)
    {
        return map.WiresByPosition.TryGetValue(position, out ExpeditionWire? wire) && wire.HasAnyWire;
    }

    private bool TrySetWire(ExpeditionTileMap map, V2I from, V2I to, bool enabled)
    {
        V2I delta = to - from;
        ExpeditionWire wire = GetOrCreateWire(map, from);
        bool changed;

        if (delta.X == 0 && delta.Y == -1)
        {
            changed = wire.Up != enabled;
            wire.Up = enabled;
        }
        else if (delta.X == 1 && delta.Y == 0)
        {
            changed = wire.Right != enabled;
            wire.Right = enabled;
        }
        else if (delta.X == 0 && delta.Y == 1)
        {
            changed = wire.Down != enabled;
            wire.Down = enabled;
        }
        else if (delta.X == -1 && delta.Y == 0)
        {
            changed = wire.Left != enabled;
            wire.Left = enabled;
        }
        else
        {
            return false;
        }

        if (!wire.HasAnyWire)
        {
            map.WiresByPosition.Remove(from);
        }

        if (changed)
        {
            MarkWireChanged(map, from, to);
        }

        return true;
    }

    private ExpeditionWire GetOrCreateWire(ExpeditionTileMap map, V2I position)
    {
        if (map.WiresByPosition.TryGetValue(position, out ExpeditionWire? wire))
        {
            return wire;
        }

        wire = new ExpeditionWire();
        map.WiresByPosition.Add(position, wire);
        return wire;
    }

    public bool ReconnectGrid(ExpeditionTileMap map)
    {
        foreach ((V2I position, ExpeditionRelay relay) in map.RelaysByPosition)
        {
            relay.Connected = false;
            relay.Powered = false;
            MarkDirty(map, position);
        }

        map.PowerUsage = 0;
        V2I rootPosition = new(0, 0);

        if (!map.RelaysByPosition.TryGetValue(rootPosition, out ExpeditionRelay? rootRelay))
        {
            MarkRelayChanged(map, rootPosition);
            MarkPowerChanged(map);
            return false;
        }

        Queue<V2I> openPositions = new();
        HashSet<V2I> visitedPositions = new();

        rootRelay.Connected = true;
        openPositions.Enqueue(rootPosition);
        visitedPositions.Add(rootPosition);
        MarkRelayChanged(map, rootPosition);

        while (openPositions.Count > 0)
        {
            V2I position = openPositions.Dequeue();

            if (!map.WiresByPosition.TryGetValue(position, out ExpeditionWire? wire))
            {
                continue;
            }

            TryReachRelay(map, position, new V2I(0, -1), wire.Up, openPositions, visitedPositions);
            TryReachRelay(map, position, new V2I(1, 0), wire.Right, openPositions, visitedPositions);
            TryReachRelay(map, position, new V2I(0, 1), wire.Down, openPositions, visitedPositions);
            TryReachRelay(map, position, new V2I(-1, 0), wire.Left, openPositions, visitedPositions);
        }

        TryPowerRelay(map, rootPosition);

        foreach (V2I relayPosition in map.RelayOrder)
        {
            if (relayPosition == rootPosition)
            {
                continue;
            }

            TryPowerRelay(map, relayPosition);
        }

        MarkPowerChanged(map);
        return true;
    }

    private void TryReachRelay(
        ExpeditionTileMap map,
        V2I from,
        V2I offset,
        bool wireEnabled,
        Queue<V2I> openPositions,
        HashSet<V2I> visitedPositions)
    {
        if (!wireEnabled)
        {
            return;
        }

        V2I targetPosition = from + offset;
        if (!visitedPositions.Add(targetPosition))
        {
            return;
        }

        if (map.RelaysByPosition.TryGetValue(targetPosition, out ExpeditionRelay? relay))
        {
            relay.Connected = true;
            MarkRelayChanged(map, targetPosition);
        }

        if (relay is not null || map.WiresByPosition.ContainsKey(targetPosition))
        {
            openPositions.Enqueue(targetPosition);
        }
    }

    private void TryPowerRelay(ExpeditionTileMap map, V2I position)
    {
        if (map.PowerUsage >= map.PowerLimit)
        {
            return;
        }

        if (!map.RelaysByPosition.TryGetValue(position, out ExpeditionRelay? relay) || !relay.Connected || relay.Powered)
        {
            return;
        }

        relay.Powered = true;
        map.PowerUsage++;
        MarkRelayChanged(map, position);
    }

    private void AddRelayOrder(ExpeditionTileMap map, V2I position)
    {
        if (map.RelayOrder.Contains(position))
        {
            return;
        }

        map.RelayOrder.Add(position);
        map.RelayOrderVersion = IncrementVersion(map);
        MarkDirty(map, position);
    }
    private void MarkStructureChanged(ExpeditionTileMap map, V2I position)
    {
        map.StructureVersion = IncrementVersion(map);
        MarkDirty(map, position);
    }

    private void MarkRelayChanged(ExpeditionTileMap map, V2I position)
    {
        map.RelayVersion = IncrementVersion(map);
        MarkDirty(map, position);
    }

    private void MarkProposedPathChanged(ExpeditionTileMap map)
    {
        map.ProposedPathVersion = IncrementVersion(map);
    }

    private void MarkProposedRelaysChanged(ExpeditionTileMap map)
    {
        map.ProposedRelaysVersion = IncrementVersion(map);
    }

    private void MarkProposedPathProgressChanged(ExpeditionTileMap map)
    {
        map.ProposedPathProgressVersion = IncrementVersion(map);
    }

    private void MarkPowerChanged(ExpeditionTileMap map)
    {
        map.PowerVersion = IncrementVersion(map);
    }


    private void MarkWireChanged(ExpeditionTileMap map, V2I from, V2I to)
    {
        map.WireVersion = IncrementVersion(map);
        MarkDirty(map, from);
        MarkDirty(map, to);
    }

    private void MarkDirty(ExpeditionTileMap map, V2I position)
    {
        map.DirtyPositions.Add(position);
    }

    private long IncrementVersion(ExpeditionTileMap map)
    {
        map.Version++;
        return map.Version;
    }
}






