using UnityEngine;

public class AdvancePlayer_Range : EnemyState
{
    private Enemy_Range enemy;

    public AdvancePlayer_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.visuals.EnableIK(true, true);

        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.advanceSpeed;
    }

    public override void Update()
    {
        base.Update();

        enemy.agent.SetDestination(Player.instance.transform.position);
        enemy.FaceTarget(GetNextPathPoint());

        if (Vector3.Distance(enemy.transform.position, Player.instance.transform.position) < enemy.advanceStoppingDistance)
            stateMachine.ChangeState(enemy.battleState);
    }
}
