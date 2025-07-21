using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float impactForce;

    private BoxCollider cd;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private TrailRenderer trailRenderer;

    [SerializeField] private GameObject bulletImpactFX;

    private Vector3 startPosition;
    private float flyDistance;

    private bool bulletDisabled;

    //default bullet speed
    private const float REFERENCE_BULLET_SPEED = 20f;
    private Vector3 bulletDirection;
    private float bulletSpeed; // bulletSpeed/8 to decrease Trail => decreaseSpeedTimeTrail

    private void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void BulletSetup(float flyDistance, Vector3 bulletDirection, float bulletSpeed, float impactForce)
    {
        this.impactForce = impactForce;

        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.time = 0.25f;
        startPosition = transform.position;
        this.flyDistance = flyDistance + 0.5f;
        this.bulletDirection = bulletDirection;
        this.bulletSpeed = bulletSpeed;

        rb.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rb.velocity = this.bulletDirection * bulletSpeed;
    }

    private void Update()
    {
        FadeTrailIfNeeded();
        DiasbleBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    private void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time <= 0)
            ReturnBulletToPool();
    }

    private void DiasbleBulletIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    private void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
            trailRenderer.time -= bulletSpeed / 8 * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            Vector3 force = rb.velocity.normalized * impactForce;
            Rigidbody hitRb = collision.collider.attachedRigidbody;

            enemy.GetHit();
            enemy.HitImpact(force, collision.contacts[0].point, hitRb);
        }

        CreateImpactFx(collision);

        ReturnBulletToPool();
    }

    private void ReturnBulletToPool() => ObjectPool.instance.ReturnObject(gameObject);

    private void CreateImpactFx(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];

            //Instantiate(bulletImpactFX, contact.point, Quaternion.LookRotation(contact.normal));
            GameObject newBulletImpactFx = ObjectPool.instance.GetObject(bulletImpactFX);
            newBulletImpactFx.transform.position = contact.point;
            newBulletImpactFx.transform.rotation = Quaternion.LookRotation(contact.normal);

            ObjectPool.instance.ReturnObject(newBulletImpactFx, 1);
        }
    }
}
