namespace ArmorPenetration;

public class ArmorPenetrationService
{
    public float ArmorScalingConstant { get; set; } = 0.01f;
    public float ArmorContestExponent { get; set; } = 3f;

    public ArmorPenetrationResult Calculate(float armor, float armorPenetration)
    {
        float effectiveArmor = MathF.Max(0f, armor - armorPenetration);
        float damageMultiplier = 1f / (1f + effectiveArmor * ArmorScalingConstant);

        return new ArmorPenetrationResult(effectiveArmor, damageMultiplier);
    }

    public ArmorPenetrationContestResult CalculateArmorContest(float armor, float armorPenetration)
    {
        float armorStrength = MathF.Pow(1f + armor * ArmorScalingConstant, ArmorContestExponent);
        float penetrationStrength = MathF.Pow(1f + armorPenetration * ArmorScalingConstant, ArmorContestExponent);
        float armorContestMultiplier = armorStrength / (armorStrength + penetrationStrength);

        return new ArmorPenetrationContestResult(armorStrength, penetrationStrength, armorContestMultiplier);
    }
}
