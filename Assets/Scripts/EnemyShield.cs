using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    [SerializeField] private int durability;

    private Enemy_Melee enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Melee>();

    }

    public void ReduceDurability()
    {
        durability--;

        if (durability <= 0)
        {
            gameObject.SetActive(false);
            enemy.anim.SetFloat("ChaseIndex", 0); // chase without shield
        }
    }
}
