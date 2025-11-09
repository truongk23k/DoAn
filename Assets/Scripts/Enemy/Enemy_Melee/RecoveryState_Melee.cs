

using UnityEngine;

public class RecoveryState_Melee : EnemyState
{
    private Enemy_Melee enemy;

    public RecoveryState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter Recovery State");
        enemy.agent.isStopped = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(Player.instance.transform.position);

        if (triggerCalled)
        {
            if (enemy.CanThrowAxe())
            {
                stateMachine.ChangeState(enemy.abilityState);
                return;
            }

            if (enemy.PlayerInAttackRange())
            {
                stateMachine.ChangeState(enemy.attackState);
                Debug.Log("Enter Attack State");
            }
            else
                stateMachine.ChangeState(enemy.chaseState);

        }
    }
}
