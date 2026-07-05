namespace Orders;

public class OrderOrchestrator
{
    private readonly List<Order> orders = [];

    public IReadOnlyCollection<Order> Orders => orders;

    public void PushOrder(Order order)
    {
        Order? existingOrder = FindCompoundableOrder(order);

        if (existingOrder is not null)
        {
            existingOrder.Compound();
            return;
        }

        orders.Add(order);
    }

    public bool RemoveOrder(Order order)
    {
        return orders.Remove(order);
    }

    public void ClearOrders()
    {
        orders.Clear();
    }

    private Order? FindCompoundableOrder(Order order)
    {
        foreach (Order existingOrder in orders)
        {
            if (existingOrder.Id == order.Id && existingOrder.CanCompound)
            {
                return existingOrder;
            }
        }

        return null;
    }
}
