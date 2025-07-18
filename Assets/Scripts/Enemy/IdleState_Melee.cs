using UnityEngine;

public class IdleState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    public IdleState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemyBase.idleTimer;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //recover
        if (enemy.PlayerInAggresionRange())
        {
            stateMachine.ChangeState(enemy.recoveryState);
            return;
        }

        if (stateTimer < 0)
            enemy.stateMachine.ChangeState(enemy.moveState);
    }
}
