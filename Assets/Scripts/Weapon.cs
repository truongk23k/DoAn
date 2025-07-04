using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}

public enum ShootType
{
    Single,
    Auto
}

[System.Serializable] // visible in inspector
public class Weapon
{
    public WeaponType weaponType;

    [Header("Shooting specifics")]
    public ShootType shotType;
    public float fireRate = 1; // bullet per second
    private float lastShootTime;

    [Header("Magazine details")]
    public int bulletInMagazine;
    public int magaxineCapacity;
    public int totalReserveAmmo;

    [Range(1, 3)]
    public float reloadSpeed = 1;
    [Range(1, 3)]
    public float equipmentSpeed = 1;

    public bool CanShoot()
    {
        if (HaveEnoughBullets() && ReadyToFire())
        {
            bulletInMagazine--;
            return true;
        }
        return false;
    }

    private bool ReadyToFire()
    {
        if (Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time;
            return true;
        }

        return false;
    }


    #region Reload methods
    private bool HaveEnoughBullets() => bulletInMagazine > 0;

    public bool NeedAutoReload()
    {
        if (ReadyToFire() && !HaveEnoughBullets())
            return true;

        return false;
    }

    public bool CanReload()
    {
        if (bulletInMagazine == magaxineCapacity)
            return false;

        if (totalReserveAmmo > 0)
        {
            return true;
        }
        //play sfx
        return false;
    }

    public void RefillBullets()
    {
        totalReserveAmmo += bulletInMagazine;

        int bulletsToReload = magaxineCapacity;

        if (bulletsToReload > totalReserveAmmo)
            bulletsToReload = totalReserveAmmo;

        totalReserveAmmo -= bulletsToReload;
        bulletInMagazine = bulletsToReload;

        if (totalReserveAmmo < 0)
            totalReserveAmmo = 0;
    }
    #endregion
}
