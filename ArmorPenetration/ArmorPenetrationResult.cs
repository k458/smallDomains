namespace ArmorPenetration;

public readonly struct ArmorPenetrationResult
{
    public ArmorPenetrationResult(float effectiveArmor, float damageMultiplier)
    {
        EffectiveArmor = effectiveArmor;
        DamageMultiplier = damageMultiplier;
    }

    public float EffectiveArmor { get; }
    public float DamageMultiplier { get; }
}
