using UnityEngine;
using UnityEngine.AI;
using Mirror;


public class KrakenAI : NetworkBehaviour
{
    public Transform[] patrolPoints;
    public Transform player;
    public float chaseRange = 100f;
    public float attackRange = 5f;
    public float attackOffset = 2f;
    public float patrolOffset = -3f;
    public float offsetLerpSpeed = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private int currentPatrolIndex;
    private bool isAttacking;
    private bool isChasing;

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

        float distance = Vector3.Distance(player.position, transform.position);

        // State logic
        if (distance <= attackRange)
        {
            isAttacking = true;
            isChasing = false;
            agent.SetDestination(transform.position); // Stop
        }
        else if (distance <= chaseRange)
        {
            isAttacking = false;
            isChasing = true;
            agent.SetDestination(player.position);
        }
        else
        {
            isAttacking = false;
            isChasing = false;

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                GoToNextPatrolPoint();
        }

        // Smoothly adjust base offset for breaching effect
        float targetOffset = isAttacking ? attackOffset : patrolOffset;
        agent.baseOffset = Mathf.Lerp(agent.baseOffset, targetOffset, Time.deltaTime * offsetLerpSpeed);

        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion look = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 5f);
        }

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
