using System.Collections;
using UnityEngine;

public class Enemy_Axe : MonoBehaviour
{
    [SerializeField] private GameObject impactFx;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;
    private float flySpeed;
    private float rotationSpeed;

    private Vector3 direction;
    private float timer = 1f;

    private bool canChangeDir;

    private int damage;

    public void AxeSetup(Vector3 originForward, float flySpeed, float timer, int damage)
    {
        canChangeDir = true;
        transform.forward = originForward;
        direction = transform.forward;

        rotationSpeed = 1600;

        this.damage = damage;
        this.flySpeed = flySpeed;
        this.timer = timer;

        rb.interpolation = RigidbodyInterpolation.None;
        // Đợi 1 frame rồi bật lại Interpolate
        StartCoroutine(EnableInterpolationDelayed());

        ObjectPool.instance.ReturnObject(gameObject, 10f);
    }



    IEnumerator EnableInterpolationDelayed()
    {
        yield return null;
        yield return null;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Update()
    {
        axeVisual.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);

        timer -= Time.deltaTime;

        if (timer > 0 && Vector3.Distance(transform.position, Player.instance.transform.position) > 1.5f && canChangeDir)
        {
            direction = Player.instance.transform.position + Vector3.up - transform.position;
        }
        else
            canChangeDir = false;


        transform.forward = rb.velocity;
    }

    private void FixedUpdate()
    {
        rb.velocity = direction.normalized * flySpeed;
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        damagable?.TakeDamage(damage);

        GameObject newFx = ObjectPool.instance.GetObject(impactFx, transform);

        ObjectPool.instance.ReturnObject(gameObject);
        ObjectPool.instance.ReturnObject(newFx, 1f);
    }
}
