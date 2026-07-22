namespace Weapon;

public readonly struct WeaponShotResult
{
    public WeaponShotResult(
        WeaponShotEndResult endResult,
        float recoilFactor,
        float distanceFactor,
        float recoilHitChance,
        float accuracyHitChance)
    {
        EndResult = endResult;
        RecoilFactor = recoilFactor;
        DistanceFactor = distanceFactor;
        RecoilHitChance = recoilHitChance;
        AccuracyHitChance = accuracyHitChance;
    }

    public WeaponShotEndResult EndResult { get; }
    public float RecoilFactor { get; }
    public float DistanceFactor { get; }
    public float RecoilHitChance { get; }
    public float AccuracyHitChance { get; }
}
