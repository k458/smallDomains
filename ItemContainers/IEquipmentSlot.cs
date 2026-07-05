namespace ItemContainers;

public interface IEquipmentSlot
{
    EquipmentType EquipmentType { get; }
    int Size { get; }
    IEquipment Equipment { get; }
}
