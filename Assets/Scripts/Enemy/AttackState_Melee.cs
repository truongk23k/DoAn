using UnityEngine;

public class AttackState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 attackTarget;
    private float attckMoveSpeed;

    private const float MAX_ATTACK_DISTANCE = 50;

    public AttackState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        attckMoveSpeed = enemy.attackData.moveSpeed;
        enemy.anim.SetFloat("AttackAnimationSpeed", enemy.attackData.animationSpeed);
        enemy.anim.SetFloat("AttackIndex", enemy.attackData.attackIndex);

        enemy.ActiveWeapon(true);

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        attackTarget = enemy.transform.position + enemy.transform.forward * MAX_ATTACK_DISTANCE;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.anim.SetFloat("RecoveryIndex", 0);

        if(enemy.PlayerInAttackRange())
            enemy.anim.SetFloat("RecoveryIndex", 1);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotateActive())
        {
            enemy.transform.rotation = enemy.FaceTarget(Player.instance.transform.position);
            attackTarget = enemy.transform.position + enemy.transform.forward * MAX_ATTACK_DISTANCE;
        }

        if (enemy.ManualMovementActive())
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, attackTarget, attckMoveSpeed * Time.deltaTime);

        //
        if (triggerCalled)
            if (enemy.PlayerInAttackRange())
                stateMachine.ChangeState(enemy.recoveryState);
            else
                stateMachine.ChangeState(enemy.chaseState);
    }
}
