namespace ArmorPenetration;

public readonly struct ArmorPenetrationContestResult
{
    public ArmorPenetrationContestResult(float armorStrength, float penetrationStrength, float armorContestMultiplier)
    {
        ArmorStrength = armorStrength;
        PenetrationStrength = penetrationStrength;
        ArmorContestMultiplier = armorContestMultiplier;
    }

    public float ArmorStrength { get; }
    public float PenetrationStrength { get; }
    public float ArmorContestMultiplier { get; }
}
