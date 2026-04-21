using UnityEngine;

public enum BossWeaponType
{
    Flamethrower,
    Hummer
}

public class Enemy_Boss : Enemy
{
    [Header("Boss details")]
    public BossWeaponType bossWeaponType;
    public float actionCooldown = 10;
    public float attackRange;

    [Header("Ability")]
    public float minAbilityDistance;
    public float abilityCooldown;
    private float lastTimeUsedAbility;

    [Header("Flamethrower ")]
    public ParticleSystem flamethrower;
    public float flamethrowDuration;
    public bool flamethrowerActive { get; private set; }

    [Header("Hummer")]
    public GameObject activationPrefab;

    [Header("Jump attack")]
    public float jumpAttackCooldown = 10;
    private float lastTimeJumped;
    public float travelTimeToTarget = 1f;
    public float minJumpDistanceRequired;
    [Space]
    public float impactRadius = 2.5f;
    public float impactPower = 5f;
    public Transform impactPoint;
    [SerializeField] private float upforceMultiplier = 10f;
    [Space]
    [SerializeField] private LayerMask whatToIgnore;
    public IdleState_Boss idleState { get; private set; }
    public MoveState_Boss moveState { get; private set; }
    public AttackState_Boss attackState { get; private set; }
    public JumpAttackState_Boss jumpAttackState { get; private set; }
    public AbilityState_Boss abilityState { get; private set; }
    public DeadState_Boss deadState { get; private set; }

    public Enemy_BossVisuals bossVisuals { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        bossVisuals = GetComponent<Enemy_BossVisuals>();

        idleState = new IdleState_Boss(this, stateMachine, "Idle");
        moveState = new MoveState_Boss(this, stateMachine, "Move");
        attackState = new AttackState_Boss(this, stateMachine, "Attack");
        jumpAttackState = new JumpAttackState_Boss(this, stateMachine, "JumpAttack");
        abilityState = new AbilityState_Boss(this, stateMachine, "Ability");
        deadState = new DeadState_Boss(this, stateMachine, "Idle"); // Idle anim is just a place holder, use ragdoll
    }

    override protected void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    override protected void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

        if (ShouldEnterBattleMode())
            EnterBattleMode();
    }

    public override void Die()
    {
        base.Die();

        if (stateMachine.currentState != deadState)
            stateMachine.ChangeState(deadState);
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();
        stateMachine.ChangeState(moveState);
    }

    public void ActivateFlamethrower(bool activate)
    {
        flamethrowerActive = activate;

        if (!activate)
        {
            flamethrower.Stop();
            anim.SetTrigger("StopFlamethrower");
            return;
        }

        var mainModule = flamethrower.main;
        mainModule.duration = flamethrowDuration + 0.5f; //flamethrower include anim startup time

        var extraModule = flamethrower.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        extraModule.duration = flamethrowDuration + 0.5f;

        flamethrower.Clear();
        flamethrower.Play();
    }

    public void ActivateHummer()
    {
        GameObject newActivation = ObjectPool.instance.GetObject(activationPrefab, impactPoint);

        ObjectPool.instance.ReturnObject(newActivation, 1f);
    }

    public bool CanDoAbility()
    {
        bool playerWithinDistance = Vector3.Distance(transform.position, Player.instance.transform.position) < minAbilityDistance;

        if(!playerWithinDistance)
            return false;

        if (Time.time >= lastTimeUsedAbility + abilityCooldown && playerWithinDistance)
        {
            return true;
        }
        return false;
    }

    public void SetAbilityOnCooldown() => lastTimeUsedAbility = Time.time;

    public void JumpImpact()
    {
        Transform impactPoint = this.impactPoint != null ? this.impactPoint : transform;

        Collider[] colliders = Physics.OverlapSphere(impactPoint.position, impactRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(impactPower, impactPoint.position, impactRadius, upforceMultiplier, ForceMode.Impulse);
            }
        }
    }


    public bool CanDoJumpAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.instance.transform.position);

        if (distanceToPlayer < minJumpDistanceRequired)
            return false;

        if (Time.time >= lastTimeJumped + jumpAttackCooldown && IsPlayerInClearSight())
        {
            return true;
        }

        return false;
    }

    public void SetJumpAttackOnCooldown() => lastTimeJumped = Time.time;

    public bool IsPlayerInClearSight()
    {
        Vector3 myPos = transform.position + new Vector3(0, 1.5f, 0);
        Vector3 directionToPlayer = (Player.instance.transform.position - myPos).normalized;

        if (Physics.Raycast(myPos, directionToPlayer, out RaycastHit hit, 100, ~whatToIgnore))
        {
            if (hit.transform == Player.instance.transform || hit.transform.parent == Player.instance.transform)
                return true;
        }

        return false;
    }

    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, Player.instance.transform.position) < attackRange;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (Player.instance != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 myPos = transform.position + new Vector3(0, 1.5f, 0);
            Gizmos.DrawLine(myPos, Player.instance.transform.position);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minJumpDistanceRequired);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, minAbilityDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);

    }
}
