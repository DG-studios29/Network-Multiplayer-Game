using UnityEngine;
using UnityEngine.AI;

public class SharkAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform player;
    public float chaseRange = 100f;
    public float attackRange = 3f;
    public float attackOffset = 0f; // Shark rises up
    public float chaseOffset = -2f;   // Shark stays under water
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
        GoToNextPatrolPoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // State logic
        if (distanceToPlayer <= attackRange)
        {
            isAttacking = true;
            isChasing = false;
            agent.SetDestination(transform.position); // stop
        }
        else if (distanceToPlayer <= chaseRange)
        {
            isAttacking = false;
            isChasing = true;
            agent.SetDestination(player.position);
            agent.speed = 10f;
        }
        else
        {
            isAttacking = false;
            isChasing = false;
            agent.speed = 5f;

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                GoToNextPatrolPoint();
        }

        // Handle rotation
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            targetRotation *= Quaternion.Euler(0, 180f, 0); // backward fix
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // Smoothly transition baseOffset
        float targetOffset = isAttacking ? attackOffset : chaseOffset;
        agent.baseOffset = Mathf.Lerp(agent.baseOffset, targetOffset, Time.deltaTime * offsetSmoothSpeed);

        // Animator updates
        animator.SetBool("isChasing", isChasing);
        animator.SetBool("isAttacking", isAttacking);
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }
}
