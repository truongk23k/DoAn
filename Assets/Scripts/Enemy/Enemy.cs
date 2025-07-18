using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float turnSpeed;

    [Header("Attack data")]
    public float attackRange;
    public float attackMoveSpeed;
    private bool manualMovement;

    [Header("Chase info")]
    public float maxChaseRange;

    [Header("Recovery")]
    public float aggresionRange;

    [Header("Idle data")]
    public float idleTimer;

    [Header("Move data")]
    public float walkSpeed;
    public float chaseSpeed;

    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;

    public Animator anim { get; private set; }

    public NavMeshAgent agent { get; private set; }

    public EnemyStateMachine stateMachine { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        agent = GetComponent<NavMeshAgent>();

        anim = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }

    protected virtual void Update()
    {

    }

    public bool PlayerOutMaxChaseRange() => Vector3.Distance(transform.position, Player.instance.transform.position) > maxChaseRange;

    public bool PlayerInAggresionRange() => Vector3.Distance(transform.position, Player.instance.transform.position) < aggresionRange;

    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, Player.instance.transform.position) < attackRange;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxChaseRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;

    public bool ManualMovementActive() => manualMovement;

    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[currentPatrolIndex].transform.position;
        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }

    private void InitializePatrolPoints()
    {
        foreach (Transform t in patrolPoints)
        {
            t.parent = null;
        }
    }

    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngles = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        return Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
    }
}
