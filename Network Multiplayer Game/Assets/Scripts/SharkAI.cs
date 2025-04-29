using UnityEngine;
using UnityEngine.AI;

public class SharkAI : MonoBehaviour
{
    public float detectionRadius = 10f; 
    public float speed = 3.5f;          
    public float chaseSpeed = 5f;       
    public Transform[] waypoints;       
    private Transform player;           
    private NavMeshAgent agent;         
    private int currentWaypoint = 0;    
    private bool isChasing = false;     
    private bool isAttacking = false;   
    private bool isMoving = false;      

    private Animator animator;          

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform; 
        animator = GetComponent<Animator>();
        agent.speed = speed;
        MoveToNextWaypoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isMoving = agent.velocity.sqrMagnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            targetRotation *= Quaternion.Euler(0, 180f, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        if (distanceToPlayer <= detectionRadius && !isChasing)
        {
            // Start chasing the player
            isChasing = true;
            agent.speed = chaseSpeed;
            animator.SetBool("isChase", true);  
            animator.SetBool("isAttacking", false); 
        }
        else if (distanceToPlayer > detectionRadius && isChasing)
        {
            // Stop chasing and return to patrolling
            isChasing = false;
            agent.speed = speed;
            animator.SetBool("isChase", false);  // Set isChase to false
            MoveToNextWaypoint();  // Start patrolling again
        }

        if (isChasing)
        {
            // Chase the player
            agent.SetDestination(player.position);
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // If the shark is not chasing, move to the next waypoint
            MoveToNextWaypoint();
        }
    }

    void MoveToNextWaypoint()
    {
        if (waypoints.Length == 0)
            return;

        agent.SetDestination(waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    
    void Attack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= 2f) 
        {
            isAttacking = true;
            animator.SetBool("isAttacking", true);  
        }
        else
        {
            isAttacking = false;
            animator.SetBool("isAttacking", false);
        }
    }
}
