namespace Damage;

public class DamageResistances
{
    public int Physical { get; set; } = 100;

    public int Cold { get; set; } = 100;

    public int Fire { get; set; } = 100;

    public int Lightning { get; set; } = 100;

    public int GetResistance(DamageType damageType)
    {
        return damageType switch
        {
            DamageType.Physical => Physical,
            DamageType.Cold => Cold,
            DamageType.Fire => Fire,
            DamageType.Lightning => Lightning,
            _ => 100
        };
    }
}
