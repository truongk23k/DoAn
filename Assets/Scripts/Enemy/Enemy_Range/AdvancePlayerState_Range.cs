using UnityEngine;

public class AdvancePlayerState_Range : EnemyState
{
    private Enemy_Range enemy;

    public float lastTimeAdvanced { get; private set; }
    public AdvancePlayerState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.visuals.EnableIK(true, true);

        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.advanceSpeed;

        if (enemy.IsUnstoppable())
        {
            enemy.visuals.EnableIK(true, false);
            stateTimer = enemy.advanceDuration;
        }
    }

    public override void Update()
    {
        base.Update();

        enemy.UpdateAimPosition();

        enemy.agent.SetDestination(Player.instance.transform.position);
        enemy.FaceTarget(GetNextPathPoint());

        if (CanEnterBattleState() && enemy.IsSeeingPlayer())
            stateMachine.ChangeState(enemy.battleState);
    }

    override public void Exit()
    {
        base.Exit();
        lastTimeAdvanced = Time.time;
    }

    private bool CanEnterBattleState()
    {
        bool closeEnoughToPlayer = Vector3.Distance(enemy.transform.position, Player.instance.transform.position) < enemy.advanceStoppingDistance;
        
        if(enemy.IsUnstoppable())
            return closeEnoughToPlayer || stateTimer <= 0f;
        else
            return closeEnoughToPlayer;
    }
}
