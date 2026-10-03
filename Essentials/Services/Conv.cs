namespace Essentials.Services;

public class Conv
{
    public int Dir8ToInt(Dir8 dir)
    {
        return dir switch
        {
            Dir8.NE => 0,
            Dir8.E => 1,
            Dir8.SE => 2,
            Dir8.S => 3,
            Dir8.SW => 4,
            Dir8.W => 5,
            Dir8.NW => 6,
            Dir8.N => 7,
            _ => -1
        };
    }

    public bool IntToDir8(int value, out Dir8 dir)
    {
        dir = default;

        if (value < 0 || value > 7)
        {
            return false;
        }

        dir = value switch
        {
            0 => Dir8.NE,
            1 => Dir8.E,
            2 => Dir8.SE,
            3 => Dir8.S,
            4 => Dir8.SW,
            5 => Dir8.W,
            6 => Dir8.NW,
            7 => Dir8.N,
            _ => default
        };

        return true;
    }
}
