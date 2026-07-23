namespace Weapon;

public interface IWeapon
{
    float CurrentRecoil { get; set; }
    float RecoilPerShot { get; }

    void UpdateWeapon(IWeaponOperator weaponOperator, float deltaTime);
    void UpdateWeaponFixed(IWeaponOperator weaponOperator);
}
