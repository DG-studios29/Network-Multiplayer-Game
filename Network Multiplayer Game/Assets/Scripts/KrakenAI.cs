using UnityEngine;
using UnityEngine.AI;

public class KrakenAI : MonoBehaviour
{
    public float detectionRadius = 15f;
    public float speed = 3.5f;
    public float attackRange = 2f;
    public float attackDamage = 50f; // Not used yet
    public float liftedHeight = 5f;
    public float normalHeight = -10f; // Base offset as you said
    public float liftSpeed = 2f; // Speed at which Kraken moves up/down
    public float attackDuration = 2f; // How long the Kraken stays attacking

    public Transform[] waypoints;

    private Transform player;
    private NavMeshAgent agent;
    private int currentWaypoint = 0;

    private bool isAttacking = false;
    private bool isLifting = false;
    private float targetHeight;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();

        agent.speed = speed;
        agent.updateRotation = false;

        MoveToNextWaypoint();

        // Set initial height
        Vector3 startPos = transform.position;
        startPos.y = normalHeight;
        transform.position = startPos;
    }

    void Update()
    {
        if (player == null) return; // Safety check

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        bool isMoving = agent.velocity.sqrMagnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);

        if (!isAttacking)
        {
            if (distanceToPlayer <= detectionRadius)
            {
                agent.SetDestination(player.position);

                if (distanceToPlayer <= attackRange)
                {
                    AttackPlayer();
                }
            }
            else
            {
                MoveToNextWaypoint();
            }
        }

        // Handle smooth lifting or dropping
        if (isLifting)
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(pos.y, targetHeight, Time.deltaTime * liftSpeed);
            transform.position = pos;
        }
    }

    void MoveToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(waypoints[currentWaypoint].position);
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }

    void AttackPlayer()
    {
        isAttacking = true;
        animator.SetBool("isAttacking", true);

        LiftKraken(true);

        Invoke(nameof(ResetAttack), attackDuration);
    }

    void ResetAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);

        LiftKraken(false);
    }

    void LiftKraken(bool lift)
    {
        isLifting = true;
        targetHeight = lift ? liftedHeight : normalHeight;
    }
}
