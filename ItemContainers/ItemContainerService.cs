using System;

namespace ItemContainers;

public static class ItemContainerService
{
    public static bool TryAddItem(IItem item, IItemContainer container)
    {
        if (!CanAddItem(item, container))
        {
            return false;
        }

        return container.TryAddItem(item);
    }

    public static bool CanAddItem(IItem item, IItemContainer container)
    {
        long totalVolume = item.Volume;
        long totalWeight = item.Weight;

        foreach (IItem storedItem in container.Items)
        {
            totalVolume += storedItem.Volume;
            totalWeight += storedItem.Weight;
        }

        if (container.LimitedByVolume && totalVolume > container.VolumeLimit)
        {
            return false;
        }

        if (container.LimitedByWeight && totalWeight > container.WeightLimit)
        {
            return false;
        }

        return true;
    }

    public static int GetTotalVolume(IItemContainer container)
    {
        int totalVolume = 0;

        foreach (IItem item in container.Items)
        {
            totalVolume += item.Volume;
        }

        return totalVolume;
    }

    public static int GetTotalWeight(IItemContainer container)
    {
        int totalWeight = 0;

        foreach (IItem item in container.Items)
        {
            totalWeight += item.Weight;
        }

        return totalWeight;
    }

    public static int GetTotalVolume(IEquipmentContainer container)
    {
        int totalVolume = 0;

        foreach (IEquipmentSlot slot in container.Slots)
        {
            if (slot.Equipment is not null)
            {
                totalVolume += slot.Equipment.Volume;
            }
        }

        return totalVolume;
    }

    public static int GetTotalWeight(IEquipmentContainer container)
    {
        int totalWeight = 0;

        foreach (IEquipmentSlot slot in container.Slots)
        {
            if (slot.Equipment is not null)
            {
                totalWeight += slot.Equipment.Weight;
            }
        }

        return totalWeight;
    }

    public static IEquipmentSlot? SelectRandomSlot(IEquipmentContainer container)
    {
        return SelectRandomSlot(container, Random.Shared);
    }

    public static IEquipmentSlot? SelectRandomSlot(
        IEquipmentContainer container,
        Random random)
    {
        long totalWeight = 0;

        foreach (IEquipmentSlot slot in container.Slots)
        {
            if (slot.Size > 0)
            {
                totalWeight += slot.Size;
            }
        }

        if (totalWeight == 0)
        {
            return null;
        }

        double selection = random.NextDouble() * totalWeight;
        long cumulativeWeight = 0;

        foreach (IEquipmentSlot slot in container.Slots)
        {
            if (slot.Size <= 0)
            {
                continue;
            }

            cumulativeWeight += slot.Size;

            if (selection < cumulativeWeight)
            {
                return slot;
            }
        }

        return null;
    }

    public static bool TryAddEquipment(
        IEquipment equipment,
        IEquipmentContainer container)
    {
        foreach (IEquipmentSlot slot in container.Slots)
        {
            if (CanAddEquipment(equipment, slot, container)
                && container.TryAddEquipment(equipment, slot))
            {
                return true;
            }
        }

        return false;
    }

    public static bool TryAddEquipment(
        IEquipment equipment,
        IEquipmentSlot slot,
        IEquipmentContainer container)
    {
        if (!CanAddEquipment(equipment, slot, container))
        {
            return false;
        }

        return container.TryAddEquipment(equipment, slot);
    }

    public static bool CanAddEquipment(
        IEquipment equipment,
        IEquipmentContainer container)
    {
        foreach (IEquipmentSlot slot in container.Slots)
        {
            if (CanAddEquipment(equipment, slot, container))
            {
                return true;
            }
        }

        return false;
    }

    public static bool CanAddEquipment(
        IEquipment equipment,
        IEquipmentSlot slot,
        IEquipmentContainer container)
    {
        return container.Slots.Contains(slot)
            && slot.Equipment is null
            && slot.EquipmentType == equipment.EquipmentType
            && slot.Size >= equipment.Size;
    }
}
