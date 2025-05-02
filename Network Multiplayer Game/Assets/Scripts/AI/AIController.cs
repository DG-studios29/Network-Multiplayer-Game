using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    private NavMeshAgent agent; // Reference to the NavMeshAgent component

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Optional: Turn off auto rotation if you want to manually control it
        agent.updateRotation = false;
    }

    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);

            if (agent.velocity.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);

                // Add 180 degrees rotation on Y axis to fix backward shark
                targetRotation *= Quaternion.Euler(0, 180f, 0);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

}
