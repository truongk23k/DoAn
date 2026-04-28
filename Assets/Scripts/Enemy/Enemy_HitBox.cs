using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_HitBox : HitBox
{
    private Enemy enemy;

    override protected void Awake()
    {
        base.Awake();
        enemy = GetComponentInParent<Enemy>();
    }

    override public void TakeDamage(int damage)
    {
        int newDamage = Mathf.RoundToInt(damage * damageMultilier);

        enemy.GetHit(newDamage);
    }

}
