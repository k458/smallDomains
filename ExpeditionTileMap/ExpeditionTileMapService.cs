namespace ExpeditionTileMap;

public class ExpeditionTileMapService
{
    public ExpeditionTile DiscoverTile(ExpeditionTileMap map, ExpeditionTileMapPosition position)
    {
        if (map.TilesByPosition.TryGetValue(position, out ExpeditionTile? existingTile))
        {
            existingTile.Discovered = true;
            return existingTile;
        }

        ExpeditionTile tile = new(position);
        map.TilesByPosition.Add(position, tile);
        return tile;
    }

    public bool TryUpdateCurrentTile(ExpeditionTileMap map, int x, int y)
    {
        ExpeditionTileMapPosition position = new(x, y);

        if (!map.TilesByPosition.TryGetValue(position, out ExpeditionTile? proposedTile) || !proposedTile.Discovered)
        {
            return false;
        }

        map.CurrentTile = proposedTile;

        if (proposedTile.Connected)
        {
            map.RecordedPath.Clear();
            map.RecordedPath.Add(proposedTile);
            return true;
        }

        int existingPathIndex = map.RecordedPath.IndexOf(proposedTile);
        if (existingPathIndex >= 0)
        {
            int removeStartIndex = existingPathIndex + 1;
            map.RecordedPath.RemoveRange(removeStartIndex, map.RecordedPath.Count - removeStartIndex);
            return true;
        }

        map.RecordedPath.Add(proposedTile);
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

        if (!map.ChildrenByPosition.TryGetValue(parentPosition, out HashSet<ExpeditionTileMapPosition>? children))
        {
            children = new HashSet<ExpeditionTileMapPosition>();
            map.ChildrenByPosition.Add(parentPosition, children);
        }

        children.Add(position);
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

        ExpeditionTileMapPosition currentPosition = relayPosition;
        while (map.TilesByPosition.TryGetValue(currentPosition, out ExpeditionTile? currentTile))
        {
            currentTile.Connected = false;

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
            }
        }

        return true;
    }

    private void RemoveParentConnection(ExpeditionTileMap map, ExpeditionTileMapPosition position)
    {
        if (!map.ParentByPosition.Remove(position, out ExpeditionTileMapPosition parentPosition))
        {
            return;
        }

        if (!map.ChildrenByPosition.TryGetValue(parentPosition, out HashSet<ExpeditionTileMapPosition>? children))
        {
            return;
        }

        children.Remove(position);

        if (children.Count == 0)
        {
            map.ChildrenByPosition.Remove(parentPosition);
        }
    }

    private bool HasChildren(ExpeditionTileMap map, ExpeditionTileMapPosition position)
    {
        return map.ChildrenByPosition.TryGetValue(position, out HashSet<ExpeditionTileMapPosition>? children) && children.Count > 0;
    }
}
