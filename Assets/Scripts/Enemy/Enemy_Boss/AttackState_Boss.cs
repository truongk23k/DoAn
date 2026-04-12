using UnityEngine;

public class AttackState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    public float lastTimeAttacked { get; private set; } //for MoveState_Boss to know to perform run anim

    public AttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.anim.SetFloat("AttackAnimIndex", Random.Range(0, 2)); // have 2 attack: 0 and 1

        enemy.agent.isStopped = true;

        stateTimer = 1f; //fake: this time to rotate foward player before attacking
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
        {
            enemy.FaceTarget(Player.instance.transform.position, 20);
        }

        if (triggerCalled)
        {
            if (enemy.PlayerInAttackRange())
            {
                stateMachine.ChangeState(enemy.attackState);
            }
            else
                stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        lastTimeAttacked = Time.time;
    }
}
