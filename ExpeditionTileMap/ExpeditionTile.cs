using Shared;

namespace ExpeditionTileMap;

public class ExpeditionTile
{
    public ExpeditionTile(V2I position)
    {
        Position = position;
    }

    public V2I Position { get; }
    public ExpeditionTileTerrainType TerrainType { get; set; }
}

