using Mirror.Examples.AdditiveLevels;
using UnityEngine;
using UnityEngine.AI;

public class KrakenAI : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] patrolPoints;

    [Header("Player Info")]
    public Transform player;
    PlayerHealth playerHealth;
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
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if ((player == null || playerHealth == null))
        {
            return;
        }

        playerIsDead = playerHealth.currentHealth <= 0;
        if (playerIsDead)
        {
            if (!isReturningToPortal)
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
        }

        // Smoothly adjust base offset always (even when dead)
        targetOffset = isAttacking ? attackOffset : patrolOffset;
        agent.baseOffset = Mathf.Lerp(agent.baseOffset, targetOffset, Time.deltaTime * offsetLerpSpeed);

        // Don't run further AI logic if dead
        if (playerIsDead)
            return;


        float distance = Vector3.Distance(player.position, transform.position);
        Vector3 direction;
        if ((player == null || playerHealth == null))
        {
            return;
        }
       
        // State logic
        if (distance <= attackRange)
        {
            isAttacking = true;
            isChasing = false;
            agent.SetDestination(transform.position);
            if (!hasAttacked)
            {
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    animator.SetTrigger("Attack");
                    lastAttackTime = Time.time;
                    playerHealth.TakeDamage(attackDamage);
                }
            }
        }
        else if (distance <= chaseRange)
        {
            isChasing = true;
            isAttacking = false;
            hasAttacked = false;
            agent.SetDestination(player.position);
        }
        else
        {
            isChasing = false;
            isAttacking = false;
            hasAttacked = false;
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                GoToNextPatrolPoint();
        }

        // Smoothly adjust base offset for breaching effect
        targetOffset = isAttacking ? attackOffset : patrolOffset;
        agent.baseOffset = Mathf.Lerp(agent.baseOffset, targetOffset, Time.deltaTime * offsetLerpSpeed);


        if (isChasing)
        {
            direction = (player.position - transform.position).normalized;
        }
        else if (agent.velocity.sqrMagnitude> 0.01f)
        {
            direction = agent.velocity.normalized;
        }
        else
        {
            direction = transform.forward;
        }

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
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
