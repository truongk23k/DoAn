using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_HitBox : HitBox
{
    private Player player;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponentInParent<Player>();
    }

    override public void TakeDamage()
    {
        player.health.ReduceHealth();
    }
}
