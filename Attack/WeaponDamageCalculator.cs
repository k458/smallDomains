namespace Attack;

public class WeaponDamageCalculator
{
    public void CalculateDamage(
        float distance,
        WeaponDamageConfiguration weaponDamage,
        IDefenderStats defender,
        out DamageCalculationResult result)
    {
        _ = distance;

        int hpBeforeHit = System.Math.Max(defender.Hp, 0);
        int actualDamage = System.Math.Min(
            System.Math.Max(weaponDamage.damage, 0),
            hpBeforeHit);

        result = new DamageCalculationResult { damageDealt = actualDamage };
    }
}
