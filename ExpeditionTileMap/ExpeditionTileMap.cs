namespace ExpeditionTileMap;

public class ExpeditionTileMap
{
    public Dictionary<ExpeditionTileMapPosition, ExpeditionTile> TilesByPosition { get; } = new();
    public Dictionary<ExpeditionTileMapPosition, ExpeditionTileMapPosition> ParentByPosition { get; } = new();
    public Dictionary<ExpeditionTileMapPosition, HashSet<ExpeditionTileMapPosition>> ChildrenByPosition { get; } = new();
    public HashSet<ExpeditionTileMapPosition> DirtyPositions { get; } = new();
    public List<ExpeditionTile> RecordedPath { get; } = new();

    public IReadOnlyCollection<ExpeditionTile> Tiles => TilesByPosition.Values;
    public ExpeditionTile? CurrentTile { get; set; }
    public long Version { get; set; }
    public long StructureVersion { get; set; }
    public long TileVersion { get; set; }
    public long RecordedPathVersion { get; set; }
    public long ParentConnectionsVersion { get; set; }
    public long ChildrenConnectionsVersion { get; set; }
}
