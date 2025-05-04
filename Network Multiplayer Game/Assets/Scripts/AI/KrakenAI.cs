using Mirror;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NetworkIdentity))]
public class KrakenAI : NetworkBehaviour
{
    [Header("Waypoints")]
    public Transform[] patrolPoints;

    [Header("Player Info")]
    public Transform player;
    PlayerHealthUI playerHealth;
    private float lastAttackTime;
    public int attackDamage = 30;
    public float attackCooldown = 1.5f;
    public bool playerIsDead = false;

    [Header("Attacking Setup")]
    public float chaseRange = 100f;
    public float attackRange = 5f;
    public float attackOffset = 2f;
    public float patrolOffset = -3f;
    public float offsetLerpSpeed = 2f;

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
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerHealth = player.GetComponent<PlayerHealthUI>();
            }
            else
            {
                Debug.LogWarning("KrakenAI: Player object not found with tag 'Player'");
            }
        }
        else
        {
            playerHealth = player.GetComponent<PlayerHealthUI>();
        }
    }

    void Update()
    {
        if (player == null || playerHealth == null)
        {
            return;
        }

        playerIsDead = playerHealth.currentHealth <= 0;
        if (playerIsDead && !isReturningToPortal)
        {
            animator.ResetTrigger("Attack");
            animator.SetBool("isChasing", false);
            isChasing = false;
            isAttacking = false;
            hasAttacked = false;
            GoToNearestPortal();
            isReturningToPortal = true;
            patrolOffset = -14f;
        }

        // Smoothly adjust base offset always (even when dead)
        targetOffset = isAttacking ? attackOffset : patrolOffset;
        agent.baseOffset = Mathf.Lerp(agent.baseOffset, targetOffset, Time.deltaTime * offsetLerpSpeed);

        if (playerIsDead)
            return;

      
        float distance = Vector3.Distance(player.position, transform.position);
        Vector3 direction = Vector3.zero;

        if (distance <= attackRange)
        {
            // Attack logic
            isAttacking = true;
            isChasing = false;
            agent.SetDestination(transform.position);
            if (!hasAttacked && Time.time - lastAttackTime >= attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
                playerHealth.TakeDamage(attackDamage);
            }
        }
        else if (distance <= chaseRange)
        {
            // Chasing logic
            isChasing = true;
            isAttacking = false;
            hasAttacked = false;
            agent.SetDestination(player.position);
        }
        else
        {
            // Patrol logic
            isChasing = false;
            isAttacking = false;
            hasAttacked = false;
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                GoToNextPatrolPoint();
        }

        // Smooth rotation
        direction = (isChasing) ? (player.position - transform.position).normalized :
                                  (agent.velocity.sqrMagnitude > 0.01f) ? agent.velocity.normalized : transform.forward;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
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

        // Find the nearest portal by comparing distances
        Transform nearestPortal = patrolPoints[0];
        float nearestDistance = Vector3.Distance(transform.position, patrolPoints[0].position);

        for (int i = 1; i < patrolPoints.Length; i++)
        {
            float distanceToPortal = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (distanceToPortal < nearestDistance)
            {
                nearestPortal = patrolPoints[i];
                nearestDistance = distanceToPortal;
            }
        }

        agent.SetDestination(nearestPortal.position);
        isChasing = false;
    }
}
