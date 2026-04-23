using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : HealthController
{
    public bool isDead { get; private set; }
    public override void ReduceHealth(int damage)
    {
        base.ReduceHealth(damage);

        if (ShouldDie())
            Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        Player.instance.anim.enabled = false;
        Player.instance.ragdoll.RagdollActive(true);
    }
}
