using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState_Range : EnemyState
{
    private Enemy_Range enemy;
    private bool interactionDisabled;

    public DeadState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        if (!enemy.throwGrenadeState.finishedThrowingGrenade)
            enemy.ThrowGrenade();

        interactionDisabled = false;

        enemy.anim.enabled = false;
        /* enemy.agent.isStopped = true;*/
        enemy.agent.enabled = false;

        enemy.ragdoll.RagdollActive(true);

        stateTimer = 2f;
    }

    public override void Update()
    {
        base.Update();

        DisableInteractionIfShould();
    }

    private void DisableInteractionIfShould()
    {
        if (stateTimer < 0 && !interactionDisabled)
        {
            interactionDisabled = true;
            //enemy.ragdoll.RagdollActive(false);
            enemy.ragdoll.CollidersActive(false);
        }
    }
}
