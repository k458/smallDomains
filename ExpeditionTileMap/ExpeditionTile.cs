namespace ExpeditionTileMap;

public class ExpeditionTile
{
    public ExpeditionTile(ExpeditionTileMapPosition position)
    {
        Position = position;
        Discovered = true;
    }

    public ExpeditionTileMapPosition Position { get; }
    public ExpeditionEncounter? Encounter { get; set; }
    public bool Connected { get; set; }
    public bool Stabilized { get; set; }
    public bool Suppressed { get; set; }
    public bool Discovered { get; set; }
    public bool Spine { get; set; }
    public bool Relay { get; set; }
    public bool Rolled { get; set; }
}
