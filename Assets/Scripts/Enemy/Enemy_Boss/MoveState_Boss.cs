using UnityEngine;

public class MoveState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    private Vector3 destination;

    public MoveState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = false;

        enemy.agent.speed = enemy.walkSpeed;

        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(GetNextPathPoint());

        if (enemy.inBattleMode)
        {
            enemy.agent.SetDestination(Player.instance.transform.position);

            if (enemy.CanJumpAttack())
                stateMachine.ChangeState(enemy.jumpAttackState);
            else if (enemy.PlayerInAttackRange())
                stateMachine.ChangeState(enemy.attackState);
        }
        else
        {
            if (Vector3.Distance(enemy.transform.position, destination) < 0.25f)
                stateMachine.ChangeState(enemy.idleState);
        }


    }
}
