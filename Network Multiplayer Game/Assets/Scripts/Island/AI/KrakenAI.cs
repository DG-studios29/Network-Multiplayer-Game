using UnityEngine;
using UnityEngine.AI;

public class KrakenAI : MonoBehaviour
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
        GoToNextPatrolPoint();
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

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

        // Rotate to movement direction
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion look = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 5f);
        }

        // Animator parameters
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
