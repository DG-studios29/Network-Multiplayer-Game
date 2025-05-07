using UnityEngine;
using UnityEngine.AI;
using Mirror;

[RequireComponent(typeof(NetworkAnimator))]
[RequireComponent(typeof(NetworkTransformReliable))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerTracker))]
public class SharkAI : NetworkBehaviour
{
    public Transform[] patrolPoints;

    public int attackDamage = 10;
    public float attackCooldown = 5f;

    public float chaseRange = 100f;
    public float attackRange = 3f;
    public float attackOffset = 1.5f;
    public float chaseOffset = -2f;
    public float offsetSmoothSpeed = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private NetworkAnimator netAnimator;
    private PlayerTracker tracker;

    private int currentPatrolIndex;
    private bool hasAttacked;
    private bool isReturningToPortal;
    private bool isChasing;
    private bool isAttacking;
    private bool isDistracted = false;
    private float distractionTimer = 0f;
    private float targetOffset;
    private float lastAttackTime = 0f;

    void Start()
    {
        if (!isServer) return;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        netAnimator = GetComponent<NetworkAnimator>();
        tracker = GetComponent<PlayerTracker>();

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning($"{name} is not on NavMesh at Start()", this);
            return;
        }

        GoToNextPatrolPoint();
    }

    void Update()
    {
        if (!isServer || !IsAgentReady()) return;

        if (isDistracted)
        {
            distractionTimer -= Time.deltaTime;
            if (distractionTimer <= 0f)
                isDistracted = false;
            else
                return;
        }

        if (tracker.currentTarget == null || tracker.playerHealth == null) return;

        bool playerIsDead = tracker.playerHealth.currentHealth <= 0;
        if (playerIsDead && !isReturningToPortal)
        {
            ResetAttack();
            GoToNearestPortal();
            isReturningToPortal = true;
            chaseOffset = -14f;
        }

        targetOffset = isAttacking ? attackOffset : chaseOffset;
        agent.baseOffset = Mathf.Lerp(agent.baseOffset, targetOffset, Time.deltaTime * offsetSmoothSpeed);

        if (playerIsDead) return;

        float dist = Vector3.Distance(transform.position, tracker.currentTarget.position);

        if (dist <= attackRange)
        {
            isAttacking = true;
            isChasing = false;
            agent.SetDestination(transform.position);

            if (!hasAttacked && Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                hasAttacked = true;
                netAnimator.SetTrigger("Attack");
                tracker.playerHealth.TakeDamage(attackDamage);
            }
        }
        else if (dist <= chaseRange)
        {
            isAttacking = false;
            isChasing = true;
            agent.SetDestination(tracker.currentTarget.position);
            agent.speed = 15f;
        }
        else
        {
            isAttacking = false;
            isChasing = false;
            agent.speed = 10f;

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                GoToNextPatrolPoint();
        }

        if (agent.velocity != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(agent.velocity.normalized) * Quaternion.Euler(0, 180f, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }

        animator.SetBool("isChasing", isChasing);
    }

    void ResetAttack()
    {
        isChasing = false;
        isAttacking = false;
        hasAttacked = false;
        animator.ResetTrigger("Attack");
        animator.SetBool("isChasing", false);
    }

    public void Distract(float duration)
    {
        if (!IsAgentReady()) return;

        isDistracted = true;
        distractionTimer = duration;
        agent.SetDestination(transform.position);
        ResetAttack();
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0 || !IsAgentReady()) return;

        currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void GoToNearestPortal()
    {
        if (patrolPoints.Length == 0 || !IsAgentReady()) return;

        Transform nearest = patrolPoints[0];
        float minDist = Vector3.Distance(transform.position, nearest.position);
        foreach (var point in patrolPoints)
        {
            float d = Vector3.Distance(transform.position, point.position);
            if (d < minDist)
            {
                nearest = point;
                minDist = d;
            }
        }
        agent.SetDestination(nearest.position);
        isChasing = false;
    }

    private bool IsAgentReady()
    {
        return agent != null && agent.isOnNavMesh && agent.isActiveAndEnabled;
    }
}
