using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NetworkAnimator))]
[RequireComponent(typeof(NavMeshAgent))]
public class KrakenAI : NetworkBehaviour
{
    [Header("Waypoints")]
    public Transform[] patrolPoints;

    [Header("Player Info")]
    private Transform player;
    private PlayerHealthUI playerHealth;

    [Header("Attacking")]
    public int attackDamage = 30;
    public float attackCooldown = 1.5f;
    public float chaseRange = 100f;
    public float attackRange = 5f;
    public float attackOffset = 2f;
    public float patrolOffset = -3f;
    public float offsetLerpSpeed = 2f;

    private float lastAttackTime;
    private bool hasAttacked = false;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isReturningToPortal = false;
    private float targetOffset;

    private NavMeshAgent agent;
    private Animator animator;
    private NetworkAnimator netAnimator;
    private KrakenHealth krakenHealth;
    private int currentPatrolIndex;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        netAnimator = GetComponent<NetworkAnimator>();
        krakenHealth = GetComponent<KrakenHealth>();

        GoToNextPatrolPoint();

        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer)
        {
            player = foundPlayer.transform;
            playerHealth = player.GetComponent<PlayerHealthUI>();
        }
    }

    void Update()
    {
        if (!isServer) return; // Only server handles AI logic

        if (player == null || playerHealth == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer)
            {
                player = foundPlayer.transform;
                playerHealth = player.GetComponent<PlayerHealthUI>();
            }
            return;
        }

        bool playerIsDead = playerHealth.currentHealth <= 0;

        // Handle returning to portal if player is dead
        if (playerIsDead)
        {
            if (!isReturningToPortal)
            {
                ResetAttackState();
                GoToNearestPortal();
                isReturningToPortal = true;
                patrolOffset = -14f;
            }
        }

        // Smooth vertical movement
        targetOffset = isAttacking ? attackOffset : patrolOffset;
        agent.baseOffset = Mathf.Lerp(agent.baseOffset, targetOffset, Time.deltaTime * offsetLerpSpeed);

        if (playerIsDead) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= attackRange)
        {
            isAttacking = true;
            isChasing = false;
            agent.SetDestination(transform.position);

            if (!hasAttacked && Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                hasAttacked = true;

                netAnimator.SetTrigger("Attack"); // Sync across clients
                StartCoroutine(DelayedAttack());  // Deal damage after animation starts
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
            ResetAttackState();

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                GoToNextPatrolPoint();
        }

        // Rotate Kraken
        Vector3 direction = isAttacking ? (player.position - transform.position).normalized :
                              agent.velocity.sqrMagnitude > 0.01f ? agent.velocity.normalized :
                              transform.forward;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        animator.SetBool("isChasing", isChasing);
    }

    IEnumerator DelayedAttack()
    {
        yield return new WaitForSeconds(0.6f); // Match your animation's hit timing
        if (player != null && !playerHealth.currentHealth.Equals(0))
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    void ResetAttackState()
    {
        isChasing = false;
        isAttacking = false;
        hasAttacked = false;
        animator.ResetTrigger("Attack");
        animator.SetBool("isChasing", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        if (other.CompareTag("Cannonball"))
        {
            krakenHealth?.TakeDamage(20);
            Destroy(other.gameObject);
        }
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
        float nearestDist = Vector3.Distance(transform.position, nearest.position);

        foreach (Transform point in patrolPoints)
        {
            float dist = Vector3.Distance(transform.position, point.position);
            if (dist < nearestDist)
            {
                nearest = point;
                nearestDist = dist;
            }
        }

        agent.SetDestination(nearest.position);
        isChasing = false;
    }
}
