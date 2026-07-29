namespace ArmorPenetration;

public class ArmorPenetrationService
{
    public float ArmorScalingConstant { get; set; } = 0.01f;

    public ArmorPenetrationResult Calculate(float armor, float armorPenetration)
    {
        float effectiveArmor = MathF.Max(0f, armor - armorPenetration);
        float damageMultiplier = 1f / (1f + effectiveArmor * ArmorScalingConstant);

        return new ArmorPenetrationResult(effectiveArmor, damageMultiplier);
    }
}
