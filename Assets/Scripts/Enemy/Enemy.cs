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
    public float idleTimer;

    [Header("Move data")]
    public float walkSpeed;
    public float chaseSpeed;
    public float turnSpeed;

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

    public virtual void GetHit()
    {
        healthPoints--;
    }

    public virtual void HitImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(HitImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator HitImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(0.01f);

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    public bool PlayerOutMaxChaseRange() => Vector3.Distance(transform.position, Player.instance.transform.position) > maxChaseRange;

    public bool PlayerInAggresionRange() => Vector3.Distance(transform.position, Player.instance.transform.position) < aggresionRange;



    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxChaseRange);

    }

    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;

    public bool ManualMovementActive() => manualMovement;

    public void ActivateManualRotate(bool manualRotate) => this.manualRotation = manualRotate;

    public bool ManualRotateActive() => manualRotation;

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
