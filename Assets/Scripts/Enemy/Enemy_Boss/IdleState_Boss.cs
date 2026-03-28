using UnityEngine;

public class IdleState_Boss : EnemyState
{
    private Enemy_Boss enemy;

    public IdleState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = true;

        stateTimer = enemyBase.idleTime;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.inBattleMode && enemy.PlayerInAttackRange())
        {
            stateMachine.ChangeState(enemy.attackState);
        }

        if (stateTimer < 0)
            enemy.stateMachine.ChangeState(enemy.moveState);
    }
}
