namespace ItemContainers;

public interface IEquipment : IItem
{
    EquipmentType EquipmentType { get; }
    int Size { get; }
}
