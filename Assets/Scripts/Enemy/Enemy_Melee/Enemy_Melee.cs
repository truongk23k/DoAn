using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackDataEnemy_Melee
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
    Regular,//auto if haven't special types
    Shield,
    Dodge,
    AxeThrow
}

public class Enemy_Melee : Enemy
{
    public Enemy_Visuals visuals { get; private set; }

    #region States
    public IdleState_Melee idleState { get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public RecoveryState_Melee recoveryState { get; private set; }
    public ChaseState_Melee chaseState { get; private set; }
    public AttackState_Melee attackState { get; private set; }
    public DeadState_Melee deadState { get; private set; }
    public AbilityState_Melee abilityState { get; private set; }
    #endregion

    [Header("Enemy settings")]
    public List<EnemyMelee_Type> meleeTypes = new List<EnemyMelee_Type>();
    public Transform shieldTransform;
    public float dodgeCooldown;
    private float lastTimeDodge = -10;
    private Enemy_Ragdoll ragdoll; // use to disable colli when dodge

    [Header("Axe throw ability")]
    public GameObject axePrefab;
    public float axeFlySpeed;
    public float axeAimTimer;
    public float axeThrowCooldown;
    private float lastTimeAxeThrow;
    public Transform axeStartPoint;

    [Header("Attack data")]
    public AttackDataEnemy_Melee attackData;
    public List<AttackDataEnemy_Melee> attackList;

    protected override void Awake()
    {
        base.Awake();

        visuals = GetComponent<Enemy_Visuals>();

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

        InitializePerk();
        visuals.SetupLook();
        UpdateAttackData();
    }

    protected override void Update()
    {
        base.Update();


    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();

        stateMachine.ChangeState(recoveryState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        walkSpeed *= 0.6f;
        ActiveWeapon(false);
    }

    public void UpdateAttackData()
    {
        Enemy_WeaponModel currentWeapon = visuals.currentWeaponModel.GetComponent<Enemy_WeaponModel>();

        if(currentWeapon.weaponData != null)
        {
            attackList = new List<AttackDataEnemy_Melee>(currentWeapon.weaponData.attackData);
            turnSpeed = currentWeapon.weaponData.turnSpeed;
        }
    }

    private void InitializePerk()
    {
        //setup first because override AC
        if (meleeTypes.Contains(EnemyMelee_Type.Dodge))
        {
            visuals.SetupWeaponType(Enemy_MeleeWeaponType.Unarmed);
        }

        if (meleeTypes.Contains(EnemyMelee_Type.AxeThrow))
        {
            visuals.SetupWeaponType(Enemy_MeleeWeaponType.Throw);
        }

        if (meleeTypes.Contains(EnemyMelee_Type.Shield))
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
            visuals.SetupWeaponType(Enemy_MeleeWeaponType.OneHand);
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
        /* hiddenWeapon.gameObject.SetActive(!active);
         pulledWeapon.gameObject.SetActive(active);*/
        visuals.hiddenWeaponModel.SetActive(!active);
        visuals.currentWeaponModel.SetActive(active);
    }

    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, Player.instance.transform.position) < attackData.attackRange;

    public void ActiveDodgeRoll()
    {
        if (!meleeTypes.Contains(EnemyMelee_Type.Dodge))
            return;

        if (Vector3.Distance(transform.position, Player.instance.transform.position) < 1f)
            return;

        if (Time.time > dodgeCooldown + lastTimeDodge)
        {
            anim.SetTrigger("Dodge");

        }
    }

    public void AssignLastTimeDodge() => lastTimeDodge = Time.time;

    public bool CanThrowAxe()
    {
        if (!meleeTypes.Contains(EnemyMelee_Type.AxeThrow))
            return false;

        if (Time.time > lastTimeAxeThrow + axeThrowCooldown)
        {
            lastTimeAxeThrow = Time.time;
            return true;
        }

        return false;
    }

    public void StartDodge() => ragdoll.CollidersActive(false);
    public void StopDodge() => ragdoll.CollidersActive(true);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }
}
