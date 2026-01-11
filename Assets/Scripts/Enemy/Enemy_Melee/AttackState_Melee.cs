using System.Collections.Generic;
using UnityEngine;

public class AttackState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 attackTarget;
    private float attackMoveSpeed;

    private const float MAX_ATTACK_DISTANCE = 50;

    public AttackState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        //test while playing
        //enemy.UpdateAttackData();

        enemy.ActiveWeapon(true);
        enemy.visuals.EnableWeaponTrail(true);

        attackMoveSpeed = enemy.attackData.moveSpeed;
        enemy.anim.SetFloat("AttackAnimationSpeed", enemy.attackData.animationSpeed);
        enemy.anim.SetFloat("AttackIndex", enemy.attackData.attackIndex);
        enemy.anim.SetFloat("SlashAttackIndex", Random.Range(0, 6));

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        attackTarget = enemy.transform.position + enemy.transform.forward * MAX_ATTACK_DISTANCE;
    }

    public override void Exit()
    {
        base.Exit();

        SetupNextAttack();

        enemy.visuals.EnableWeaponTrail(false);
    }

    private void SetupNextAttack()
    {
        int recoveryIndex = PlayerClose() ? 1 : 0;
        enemy.anim.SetFloat("RecoveryIndex", recoveryIndex);

        enemy.attackData = UpdateAttackData();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotateActive())
        {
            enemy.FaceTarget(Player.instance.transform.position);
            attackTarget = enemy.transform.position + enemy.transform.forward * MAX_ATTACK_DISTANCE;
        }

        if (enemy.ManualMovementActive())
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, attackTarget, attackMoveSpeed * Time.deltaTime);

        //
        if (triggerCalled)
            if (enemy.PlayerInAttackRange())
            {
                Debug.Log("Change to Recovery State from Attack State");
                stateMachine.ChangeState(enemy.recoveryState);
            }
            else
                stateMachine.ChangeState(enemy.chaseState);
    }

    private bool PlayerClose() => Vector3.Distance(enemy.transform.position, Player.instance.transform.position) <= 1;

    private AttackDataEnemy_Melee UpdateAttackData()
    {
        List<AttackDataEnemy_Melee> validAttacks = new List<AttackDataEnemy_Melee>(enemy.attackList);

        if (PlayerClose())
            validAttacks.RemoveAll(parameter => parameter.attackType == AttackType_Melee.Charge);

        // Kiểm tra nếu list rỗng thì giữ nguyên attackData hiện tại
        if (validAttacks.Count == 0)
        {
            Debug.LogWarning($"No valid attacks found for {enemy.gameObject.name}. Keeping current attack data.");
            return enemy.attackData;
        }

        int random = Random.Range(0, validAttacks.Count);

        return validAttacks[random];
    }
}
