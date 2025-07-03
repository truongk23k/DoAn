using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}

[System.Serializable] // visible in inspector
public class Weapon
{
    public WeaponType weaponType;

    public int bulletInMagazine;
    public int magaxineCapacity;
    public int totalReserveAmmo;

    [Range(1, 3)]
    public float reloadSpeed = 1;
    [Range(1, 3)]
    public float equipmentSpeed = 1;

    public bool CanShot()
    {
        return HaveEnoughBullets();
    }

    private bool HaveEnoughBullets()
    {
        if (bulletInMagazine > 0)
        {
            bulletInMagazine--;
            return true;
        }
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
}
