namespace Weapon;

public class Weapon : IWeapon
{
    public float CurrentRecoil { get; set; }
    public float RecoilPerShot { get; set; }
    public float RecoilRetentionMultiplier { get; set; } = 0.9f;
    public float RecoilZeroThreshold { get; set; } = 0.01f;

    public void UpdateWeapon(IWeaponOperator weaponOperator, float deltaTime)
    {
        _ = weaponOperator;
        _ = deltaTime;
    }

    public void UpdateWeaponFixed(IWeaponOperator weaponOperator)
    {
        _ = weaponOperator;

        if (CurrentRecoil <= 0f)
        {
            CurrentRecoil = 0f;
            return;
        }

        CurrentRecoil *= RecoilRetentionMultiplier;

        if (CurrentRecoil < RecoilZeroThreshold)
        {
            CurrentRecoil = 0f;
        }
    }
}

