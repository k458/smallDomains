namespace ArenaPositioning;

public class ArenaPosition
{
    internal ArenaPosition(int rankIndex)
    {
        RankIndex = rankIndex;
    }

    public int RankIndex { get; }

    public IArenaPositionOccupant? Occupant { get; internal set; }

    public bool IsOccupied => Occupant is not null;
}
