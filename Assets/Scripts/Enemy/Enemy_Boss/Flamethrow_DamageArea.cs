using UnityEngine;

public class Flamethrow_DamageArea : MonoBehaviour
{
    private Enemy_Boss enemy;

    private float damageCooldown;
    private float lastTimeDamaged;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Boss>();

        damageCooldown = enemy.flameDamageCooldown;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!enemy.flamethrowerActive)
            return;

        if (Time.time - lastTimeDamaged < damageCooldown)
            return;

        IDamagable damagable = other.GetComponent<IDamagable>();

        if (damagable != null)
        {
            damagable.TakeDamage();
            lastTimeDamaged = Time.time;
        }
    }
}
