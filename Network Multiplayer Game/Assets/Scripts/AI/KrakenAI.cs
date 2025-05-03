using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class KrakenAI : NetworkBehaviour
{
    public Transform[] patrolPoints;
    public float chaseRange = 100f;
    public float attackRange = 5f;
    public float attackOffset = 2f;
    public float patrolOffset = -3f;
    public float offsetLerpSpeed = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private int currentPatrolIndex;
    private Transform targetPlayer;
    private bool isChasing;
    private bool isAttacking;

    public override void OnStartServer()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        GoToNextPatrolPoint();
    }

    [ServerCallback]
    void Update()
    {
        if (!isServer) return;

        targetPlayer = GetClosestPlayer();
        if (targetPlayer == null) return;

        float distance = Vector3.Distance(targetPlayer.position, transform.position);

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
            agent.SetDestination(targetPlayer.position);
        }
        else
        {
            isAttacking = false;
            isChasing = false;
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                GoToNextPatrolPoint();
        }

        float targetOffset = isAttacking ? attackOffset : patrolOffset;
        agent.baseOffset = Mathf.Lerp(agent.baseOffset, targetOffset, Time.deltaTime * offsetLerpSpeed);

        if (isAttacking)
        {
            Vector3 dir = (targetPlayer.position - transform.position).normalized;
            dir.y = 0f;
            if (dir != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
            }
        }
        else if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion look = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 5f);
        }

        animator.SetBool("isChasing", isChasing);
        animator.SetBool("isAttacking", isAttacking);
    }

    Transform GetClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject p in players)
        {
            float dist = Vector3.Distance(p.transform.position, transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = p.transform;
            }
        }

        return closest;
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }
}
