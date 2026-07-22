namespace Weapon;

public interface IWeapon
{
    float CurrentRecoil { get; set; }
    float RecoilPerShot { get; }
}
