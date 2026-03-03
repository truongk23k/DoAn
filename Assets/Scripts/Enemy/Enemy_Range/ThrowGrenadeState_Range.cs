public class ThrowGrenadeState_Range : EnemyState
{
    Enemy_Range enemy;

    public ThrowGrenadeState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(Player.instance.transform.position);

        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        enemy.ThrowGrenade();
    }
}
