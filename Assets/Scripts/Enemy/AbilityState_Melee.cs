using UnityEngine;

public class AbilityState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 movementDirection;

    private const float MAX_MOVEMENT_DISTANCE = 20;

    private float moveSpeed;

    public AbilityState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.ActiveWeapon(true);

        moveSpeed = enemy.walkSpeed;
        movementDirection = enemy.transform.position + enemy.transform.forward * MAX_MOVEMENT_DISTANCE;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotateActive())
        {
            enemy.transform.rotation = enemy.FaceTarget(Player.instance.transform.position);
            movementDirection = enemy.transform.position + enemy.transform.forward * MAX_MOVEMENT_DISTANCE;
        }

        if (enemy.ManualMovementActive())
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, movementDirection, enemy.walkSpeed * Time.deltaTime);

        if (triggerCalled)
            stateMachine.ChangeState(enemy.recoveryState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        GameObject newAxe = ObjectPool.instance.GetObject(enemy.axePrefab);

        newAxe.transform.position = enemy.axeStartPoint.position;

        newAxe.GetComponent<EnemyAxe>().AxeSetup(enemy.transform.forward, enemy.axeFlySpeed, enemy.axeAimTimer);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.walkSpeed = moveSpeed;
        enemy.anim.SetFloat("RecoveryIndex", 0);
    }
}
