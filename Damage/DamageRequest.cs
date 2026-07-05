namespace Damage;

public class DamageRequest
{
    public DamageRequest(IDamageable target, DamageType damageType, int amount)
    {
        Target = target;
        DamageType = damageType;
        Amount = amount;
    }

    public IDamageable Target { get; }

    public DamageType DamageType { get; }

    public int Amount { get; }
}
