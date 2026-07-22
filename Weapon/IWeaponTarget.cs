using System.Numerics;

namespace Weapon;

public interface IWeaponTarget
{
    Vector2 Position2D { get; }
    IWeaponTargetProperties WeaponTargetProperties { get; }
}
