namespace ItemContainers;

public interface IWeapon : IEquipment
{
    float DesiredRange { get; }
}
