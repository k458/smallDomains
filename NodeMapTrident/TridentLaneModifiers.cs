namespace NodeMapTrident;

public readonly struct TridentLaneModifiers
{
    public TridentLaneModifiers(
        double rareEncounterMultiplier,
        double rewardVarianceMultiplier,
        double dangerMultiplier)
    {
        RareEncounterMultiplier = rareEncounterMultiplier;
        RewardVarianceMultiplier = rewardVarianceMultiplier;
        DangerMultiplier = dangerMultiplier;
    }

    public double RareEncounterMultiplier { get; }
    public double RewardVarianceMultiplier { get; }
    public double DangerMultiplier { get; }
}
