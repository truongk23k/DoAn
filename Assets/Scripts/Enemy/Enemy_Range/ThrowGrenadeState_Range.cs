using UnityEngine;


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

        enemy.visuals.ActiveWeapon(false);
        enemy.visuals.EnableIK(false, false);
        enemy.visuals.EnableSecondaryWeaponModel(true);
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(Player.instance.transform.position + Vector3.up);
        enemy.aim.position = Player.instance.transform.position + Vector3.up;

        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        enemy.ThrowGrenade();       
    }
}
