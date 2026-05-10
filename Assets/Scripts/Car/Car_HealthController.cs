using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_HealthController : MonoBehaviour, IDamagable
{
    private Car_Controller carController;

    public int maxHealth;
    public int currentHealth;

    private bool carBroken;

    [Header("Explosion info")]
    [SerializeField] private int explosionDamage = 100;
    [Space]
    [SerializeField] private float explosionRadius = 3;
    [SerializeField] private float explosionDelay = 3;
    [SerializeField] private float explosionForce = 7;
    [SerializeField] private float explosionUpwardsModifier = 2;
    [Space]
    [SerializeField] private ParticleSystem fireFx;
    [SerializeField] private ParticleSystem explosionFx;

    private void Start()
    {
        carController = GetComponent<Car_Controller>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (fireFx.gameObject.activeSelf)
            fireFx.transform.rotation = Quaternion.identity;
    }

    public void UpdateCarHealthUI()
    {
        UI.instance.inGameUI.UpdateCarHealthUI(currentHealth, maxHealth);
    }

    private void ReduceHealth(int damage)
    {
        if (carBroken)
            return;

        currentHealth -= damage;

        if (currentHealth < 0)
            BrakeTheCar();
    }

    private void BrakeTheCar()
    {
        carBroken = true;
        carController.BrakeTheCar();

        fireFx.gameObject.SetActive(true);
        StartCoroutine(ExplosionCo(explosionDelay));
    }

    public void TakeDamage(int damage)
    {
        ReduceHealth(damage);
        UpdateCarHealthUI();
    }

    private IEnumerator ExplosionCo(float delay)
    {
        yield return new WaitForSeconds(delay);

        explosionFx.gameObject.SetActive(true);

        carController.rb.AddExplosionForce(explosionForce, transform.position - Vector3.down + (Vector3.forward * 1.5f),
            explosionRadius, explosionUpwardsModifier, ForceMode.Impulse);

        Explode();
    }

    private void Explode()
    {
        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            IDamagable damagable = hit.GetComponent<IDamagable>();

            if (damagable != null)
            {
                GameObject rootEntity = hit.transform.root.gameObject;

                if (uniqueEntities.Add(rootEntity) == false)
                    continue;

                damagable.TakeDamage(explosionDamage);
                Vector3 explosionPoint = transform.position + (Vector3.forward * 1.5f);

                hit.GetComponentInChildren<Rigidbody>()?.
                    AddExplosionForce(explosionForce, explosionPoint, explosionRadius, explosionUpwardsModifier, ForceMode.VelocityChange);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3.forward * 1.5f), explosionRadius);
    }
}
