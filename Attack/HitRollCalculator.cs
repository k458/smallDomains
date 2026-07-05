using System;

namespace Attack;

public static class HitRollCalculator
{
    public static HitRollResult RollHit(
        IAttackerStats attacker,
        IDefenderStats defender,
        float distance,
        WeaponRangeConfiguration weaponRangeConfiguration)
    {
        if (distance > weaponRangeConfiguration.maxRange)
        {
            return new HitRollResult { IsHit = false };
        }

        float accuracyMultiplier = GetAccuracyMultiplier(
            distance,
            weaponRangeConfiguration);
        float baseHitChance = Math.Max(attacker.Accuracy - defender.Dodge, 10) / 100f;
        float hitChance = baseHitChance * accuracyMultiplier;
        float roll = Random.Shared.NextSingle();

        return new HitRollResult
        {
            IsHit = roll < hitChance
        };
    }

    private static float GetAccuracyMultiplier(
        float distance,
        WeaponRangeConfiguration weaponRangeConfiguration)
    {
        if (distance <= weaponRangeConfiguration.minRange)
        {
            return weaponRangeConfiguration.minRangeAccuracyMult;
        }

        if (distance <= weaponRangeConfiguration.optimalRange)
        {
            return InterpolateMultiplier(
                distance,
                weaponRangeConfiguration.minRange,
                weaponRangeConfiguration.optimalRange,
                weaponRangeConfiguration.minRangeAccuracyMult,
                weaponRangeConfiguration.optimalRangeAccuracyMult);
        }

        return InterpolateMultiplier(
            distance,
            weaponRangeConfiguration.optimalRange,
            weaponRangeConfiguration.maxRange,
            weaponRangeConfiguration.optimalRangeAccuracyMult,
            weaponRangeConfiguration.maxRangeAccuracyMult);
    }

    private static float InterpolateMultiplier(
        float distance,
        float startRange,
        float endRange,
        float startMultiplier,
        float endMultiplier)
    {
        if (endRange <= startRange)
        {
            return endMultiplier;
        }

        float rangeProgress = (distance - startRange) / (endRange - startRange);
        return startMultiplier + (endMultiplier - startMultiplier) * rangeProgress;
    }
}
