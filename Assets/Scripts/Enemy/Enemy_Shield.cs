using UnityEngine;

public class Enemy_Shield : MonoBehaviour, IDamagable
{
    [SerializeField] private int durability;

    private Enemy_Melee enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Melee>();
        durability = enemy.shieldDurability;

    }

    public void ReduceDurability()
    {
        durability--;

        if (durability <= 0)
        {
            enemy.anim.SetFloat("ChaseIndex", 0); // chase without shield
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage()
    {
        ReduceDurability();
    }
}
