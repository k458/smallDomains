namespace ExpeditionTileMap;

public class ExpeditionTileMap
{
    public Dictionary<ExpeditionTileMapPosition, ExpeditionTile> TilesByPosition { get; } = new();
    public Dictionary<ExpeditionTileMapPosition, ExpeditionTileMapPosition> ParentByPosition { get; } = new();
    public Dictionary<ExpeditionTileMapPosition, HashSet<ExpeditionTileMapPosition>> ChildrenByPosition { get; } = new();
    public List<ExpeditionTile> RecordedPath { get; } = new();

    public IReadOnlyCollection<ExpeditionTile> Tiles => TilesByPosition.Values;
    public ExpeditionTile? CurrentTile { get; set; }
}
