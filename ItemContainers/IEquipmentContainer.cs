using System.Collections.Generic;

namespace ItemContainers;

public interface IEquipmentContainer
{
    List<IEquipmentSlot> Slots { get; }
    List<IWeapon> Weapons { get; }

    bool TryAddEquipment(IEquipment equipment);
    bool TryAddEquipment(IEquipment equipment, IEquipmentSlot slot);
    bool TryRemoveEquipment(IEquipment equipment);
}
