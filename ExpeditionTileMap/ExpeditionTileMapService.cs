namespace ExpeditionTileMap;

public class ExpeditionTileMapService
{
    public ExpeditionTile DiscoverTile(ExpeditionTileMap map, ExpeditionTileMapPosition position)
    {
        if (map.TilesByPosition.TryGetValue(position, out ExpeditionTile? existingTile))
        {
            existingTile.Discovered = true;
            MarkTileChanged(map, position);
            return existingTile;
        }

        ExpeditionTile tile = new(position);
        map.TilesByPosition.Add(position, tile);
        MarkStructureChanged(map, position);
        return tile;
    }

    public bool TryUpdateCurrentTile(ExpeditionTileMap map, int x, int y)
    {
        ExpeditionTileMapPosition position = new(x, y);

        if (!map.TilesByPosition.TryGetValue(position, out ExpeditionTile? proposedTile) || !proposedTile.Discovered)
        {
            return false;
        }

        ExpeditionTileMapPosition? previousPosition = map.CurrentTile?.Position;
        map.CurrentTile = proposedTile;
        MarkTileChanged(map, position);

        if (previousPosition.HasValue && previousPosition.Value != position)
        {
            MarkTileChanged(map, previousPosition.Value);
        }

        if (proposedTile.Connected)
        {
            map.RecordedPath.Clear();
            map.RecordedPath.Add(proposedTile);
            MarkRecordedPathChanged(map);
            MarkTileChanged(map, position);
            return true;
        }

        int existingPathIndex = map.RecordedPath.IndexOf(proposedTile);
        if (existingPathIndex >= 0)
        {
            int removeStartIndex = existingPathIndex + 1;
            map.RecordedPath.RemoveRange(removeStartIndex, map.RecordedPath.Count - removeStartIndex);
            MarkRecordedPathChanged(map);
            return true;
        }

        map.RecordedPath.Add(proposedTile);
        MarkRecordedPathChanged(map);
        MarkTileChanged(map, position);
        return true;
    }

    public bool TryConnectTile(ExpeditionTileMap map, int x, int y, int parentX, int parentY)
    {
        ExpeditionTileMapPosition position = new(x, y);
        ExpeditionTileMapPosition parentPosition = new(parentX, parentY);

        if (position == parentPosition)
        {
            return false;
        }

        if (!map.TilesByPosition.TryGetValue(position, out ExpeditionTile? tile) || !tile.Discovered)
        {
            return false;
        }

        if (!map.TilesByPosition.TryGetValue(parentPosition, out ExpeditionTile? parentTile) || !parentTile.Discovered)
        {
            return false;
        }

        RemoveParentConnection(map, position);

        tile.Connected = true;
        map.ParentByPosition[position] = parentPosition;
        MarkParentConnectionsChanged(map);

        if (!map.ChildrenByPosition.TryGetValue(parentPosition, out HashSet<ExpeditionTileMapPosition>? children))
        {
            children = new HashSet<ExpeditionTileMapPosition>();
            map.ChildrenByPosition.Add(parentPosition, children);
        }

        children.Add(position);
        MarkChildrenConnectionsChanged(map);
        MarkTileChanged(map, position);
        MarkTileChanged(map, parentPosition);
        return true;
    }

    public bool TryRemoveRelay(ExpeditionTileMap map, int x, int y)
    {
        ExpeditionTileMapPosition relayPosition = new(x, y);

        if (!map.TilesByPosition.TryGetValue(relayPosition, out ExpeditionTile? relayTile) || !relayTile.Relay)
        {
            return false;
        }

        relayTile.Relay = false;
        MarkTileChanged(map, relayPosition);

        ExpeditionTileMapPosition currentPosition = relayPosition;
        while (map.TilesByPosition.TryGetValue(currentPosition, out ExpeditionTile? currentTile))
        {
            currentTile.Connected = false;
            MarkTileChanged(map, currentPosition);

            if (!map.ParentByPosition.TryGetValue(currentPosition, out ExpeditionTileMapPosition parentPosition))
            {
                RemoveParentConnection(map, currentPosition);
                break;
            }

            RemoveParentConnection(map, currentPosition);

            if (!map.TilesByPosition.TryGetValue(parentPosition, out ExpeditionTile? parentTile))
            {
                break;
            }

            MarkTileChanged(map, parentPosition);

            if (parentTile.Spine || parentTile.Relay || !map.ParentByPosition.ContainsKey(parentPosition) || HasChildren(map, parentPosition))
            {
                break;
            }

            currentPosition = parentPosition;
        }

        return true;
    }

    public void RerollMap(ExpeditionTileMap map)
    {
        foreach ((ExpeditionTileMapPosition position, ExpeditionTile tile) in map.TilesByPosition.ToArray())
        {
            if (tile.Connected)
            {
                continue;
            }

            if (tile.Suppressed)
            {
                tile.Discovered = false;
                tile.Rolled = false;
                MarkTileChanged(map, position);
                continue;
            }

            RemoveTile(map, position);
        }
    }

    public bool TryGetTile(ExpeditionTileMap map, ExpeditionTileMapPosition position, out ExpeditionTile? tile)
    {
        return map.TilesByPosition.TryGetValue(position, out tile);
    }

    public bool RemoveTile(ExpeditionTileMap map, ExpeditionTileMapPosition position)
    {
        if (!map.TilesByPosition.Remove(position))
        {
            return false;
        }

        RemoveParentConnection(map, position);

        if (map.ChildrenByPosition.Remove(position, out HashSet<ExpeditionTileMapPosition>? children))
        {
            foreach (ExpeditionTileMapPosition childPosition in children)
            {
                map.ParentByPosition.Remove(childPosition);
                MarkParentConnectionsChanged(map);
                MarkTileChanged(map, childPosition);
            }

            MarkChildrenConnectionsChanged(map);
        }

        if (map.CurrentTile?.Position == position)
        {
            map.CurrentTile = null;
        }

        if (map.RecordedPath.RemoveAll(tile => tile.Position == position) > 0)
        {
            MarkRecordedPathChanged(map);
        }

        MarkStructureChanged(map, position);
        return true;
    }

    private void RemoveParentConnection(ExpeditionTileMap map, ExpeditionTileMapPosition position)
    {
        if (!map.ParentByPosition.Remove(position, out ExpeditionTileMapPosition parentPosition))
        {
            return;
        }

        MarkParentConnectionsChanged(map);
        MarkTileChanged(map, position);
        MarkTileChanged(map, parentPosition);

        if (!map.ChildrenByPosition.TryGetValue(parentPosition, out HashSet<ExpeditionTileMapPosition>? children))
        {
            return;
        }

        children.Remove(position);
        MarkChildrenConnectionsChanged(map);

        if (children.Count == 0)
        {
            map.ChildrenByPosition.Remove(parentPosition);
        }
    }

    private bool HasChildren(ExpeditionTileMap map, ExpeditionTileMapPosition position)
    {
        return map.ChildrenByPosition.TryGetValue(position, out HashSet<ExpeditionTileMapPosition>? children) && children.Count > 0;
    }

    private void MarkStructureChanged(ExpeditionTileMap map, ExpeditionTileMapPosition position)
    {
        MarkTileChanged(map, position);
        map.StructureVersion = map.Version;
    }

    private void MarkTileChanged(ExpeditionTileMap map, ExpeditionTileMapPosition position)
    {
        map.TileVersion = IncrementVersion(map);
        map.DirtyPositions.Add(position);
    }

    private void MarkRecordedPathChanged(ExpeditionTileMap map)
    {
        map.RecordedPathVersion = IncrementVersion(map);
    }

    private void MarkParentConnectionsChanged(ExpeditionTileMap map)
    {
        map.ParentConnectionsVersion = IncrementVersion(map);
    }

    private void MarkChildrenConnectionsChanged(ExpeditionTileMap map)
    {
        map.ChildrenConnectionsVersion = IncrementVersion(map);
    }

    private long IncrementVersion(ExpeditionTileMap map)
    {
        map.Version++;
        return map.Version;
    }
}
