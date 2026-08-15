namespace ExpeditionTileMap;

public class ExpeditionWire
{
    public bool Up { get; set; }
    public bool Right { get; set; }
    public bool Down { get; set; }
    public bool Left { get; set; }

    public bool HasAnyWire => Up || Right || Down || Left;

    public bool Connected { get; set; }
}

