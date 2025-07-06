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
    public int bulletsPerShoot;
    public float defaultFireRate = 1;
    public float fireRate = 1; // shoot per second
    private float lastShootTime;

    [Header("Magazine details")]
    public int bulletInMagazine;
    public int magaxineCapacity;
    public int totalReserveAmmo;

    [Range(1, 3)]
    public float reloadSpeed = 1;
    [Range(1, 3)]
    public float equipmentSpeed = 1;
    [Range(2, 12)]
    public float gunDistance = 4;

    [Header("Spread")]
    public float baseSpread = 1;
    public float maximumSpread = 3;
    public float currentSpread;

    public float spreadIncreaseRate = 0.15f;

    private float lastSpreadUpdateTime;
    public float spreadCooldown;

    [Header("Burst fire")]
    public bool burstAvalible;
    public bool burstActive;

    public int burstBulletsPerShoot;
    public float burstFireRate;
    public float burstFireDelay;


    #region Spread methods
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-currentSpread, currentSpread);

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

        return spreadRotation * originalDirection;
    }

    private void UpdateSpread()
    {
        if (Time.time - lastSpreadUpdateTime > spreadCooldown)
            currentSpread = baseSpread;
        else
            IncreaseSpread();

        lastSpreadUpdateTime = Time.time;
    }

    private void IncreaseSpread()
    {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }
    #endregion

    #region Burst methods
    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            burstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    public void ToggleBurst()
    {
        if (!burstAvalible)
            return;
        burstActive = !burstActive;

        if (burstActive)
        {
            bulletsPerShoot = burstBulletsPerShoot;
            fireRate = burstFireRate;
        }
        else
        {
            bulletsPerShoot = 1;
            fireRate = defaultFireRate;
        }
    }
    #endregion

    public bool CanShoot()
    {
        if (HaveEnoughBullets() && ReadyToFire())
            return true;

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
