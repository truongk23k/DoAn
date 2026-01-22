using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int healthPoints = 20;

    private bool manualMovement;
    private bool manualRotation;

    [Header("Chase info")]
    public float maxChaseRange;

    [Header("Recovery")]
    public float aggresionRange;

    [Header("Idle data")]
    public float idleTime;

    [Header("Move data")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;
    public float turnSpeed;

    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] patrolPointsPosition;
    private int currentPatrolIndex;

    public bool inBattleMode { get; private set; }

    public Animator anim { get; private set; }

    public NavMeshAgent agent { get; private set; }

    public EnemyStateMachine stateMachine { get; private set; }

    public Enemy_Visuals visuals { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        visuals = GetComponent<Enemy_Visuals>();

        agent = GetComponent<NavMeshAgent>();
        // Tắt auto traverse để tự xử lý nhảy
        agent.autoTraverseOffMeshLink = false;

        anim = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }

    protected virtual void Update()
    {
        // Xử lý Off-Mesh Link
        if (agent.isOnOffMeshLink && !agent.isStopped)
        {
            HandleOffMeshLink();
        }

        stateMachine.currentState.Update();

        if (ShouldEnterBattleMode())
        {
            EnterBattleMode();
        }
    }

    private void HandleOffMeshLink()
    {
        // Lấy thông tin Off-Mesh Link
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos;

        // Di chuyển mượt từ startPos đến endPos
        float speed = agent.speed;
        agent.transform.position = Vector3.MoveTowards(startPos, endPos, speed * Time.deltaTime);

        // Khi đến đích, hoàn thành Off-Mesh Link
        if (Vector3.Distance(agent.transform.position, endPos) < 0.1f)
        {
            agent.CompleteOffMeshLink();
        }
    }

    public virtual void EnterBattleMode()
    {
        inBattleMode = true;
    }

    protected bool ShouldEnterBattleMode()
    {
        bool inAggresionRange = Vector3.Distance(transform.position, Player.instance.transform.position) < aggresionRange;

        if (inAggresionRange && !inBattleMode)
        {
            return true;
        }

        return false;
    }

    public virtual void GetHit()
    {
        EnterBattleMode();

        healthPoints--;
    }

    public virtual void DeathImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(DeathImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator DeathImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(0.01f);

        Debug.Log("deadim");
        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    public bool PlayerOutMaxChaseRange()
    {
        if (Vector3.Distance(transform.position, Player.instance.transform.position) > maxChaseRange)
        {
            inBattleMode = false;
            return true;
        }

        return false;
    }

    public void FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngles = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
    }


    #region Animation events
    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;

    public bool ManualMovementActive() => manualMovement;

    public void ActivateManualRotate(bool manualRotate) => this.manualRotation = manualRotate;

    public bool ManualRotateActive() => manualRotation;

    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }
    #endregion

    #region Patrol logic
    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointsPosition[currentPatrolIndex];
        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }

    private void InitializePatrolPoints()
    {
        patrolPointsPosition = new Vector3[patrolPoints.Length];

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }
    }
    #endregion

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxChaseRange);

    }
}
