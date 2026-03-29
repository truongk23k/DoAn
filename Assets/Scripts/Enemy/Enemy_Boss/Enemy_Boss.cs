using UnityEngine;

public class Enemy_Boss : Enemy
{
    public float attackRange;

    [Header("Ability")]
    public float flamethrowDuration;

    [Header("Jump attack")]
    public float jumpAttackCooldown = 10;
    private float lastTimeJumped;
    public float travelTimeToTarget = 1f;
    public float minJumpDistanceRequired;
    [Space]
    [SerializeField] private LayerMask whatToIgnore;
    public IdleState_Boss idleState { get; private set; }
    public MoveState_Boss moveState { get; private set; }
    public AttackState_Boss attackState { get; private set; }
    public JumpAttackState_Boss jumpAttackState { get; private set; }
    public AbilityState_Boss abilityState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Boss(this, stateMachine, "Idle");
        moveState = new MoveState_Boss(this, stateMachine, "Move");
        attackState = new AttackState_Boss(this, stateMachine, "Attack");
        jumpAttackState = new JumpAttackState_Boss(this, stateMachine, "JumpAttack");
        abilityState = new AbilityState_Boss(this, stateMachine, "Ability");
    }

    override protected void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    override protected void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.V))
        {
            stateMachine.ChangeState(abilityState);
        }

        stateMachine.currentState.Update();

        if (ShouldEnterBattleMode())
            EnterBattleMode();
    }

    public override void EnterBattleMode()
    {
        /*base.EnterBattleMode();
        stateMachine.ChangeState(moveState);*/
    }

    public void ActivateFlamethrower(bool activate)
    {
        if(!activate)
        {
            anim.SetTrigger("StopFlamethrower");
            return;
        }

        //activate particles
    }

    public bool CanJumpAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.instance.transform.position);

        if (distanceToPlayer < minJumpDistanceRequired)
            return false;

        if (Time.time >= lastTimeJumped + jumpAttackCooldown && IsPlayerInClearSight())
        {
            lastTimeJumped = Time.time;
            return true;
        }

        return false;
    }

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
    }
}
