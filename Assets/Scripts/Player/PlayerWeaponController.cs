using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
    //default bullet speed
    private const float REFERENCE_BULLET_SPEED = 20f;

    [SerializeField] private Weapon currentWeapon;
    private bool weaponReady;
    private bool isEquip_NoShoot;

    private bool isShooting;

    [Header("Bullet details")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int maxSlots = 2;
    [SerializeField] private List<Weapon> weaponSlots;

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
    private void EquipStartingWeapon() => EquipWeapon(0);

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
    }

    public void SetIsEquip(bool isEquip) => isEquip_NoShoot = isEquip;

    public void PickupWeapon(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
        {
            Debug.Log("No slots avalible");
            return;
        }

        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnBackupWeaponModel();
    }

    private void DropWeapon()
    {
        if (HasOnlyOneWeapon())
            return;

        weaponSlots.Remove(currentWeapon);

        EquipWeapon(0);
    }

    public void SetWeaponReady(bool ready) => weaponReady = ready;

    public bool WeaponReady() => weaponReady;
    #endregion

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

        if (currentWeapon.shotType == ShootType.Single)
            isShooting = false;

        SetWeaponReady(true);

        //GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));
        GameObject newBullet = ObjectPool.instance.GetBullet();
        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = bulletsDirection * bulletSpeed;

        player.weaponVisuals.PlayFireAnimation();
        //destroy after 10s in Bullet
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

    public Weapon CurrentWeapon => currentWeapon;

    public Weapon BackupWeapon()
    {
        foreach (Weapon weapon in weaponSlots)
            if (weapon != currentWeapon)
                return weapon;

        return null;
    }

    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint;

    #region Input Events
    private void AssignInputEvents()
    {
        PlayerControlls controls = player.controls;

        controls.Character.Fire.performed += context => isShooting = true;
        controls.Character.Fire.canceled += context => isShooting = false;

        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);

        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };
    }

    #endregion

    /*private void OnDrawGizmos()
    {
        Gizmos.DrawLine(weaponHolder.position, weaponHolder.position + weaponHolder.forward * 25);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(gunPoint.position, gunPoint.position + BulletDirection() * 25);
    }*/
}
