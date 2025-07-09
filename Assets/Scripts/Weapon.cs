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

    #region Regular mode variables
    public ShootType shotType;
    public int bulletsPerShoot { get; private set; }

    private float defaultFireRate = 1;
    public float fireRate = 1; // shoot per second
    private float lastShootTime;
    #endregion

    #region Burst mode variables
    public bool burstAvalible;
    public bool burstActive;

    private int burstBulletsPerShoot;
    private float burstFireRate;
    public float burstFireDelay { get; private set; }
    #endregion

    [Header("Magazine details")]
    public int bulletInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    #region Weapon generic info variables
    public float reloadSpeed { get; private set; }
    public float equipmentSpeed { get; private set; }
    public float gunDistance { get; private set; }
    public float cameraDistance { get; private set; }
    #endregion

    #region Weapon spread variables
    [Header("Spread")]
    private float baseSpread = 1;
    private float maximumSpread = 3;
    private float currentSpread;

    private float spreadIncreaseRate = 0.15f;
    private float spreadCooldown;

    private float lastSpreadUpdateTime;
    #endregion

    public Weapon(Weapon_Data weaponData)
    {
        bulletInMagazine = weaponData.bulletInMagazine;
        magazineCapacity = weaponData.magazineCapacity;
        totalReserveAmmo = weaponData.totalReserveAmmo;

        weaponType = weaponData.weaponType;
        fireRate = weaponData.fireRate;
        shotType = weaponData.shootType;
        bulletsPerShoot = weaponData.bulletPerShoot;
        //burst
        burstAvalible = weaponData.burstAvalible;
        burstActive = weaponData.burstActive;
        burstBulletsPerShoot = weaponData.burstBulletsPerShoot;
        burstFireRate = weaponData.burstFireRate;
        burstFireDelay = weaponData.burstFireDelay;
        //spread
        baseSpread = weaponData.baseSpread;
        maximumSpread = weaponData.maxSpread;
        spreadIncreaseRate = weaponData.spreadIncreaseRate;
        spreadCooldown = weaponData.spreadCooldown;
        //spesifics
        reloadSpeed = weaponData.reloadSpeed;
        equipmentSpeed = weaponData.equipmentSpeed;
        gunDistance = weaponData.gunDistance;
        cameraDistance = weaponData.cameraDistance;

        defaultFireRate = fireRate;
    }

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
            ApplyBurst();
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
            ApplyBurst();
        }
        else
        {
            bulletsPerShoot = 1;
            fireRate = defaultFireRate;
        }
    }

    private void ApplyBurst()
    {
        bulletsPerShoot = burstBulletsPerShoot;
        fireRate = burstFireRate;
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
        if (bulletInMagazine == magazineCapacity)
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

        int bulletsToReload = magazineCapacity;

        if (bulletsToReload > totalReserveAmmo)
            bulletsToReload = totalReserveAmmo;

        totalReserveAmmo -= bulletsToReload;
        bulletInMagazine = bulletsToReload;

        if (totalReserveAmmo < 0)
            totalReserveAmmo = 0;
    }
    #endregion
}
