using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed;
    [SerializeField] Transform gunPoint;

    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Transform aim;

    private void Start()
    {
        player = GetComponent<Player>();
        player.controls.Character.Fire.performed += context => Shoot();
    }

    private void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));
        newBullet.GetComponent<Rigidbody>().velocity = BulletDirection() * bulletSpeed;

        GetComponentInChildren<Animator>().SetTrigger("Fire");

        Destroy(newBullet, 10);
    }

    private Vector3 BulletDirection()
    {
        weaponHolder.LookAt(aim);
        gunPoint.LookAt(aim);

        Vector3 direction = (aim.position - gunPoint.position).normalized;
        //direction.y = 0;

        return direction;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(weaponHolder.position, weaponHolder.position + weaponHolder.forward * 25);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(gunPoint.position, gunPoint.position + BulletDirection() * 25);
    }
}
