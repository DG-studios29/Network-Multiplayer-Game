using UnityEngine;
using UnityEngine.AI;
using Mirror;
using System.Linq;

[RequireComponent(typeof(NetworkAnimator))]
public class SharkAI : NetworkBehaviour
{
    public Transform[] patrolPoints;

    private Transform player;
    private PlayerHealthUI playerHealth;
    private float lastAttackTime;
    public int attackDamage = 10;
    public float attackCooldown = 5f;
    public bool playerIsDead = false;

    public float chaseRange = 100f;
    public float attackRange = 3f;
    public float attackOffset = 1.5f;
    public float chaseOffset = -2f;
    public float offsetSmoothSpeed = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private NetworkAnimator netAnimator;

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
        netAnimator = GetComponent<NetworkAnimator>();
        GoToNextPatrolPoint();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        player = players.OrderBy(p => Vector3.Distance(p.transform.position, transform.position)).FirstOrDefault()?.transform;
        playerHealth = player?.GetComponent<PlayerHealthUI>();
    }

    void Update()
    {
        if (!isServer) return;
        if (isDistracted)
        {
            distractionTimer -= Time.deltaTime;
            if (distractionTimer <= 0f) isDistracted = false;
            else return;
        }

        if (player == null || playerHealth == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            player = players.OrderBy(p => Vector3.Distance(p.transform.position, transform.position)).FirstOrDefault()?.transform;
            playerHealth = player?.GetComponent<PlayerHealthUI>();
        }
        if (player == null || playerHealth == null) return;

        playerIsDead = playerHealth.currentHealth <= 0;
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

        float dist = Vector3.Distance(transform.position, player.position);

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
                playerHealth.TakeDamage(attackDamage);
            }
        }
        else if (dist <= chaseRange)
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
            if (!agent.pathPending && agent.remainingDistance < 0.5f) GoToNextPatrolPoint();
        }

        Quaternion targetRot = Quaternion.LookRotation(agent.velocity.normalized) * Quaternion.Euler(0, 180f, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);

        animator.SetBool("isChasing", isChasing);
    }

    void ResetAttack()
    {
        isChasing = false; isAttacking = false; hasAttacked = false;
        animator.ResetTrigger("Attack");
        animator.SetBool("isChasing", false);
    }

    public void Distract(float duration)
    {
        isDistracted = true;
        distractionTimer = duration;
        agent.SetDestination(transform.position);
        ResetAttack();
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
        float minDist = Vector3.Distance(transform.position, nearest.position);
        foreach (var point in patrolPoints)
        {
            float d = Vector3.Distance(transform.position, point.position);
            if (d < minDist) { nearest = point; minDist = d; }
        }
        agent.SetDestination(nearest.position);
        isChasing = false;
    }
}