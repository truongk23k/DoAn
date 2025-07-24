using System.Collections;
using UnityEngine;

public class EnemyAxe : MonoBehaviour
{
    [SerializeField] private GameObject impactFx;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;
    public float flySpeed;
    public float rotationSpeed;

    public Vector3 direction;
    public float timer = 1f;

    private bool canChangeDir;

    public void AxeSetup(Vector3 originForward, float flySpeed, float timer)
    {
        canChangeDir = true;
        transform.forward = originForward;
        direction = transform.forward;

        rotationSpeed = 1600;

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

        if (timer > 0 && Vector3.Distance(transform.position, Player.instance.transform.position) > 1 && canChangeDir)
        {
            direction = Player.instance.transform.position + Vector3.up - transform.position;
        }
        else
            canChangeDir = false;

        rb.velocity = direction.normalized * flySpeed;

        transform.forward = rb.velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        Player player = other.GetComponent<Player>();

        if (bullet != null || player != null)
        {
            GameObject newFx = ObjectPool.instance.GetObject(impactFx);
            newFx.transform.position = transform.position;

            ObjectPool.instance.ReturnObject(gameObject);
            ObjectPool.instance.ReturnObject(newFx, 1f);
        }
    }
}
