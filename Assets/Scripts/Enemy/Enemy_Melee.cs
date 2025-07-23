using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackData
{
    public string attackName;
    public AttackType_Melee attackType;
    public float attackRange;
    public float moveSpeed;
    public float attackIndex;
    [Range(1, 2)]
    public float animationSpeed;
}

public enum AttackType_Melee
{
    Close,
    Charge
}

public enum EnemyMelee_Type
{
    Regular,
    Shield,
    Dodge
}

public class Enemy_Melee : Enemy
{
    public IdleState_Melee idleState { get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public RecoveryState_Melee recoveryState { get; private set; }
    public ChaseState_Melee chaseState { get; private set; }
    public AttackState_Melee attackState { get; private set; }
    public DeadState_Melee deadState { get; private set; }
    public AbilityState_Melee abilityState { get; private set; }

    [Header("Enemy settings")]
    public EnemyMelee_Type meleeType;
    public Transform shieldTransform;
    public float dodgeCooldown;
    private float lastTimeDodge;
    private Enemy_Ragdoll ragdoll; // use to disable colli when dodge

    [Header("Attack data")]
    public AttackData attackData;
    public List<AttackData> attackList;

    [SerializeField] private Transform hiddenWeapon;
    [SerializeField] private Transform pulledWeapon;

    protected override void Awake()
    {
        base.Awake();

        ragdoll = GetComponent<Enemy_Ragdoll>();

        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        chaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        attackState = new AttackState_Melee(this, stateMachine, "Attack");
        deadState = new DeadState_Melee(this, stateMachine, "Idle"); // Idle anim is just a place holder, use ragdoll
        abilityState = new AbilityState_Melee(this, stateMachine, "AxeThrow");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);

        InitializeSpeciality();
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    public void TriggerAbility()
    {
        walkSpeed *= 0.6f;
        Debug.Log("Create axe");
        ActiveWeapon(false);
    }

    private void InitializeSpeciality()
    {
        if (meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
        }
    }

    public override void GetHit()
    {
        base.GetHit();

        if (healthPoints <= 0)
            stateMachine.ChangeState(deadState);
    }

    public void ActiveWeapon(bool active)
    {
        hiddenWeapon.gameObject.SetActive(!active);
        pulledWeapon.gameObject.SetActive(active);
    }

    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, Player.instance.transform.position) < attackData.attackRange;

    public void ActiveDodgeRoll()
    {
        if (meleeType != EnemyMelee_Type.Dodge)
            return;

        if (Vector3.Distance(transform.position, Player.instance.transform.position) < 2f)
            return;

        if (Time.time > dodgeCooldown + lastTimeDodge)
        {
            lastTimeDodge = Time.time;
            anim.SetTrigger("Dodge");
            ragdoll.CollidersActive(false);
        }
    }

    public void StopDodge() => ragdoll.CollidersActive(true);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }
}
