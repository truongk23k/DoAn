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

    override public void TakeDamage()
    {
        base.TakeDamage();
        enemy.healthPoints--;
         
    }

}
