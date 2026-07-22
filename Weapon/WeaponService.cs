using System;
using System.Numerics;

namespace Weapon;

public class WeaponService
{
    public float DistanceEffectMultiplier { get; set; } = 0.1f;

    public bool TryShoot(
        IWeapon weapon,
        IWeaponOperator weaponOperator,
        IWeaponTarget weaponTarget,
        out WeaponShotResult result)
    {
        float distance = Vector2.Distance(
            weaponOperator.Position2D,
            weaponTarget.Position2D);
        float recoilFactor = 1f / (1f + weapon.CurrentRecoil);
        float distanceFactor = 1f / (1f + distance * DistanceEffectMultiplier);
        float recoilHitChance = Math.Clamp(recoilFactor * distanceFactor, 0f, 1f);
        float recoilRoll = Random.Shared.NextSingle();
        float recoilIncrease = weapon.RecoilPerShot
            / (1f + weaponOperator.WeaponOperatorProperties.RecoilControl);

        weapon.CurrentRecoil += recoilIncrease;

        if (recoilRoll > recoilHitChance)
        {
            result = new WeaponShotResult(
                WeaponShotEndResult.MissDueToRecoil,
                recoilFactor,
                distanceFactor,
                recoilHitChance,
                0f);

            return true;
        }

        int accuracy = weaponOperator.WeaponOperatorProperties.Accuracy;
        int dodge = weaponTarget.WeaponTargetProperties.Dodge;
        int accuracyAndDodge = accuracy + dodge;
        float accuracyHitChance = accuracyAndDodge <= 0
            ? 0f
            : Math.Clamp((float)accuracy / accuracyAndDodge * distanceFactor, 0f, 1f);
        float dodgeRoll = Random.Shared.NextSingle();

        result = new WeaponShotResult(
            dodgeRoll <= accuracyHitChance
                ? WeaponShotEndResult.Hit
                : WeaponShotEndResult.MissDueToEnemyDodge,
            recoilFactor,
            distanceFactor,
            recoilHitChance,
            accuracyHitChance);

        return true;
    }
}
