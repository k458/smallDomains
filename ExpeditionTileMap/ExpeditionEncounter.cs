using Shared;

namespace ExpeditionTileMap;

public class ExpeditionEncounter
{
    public ExpeditionEncounter(V2I position)
    {
        Position = position;
    }

    public V2I Position { get; }
}

