using UnityEngine;

public class JumpAttackState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    private Vector3 lastPlayerPos;

    private float jumpAttackMovementSpeed;

    public JumpAttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        lastPlayerPos = Player.instance.transform.position;
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        enemy.bossVisuals.PlaceLandingZone(lastPlayerPos);
        enemy.bossVisuals.EnableWeaponTrail(true);

        float distanceToPlayer = Vector3.Distance(enemy.transform.position, lastPlayerPos);
        jumpAttackMovementSpeed = distanceToPlayer / enemy.travelTimeToTarget;

        enemy.FaceTarget(lastPlayerPos, 1000);
    }

    public override void Update()
    {
        base.Update();

        Vector3 myPos = enemy.transform.position;
        enemy.agent.enabled = !enemy.ManualMovementActive();

        if (enemy.ManualMovementActive())
        {
            enemy.transform.position = Vector3.MoveTowards(myPos, lastPlayerPos, jumpAttackMovementSpeed * Time.deltaTime);
        }

        if (triggerCalled)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.SetJumpAttackOnCooldown();
        enemy.bossVisuals.EnableWeaponTrail(false);
    }
}
