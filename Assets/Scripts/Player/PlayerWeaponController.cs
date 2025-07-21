using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public Weapon_Data dataWeaponStart;

    private Player player;

    [SerializeField] private Weapon currentWeapon;
    private bool weaponReady;
    private bool isEquip_NoShoot;

    private bool isShooting;

    [Header("Bullet details")]
    [SerializeField] float bulletImpactForce = 100;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int maxSlots = 2;
    [SerializeField] private List<Weapon> weaponSlots;

    [SerializeField] private GameObject weaponPickupPrefab;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();

        Invoke("EquipStartingWeapon", 0.1f);
    }

    private void Update()
    {
        if (isShooting)
            Shoot();

    }

    #region Slot management - Pickup/Equip/Drop/Ready
    private void EquipStartingWeapon()
    {
        PickupWeapon(new Weapon(dataWeaponStart));
        EquipWeapon(0);
    }

    private void EquipWeapon(int i)
    {
        if (weaponSlots.Count <= i)
            return;

        if (weaponSlots[i] == null)
            return;
        if (currentWeapon == weaponSlots[i])
            return;

        SetWeaponReady(false);

        currentWeapon = weaponSlots[i];

        isEquip_NoShoot = true;
        player.weaponVisuals.PlayWeaponEquipAnimation();

        CameraManager.instance.ChangeCameraDistance(currentWeapon.cameraDistance);
    }

    public void SetIsEquip(bool isEquip) => isEquip_NoShoot = isEquip;

    public void PickupWeapon(Weapon newWeapon)
    {
        //if inventory have this weapon => pickup only ammo
        Weapon weaponCheckName = WeaponByNameInSlots(newWeapon.weaponName);
        if (weaponCheckName != null)
        {
            weaponCheckName.totalReserveAmmo += newWeapon.totalReserveAmmo;
            return;
        }
        //if inventory have this type weapon => swap weapon
        Weapon weaponCheckType = WeaponByTypeInSlots(newWeapon.weaponType);
        if (weaponCheckType != null)
        {
            CreateWeaponOnGround(weaponCheckType);

            int weaponIndex = weaponSlots.IndexOf(weaponCheckType);

            player.weaponVisuals.SwitchOffWeaponModels();
            weaponSlots[weaponIndex] = newWeapon;
            EquipWeapon(weaponIndex);
            return;
        }
        //if inventory full => swap current weapon
        if (weaponSlots.Count >= maxSlots)
        {
            CreateWeaponOnGround(currentWeapon);

            int currentWeaponIndex = weaponSlots.IndexOf(currentWeapon);

            player.weaponVisuals.SwitchOffWeaponModels();
            weaponSlots[currentWeaponIndex] = newWeapon;
            EquipWeapon(currentWeaponIndex);
            return;
        }

        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnBackupWeaponModel();
    }

    private void DropCurrentWeapon()
    {
        if (HasOnlyOneWeapon())
            return;

        CreateWeaponOnGround(currentWeapon);

        weaponSlots.Remove(currentWeapon);

        EquipWeapon(0);
    }

    private void CreateWeaponOnGround(Weapon weapon)
    {
        GameObject droppedWeapon = ObjectPool.instance.GetObject(weaponPickupPrefab);
        droppedWeapon.GetComponent<Pickup_Weapon>().SetupPickupWeapon(weapon, transform);
    }

    public void SetWeaponReady(bool ready) => weaponReady = ready;

    public bool WeaponReady() => weaponReady;
    #endregion

    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);

        int numShoot = currentWeapon.bulletInMagazine >= currentWeapon.bulletsPerShoot ? currentWeapon.bulletsPerShoot : currentWeapon.bulletInMagazine;

        for (int i = 1; i <= numShoot; i++)
        {
            FireSingleBullet();

            yield return new WaitForSeconds(currentWeapon.burstFireDelay);

            if (i >= currentWeapon.bulletsPerShoot)
                SetWeaponReady(true);
        }
    }

    private void Shoot()
    {
        if (isEquip_NoShoot)
            return;

        if (!currentWeapon.CanShoot())
        {
            if (currentWeapon.CanReload() && currentWeapon.NeedAutoReload())
                Reload();

            return;
        }

        player.weaponVisuals.PlayFireAnimation();
        SetWeaponReady(true); //when equip i shoot

        if (currentWeapon.shotType == ShootType.Single)
            isShooting = false;

        if (currentWeapon.BurstActivated())
        {
            StartCoroutine(BurstFire());
            return;
        }

        FireSingleBullet();
    }

    private void FireSingleBullet()
    {
        currentWeapon.bulletInMagazine--;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);
        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());

        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.BulletSetup(currentWeapon.gunDistance, bulletsDirection, bulletSpeed, bulletImpactForce);

    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
    }

    public Vector3 BulletDirection()
    {
        //find a better place for it because it off when reload or grab => OK make it in PlayAim
        /* weaponHolder.LookAt(aim);
         gunPoint.LookAt(aim);*/
        Transform aim = player.aim.Aim();

        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (!player.aim.CanAimPrecisely() && player.aim.Target() == null)
            direction.y = 0;

        return direction;
    }

    private bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;

    public Weapon WeaponByNameInSlots(string weaponName)
    {
        foreach (Weapon weapon in weaponSlots)
            if (weapon.weaponName == weaponName)
                return weapon;

        return null;
    }

    public Weapon WeaponByTypeInSlots(WeaponType weaponType)
    {
        foreach (Weapon weapon in weaponSlots)
            if (weapon.weaponType == weaponType)
                return weapon;

        return null;
    }

    public Weapon CurrentWeapon() => currentWeapon;

    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint;

    #region Input Events
    private void AssignInputEvents()
    {
        PlayerControlls controls = player.controls;

        controls.Character.Fire.performed += context => isShooting = true;
        controls.Character.Fire.canceled += context => isShooting = false;

        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
        controls.Character.EquipSlot3.performed += context => EquipWeapon(2);
        controls.Character.EquipSlot4.performed += context => EquipWeapon(3);
        controls.Character.EquipSlot5.performed += context => EquipWeapon(4);

        controls.Character.DropCurrentWeapon.performed += context => DropCurrentWeapon();

        controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };
        controls.Character.ToogleWeaponMode.performed += context => currentWeapon.ToggleBurst();
    }

    #endregion

    /*private void OnDrawGizmos()
    {
        Gizmos.DrawLine(weaponHolder.position, weaponHolder.position + weaponHolder.forward * 25);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(gunPoint.position, gunPoint.position + BulletDirection() * 25);
    }*/
}
