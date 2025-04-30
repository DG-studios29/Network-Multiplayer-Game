using UnityEngine;
using UnityEngine.AI;

public class KrakenAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 15f;
    public float stoppingDistance = 3f;
    public Transform[] waypoints;
    public float patrolWaitTime = 2f;

    private NavMeshAgent agent;
    public Animator animator;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private float distanceToPlayer;

    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GoToNextWaypoint();
    }

    private void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Determine state
        if (distanceToPlayer <= stoppingDistance + 0.5f)
        {
            currentState = State.Attack;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            currentState = State.Chase;
        }
        else
        {
            currentState = State.Patrol;
        }

        // Handle states
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                agent.baseOffset = -14f;
                break;
            case State.Chase:
                Chase();
                agent.baseOffset = -14f;
                break;
            case State.Attack:
                Attack();
                agent.baseOffset = -7f;
                break;
        }

        // Rotate to face movement direction
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void Patrol()
    {
        agent.isStopped = false;
        animator.SetBool("isMoving", true);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= patrolWaitTime)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                GoToNextWaypoint();
                waitTimer = 0f;
            }
        }
    }

    private void Chase()
    {
        agent.isStopped = false;
        agent.speed = 8f;
        agent.SetDestination(player.position);
        animator.SetBool("isMoving", true);
        
    }

    private void Attack()
    {
        agent.isStopped = true;
        transform.LookAt(player);
        animator.SetBool("isMoving", false);
        animator.SetTrigger("Attack"); // Assumes animation has Attack trigger
    }

    private void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }
}
