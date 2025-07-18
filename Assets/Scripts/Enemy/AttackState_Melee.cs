using UnityEngine;

public class AttackState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 attackTarget;

    private const float MAX_ATTACK_DISTANCE = 50;

    public AttackState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.ActiveWeapon(true);

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        attackTarget = enemy.transform.position + enemy.transform.forward * MAX_ATTACK_DISTANCE;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualMovementActive())
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, attackTarget, enemy.attackMoveSpeed * Time.deltaTime);

        //
        if (triggerCalled)
            stateMachine.ChangeState(Random.Range(0, 100) > 30 ? enemy.chaseState : enemy.recoveryState);
    }
}
