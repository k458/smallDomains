namespace Damage;

public class DamageSystem
{
    public DamageResult ApplyDamage(DamageRequest request)
    {
        int resistance = request.Target.Resistances.GetResistance(request.DamageType);
        int finalAmount = CalculateDamage(request.Amount, resistance);

        request.Target.ApplyResolvedDamage(finalAmount);

        return new DamageResult(
            request.DamageType,
            request.Amount,
            resistance,
            finalAmount);
    }

    public int CalculateDamage(int amount, int resistance)
    {
        if (resistance <= 0)
        {
            return amount;
        }

        return amount * 100 / resistance;
    }
}
