using Shared;

namespace ExpeditionTileMap;

public class ExpeditionRelay
{
    public ExpeditionRelay(V2I position)
    {
        Position = position;
    }

    public V2I Position { get; }
    public bool Connected { get; set; }
    public bool Powered { get; set; }
}
