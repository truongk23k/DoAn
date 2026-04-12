using UnityEngine;

public class AbilityState_Boss : EnemyState
{
    private Enemy_Boss enemy;

    public AbilityState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.flamethrowDuration + 2f;
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(Player.instance.transform.position);

        if (stateTimer < 0 && enemy.flamethrowerActive)
            enemy.ActivateFlamethrower(false);

        if (triggerCalled)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        enemy.ActivateFlamethrower(true);
        enemy.bossVisuals.DischargeBatteries();
    }

    override public void Exit()
    {
        base.Exit();
        enemy.SetAbilityOnCooldown();
        enemy.bossVisuals.ResetBatteries();
    }
}
