namespace Damage;

public class DamageResult
{
    public DamageResult(DamageType damageType, int baseAmount, int resistance, int finalAmount)
    {
        DamageType = damageType;
        BaseAmount = baseAmount;
        Resistance = resistance;
        FinalAmount = finalAmount;
    }

    public DamageType DamageType { get; }

    public int BaseAmount { get; }

    public int Resistance { get; }

    public int FinalAmount { get; }
}
