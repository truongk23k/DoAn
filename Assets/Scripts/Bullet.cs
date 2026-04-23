using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int bulletDamage;
    public float impactForce;

    private BoxCollider cd;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private TrailRenderer trailRenderer;

    [SerializeField] private GameObject bulletImpactFX;

    private Vector3 startPosition;
    private float flyDistance;

    private bool bulletDisabled;

    private LayerMask allyLayerMask;

    //default bullet speed
    public float REFERENCE_BULLET_SPEED = 20f;
    private Vector3 bulletDirection;
    private float bulletSpeed; // bulletSpeed/8 to decrease Trail => decreaseSpeedTimeTrail

    protected virtual void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void BulletSetup(LayerMask allyLayerMask, Vector3 bulletDirection, float bulletSpeed, int bulletDamage, float flyDistance = 100, float impactForce = 100)
    {
        this.impactForce = impactForce;
        this.allyLayerMask = allyLayerMask;
        this.bulletDamage = bulletDamage;

        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.Clear();
        trailRenderer.time = 0.25f;
        startPosition = transform.position;
        this.flyDistance = flyDistance + 0.5f;
        this.bulletDirection = bulletDirection;
        this.bulletSpeed = bulletSpeed;

        rb.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rb.velocity = this.bulletDirection * bulletSpeed;
    }

    protected virtual void Update()
    {
        FadeTrailIfNeeded();
        DiasbleBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    protected void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time <= 0)
            ReturnBulletToPool();
    }

    protected void DiasbleBulletIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    protected void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
            trailRenderer.time -= bulletSpeed / 8 * Time.deltaTime;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (!FriendlyFire())
        {
            if ((allyLayerMask.value & (1 << collision.gameObject.layer)) > 0)
            {
                ReturnBulletToPool(10);
                return;
            }
        }

        CreateImpactFx();
        ReturnBulletToPool();

        IDamagable damagable = collision.gameObject.GetComponentInParent<IDamagable>();
        damagable?.TakeDamage(bulletDamage);

        ApplyBulletImpactToEnemy(collision);
    }

    private void ApplyBulletImpactToEnemy(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Vector3 force = rb.velocity.normalized * impactForce;
            Rigidbody hitRb = collision.collider.attachedRigidbody;

            enemy.BulletImpact(force, collision.contacts[0].point, hitRb);
        }
    }

    protected void ReturnBulletToPool(float delay = 0) => ObjectPool.instance.ReturnObject(gameObject, delay);

    protected void CreateImpactFx()
    {
        GameObject newBulletImpactFx = ObjectPool.instance.GetObject(bulletImpactFX, transform);
        ObjectPool.instance.ReturnObject(newBulletImpactFx, 1);
    }

    private bool FriendlyFire() => GameManager.instance.friendlyFire;
}
