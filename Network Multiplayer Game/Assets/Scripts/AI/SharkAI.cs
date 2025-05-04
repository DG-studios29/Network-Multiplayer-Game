using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class SharkAI : NetworkBehaviour
{
    [Header("Waypoints")]
    public Transform[] patrolPoints;

    [Header("Player Info")]
    private Transform player;
    private PlayerHealthUI playerHealth;
    private float lastAttackTime;
    public int attackDamage = 10;
    public float attackCooldown = 5f;
    public bool playerIsDead = false;

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
    private bool isDistracted = false;
    private float distractionTimer = 0f;

    private float targetOffset;

    void Start()
    {
        if (!isServer) return;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        GoToNextPatrolPoint();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = player.GetComponent<PlayerHealthUI>();
        }
    }

    void Update()
    {
        if (!isServer) return;
        
        if (isDistracted)
        {
            distractionTimer -= Time.deltaTime;
            if (distractionTimer <= 0f)
            {
                isDistracted = false;
            }
            else
            {
                return; 
            }
        }

        if (player == null || playerHealth == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerHealth = player.GetComponent<PlayerHealthUI>();
            }

            if (player == null || playerHealth == null)
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
                chaseOffset = -14f;
            }
        }

        targetOffset = isAttacking ? attackOffset : chaseOffset;
        agent.baseOffset = Mathf.Lerp(agent.baseOffset, targetOffset, Time.deltaTime * offsetSmoothSpeed);

        if (playerIsDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
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
            agent.speed = 10f;

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                GoToNextPatrolPoint();
        }

        targetOffset = isAttacking ? attackOffset : chaseOffset;
        agent.baseOffset = Mathf.Lerp(agent.baseOffset, targetOffset, Time.deltaTime * offsetSmoothSpeed);

        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            targetRotation *= Quaternion.Euler(0, 180f, 0);
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

    public void Distract(float duration)
    {
        isDistracted = true;
        distractionTimer = duration;
        agent.SetDestination(transform.position);
        animator.SetBool("isChasing", false);
        animator.ResetTrigger("Attack");
    }

}
