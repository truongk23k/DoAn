using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
    //default bullet speed
    private const float REFERENCE_BULLET_SPEED = 20f;

    [SerializeField] private Weapon currentWeapon;

    [Header("Bullet details")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed;
    [SerializeField] Transform gunPoint;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int maxSlots = 2;
    [SerializeField] private List<Weapon> weaponSlots;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();

        currentWeapon.bulletInMagazine = currentWeapon.totalReserveAmmo;
    }

    #region Slot management - Pickup/Equip/Drop
    private void EquipWeapon(int i)
    {
        currentWeapon = weaponSlots[i];
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
        {
            Debug.Log("No slots avalible");
            return;
        }

        weaponSlots.Add(newWeapon);
    }

    private void DropWeapon()
    {
        if (weaponSlots.Count <= 1)
            return;

        weaponSlots.Remove(currentWeapon);

        currentWeapon = weaponSlots[0];
    }
    #endregion

    private void Shoot()
    {
        //myself but we can make fun cancle grab
        if (player.weaponVisuals.isGrabbingWeapon)
            return;

        if (!currentWeapon.CanShot())
        {
            if (currentWeapon.CanReload())
                player.weaponVisuals.PlayReloadAnimation();

            return;
        }

        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();
        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = BulletDirection() * bulletSpeed;

        GetComponentInChildren<Animator>().SetTrigger("Fire");

        Destroy(newBullet, 10);
    }

    public Vector3 BulletDirection()
    {
        //find a better place for it because it off when reload or grab
        /* weaponHolder.LookAt(aim);
         gunPoint.LookAt(aim);*/
        Transform aim = player.aim.Aim();

        Vector3 direction = (aim.position - gunPoint.position).normalized;

        if (!player.aim.CanAimPrecisely() && player.aim.Target() == null)
            direction.y = 0;

        return direction;
    }

    public Weapon CurrentWeapon => currentWeapon;

    public Transform GunPoint() => gunPoint;

    #region Input Events
    private void AssignInputEvents()
    {
        PlayerControlls controls = player.controls;

        controls.Character.Fire.performed += context => Shoot();
        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);

        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload())
                player.weaponVisuals.PlayReloadAnimation();
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
