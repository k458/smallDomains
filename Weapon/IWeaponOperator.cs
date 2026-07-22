using System.Numerics;

namespace Weapon;

public interface IWeaponOperator
{
    Vector2 Position2D { get; }
    IWeaponOperatorProperties WeaponOperatorProperties { get; }
    IWeapon? Weapon { get; set; }
}
