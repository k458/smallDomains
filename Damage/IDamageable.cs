namespace Damage;

public interface IDamageable
{
    DamageResistances Resistances { get; }

    void ApplyResolvedDamage(int amount);
}
