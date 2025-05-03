using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class SharkAI : NetworkBehaviour
{
    public float chaseRange = 100f;
    public float attackRange = 3f;
    public Transform[] patrolPoints;

    private NavMeshAgent agent;
    private Animator animator;
    private int currentPatrolIndex;
    private Transform targetPlayer;

    private bool isChasing;
    private bool isAttacking;

    public float patrolOffset = -2f;
    public float attackOffset = 1f;

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

        float distance = Vector3.Distance(transform.position, targetPlayer.position);

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
        agent.baseOffset = Mathf.Lerp(agent.baseOffset, targetOffset, Time.deltaTime * 2f);

        if (isAttacking)
        {
            Vector3 dir = (targetPlayer.position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                Quaternion look = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 5f);
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
        float minDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject go in players)
        {
            float dist = Vector3.Distance(go.transform.position, transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = go.transform;
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
