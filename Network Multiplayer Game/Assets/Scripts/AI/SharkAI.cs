using UnityEngine;
using UnityEngine.AI;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class SharkAI : NetworkBehaviour
{
    [Header("Waypoints")]
    public Transform[] patrolPoints;

    [Header("Player Info")]
    public Transform player;
    private PlayerHealth playerHealth;
    private float lastAttackTime;
    public int attackDamage = 10;
    public float attackCooldown = 5f;
    private bool playerIsDead = false;

    [Header("Attacking Setup")]
    public float chaseRange = 100f;
    public float attackRange = 3f;
    public float attackOffset = 1.5f;
    public float chaseOffset = -2f;
    public float offsetSmoothSpeed = 2f;

    [Header("A.I Setup")]
    private NavMeshAgent agent;
    private Animator animator;
    private int currentPatrolIndex;
    private bool hasAttacked;
    private bool isReturningToPortal;
    private bool isChasing;
    private bool isAttacking;

    private float targetOffset;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        GoToNextPatrolPoint();

        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
    }

    [ServerCallback]
    void Update()
    {
        if (player == null || playerHealth == null)
            return;

        // Check if the player is already dead
        playerIsDead = playerHealth.currentHealth <= 0;
        if (playerIsDead && !isReturningToPortal)
        {
            animator.ResetTrigger("Attack");
            animator.SetBool("isChasing", false);
            isChasing = isAttacking = hasAttacked = false;
            GoToNearestPortal();
            isReturningToPortal = true;
            chaseOffset = -14f;
        }

        // Always lerp the baseOffset for that swoop effect
        targetOffset = isAttacking ? attackOffset : chaseOffset;
        agent.baseOffset = Mathf.Lerp(agent.baseOffset, targetOffset, Time.deltaTime * offsetSmoothSpeed);

        // Stop all further AI if the player’s dead
        if (playerIsDead)
            return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            // Attack
            isAttacking = true;
            isChasing = false;
            agent.SetDestination(transform.position);

            if (!hasAttacked && Time.time - lastAttackTime >= attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
                playerHealth.TakeDamage(attackDamage);  // server?only
            }
        }
        else if (dist <= chaseRange)
        {
            // Chase
            isAttacking = false;
            isChasing = true;
            hasAttacked = false;
            agent.speed = 15f;
            agent.SetDestination(player.position);
        }
        else
        {
            // Patrol
            isAttacking = isChasing = false;
            agent.speed = 10f;

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                GoToNextPatrolPoint();
        }

        // Smooth rotation facing travel direction (flip 180° so the shark faces forward)
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion look = Quaternion.LookRotation(agent.velocity.normalized) * Quaternion.Euler(0, 180, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 5f);
        }

        animator.SetBool("isChasing", isChasing);
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void GoToNearestPortal()
    {
        if (patrolPoints.Length == 0) return;

        Transform nearest = patrolPoints[0];
        float bestDist = Vector3.Distance(transform.position, nearest.position);

        for (int i = 1; i < patrolPoints.Length; i++)
        {
            float d = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (d < bestDist)
            {
                bestDist = d;
                nearest = patrolPoints[i];
            }
        }

        agent.SetDestination(nearest.position);
        isChasing = false;
    }
}
