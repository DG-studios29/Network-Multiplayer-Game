using UnityEngine;
using UnityEngine.AI;
using Mirror;


public class SharkAI : NetworkBehaviour
{
    public Transform[] patrolPoints;
    public Transform player;
    public float chaseRange = 100f;
    public float attackRange = 3f;
    public float attackOffset = 1.5f;
    public float chaseOffset = -2f;
    public float offsetSmoothSpeed = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private int currentPatrolIndex;
    private bool isChasing;
    private bool isAttacking;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (isServer)
            GoToNextPatrolPoint();
    }

    void Update()
    {
        if (!isServer) return; // AI logic only runs on the server

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // State transitions
        if (distanceToPlayer <= attackRange)
        {
            isAttacking = true;
            isChasing = false;
            agent.SetDestination(transform.position);
        }
        else if (distanceToPlayer <= chaseRange)
        {
            isAttacking = false;
            isChasing = true;
            agent.SetDestination(player.position);
            agent.speed = 15f;
        }
        else
        {
            isAttacking = false;
            isChasing = false;
            agent.speed = 5f;

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                GoToNextPatrolPoint();
        }

        // Rotation and breaching logic
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            targetRotation *= Quaternion.Euler(0, 180f, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        float targetOffset = isAttacking ? attackOffset : chaseOffset;
        agent.baseOffset = Mathf.Lerp(agent.baseOffset, targetOffset, Time.deltaTime * offsetSmoothSpeed);

        RpcUpdateAnimation(isChasing, isAttacking);
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    [ClientRpc]
    void RpcUpdateAnimation(bool chasing, bool attacking)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator is missing on KrakenAI!");
                return;
            }
        }

        animator.SetBool("isChasing", chasing);
        animator.SetBool("isAttacking", attacking);
    }
}
