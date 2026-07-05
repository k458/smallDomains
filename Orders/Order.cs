namespace Orders;

public class Order
{
    private readonly int initialBasicCost;

    public Order(string id, int basicCost, int compoundCost, bool canCompound)
    {
        Id = id;
        initialBasicCost = basicCost;
        BasicCost = basicCost;
        CompoundCost = compoundCost;
        CanCompound = canCompound;
    }

    public string Id { get; }

    public int BasicCost { get; private set; }

    public int CompoundCost { get; }

    public int CompoundCounter { get; private set; }

    public bool CanCompound { get; }

    public void Compound()
    {
        if (!CanCompound)
        {
            return;
        }

        CompoundCounter++;
        BasicCost += CompoundCost;
    }

    public void Reset()
    {
        BasicCost = initialBasicCost;
        CompoundCounter = 0;
    }
}
