using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        
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

                
                targetRotation *= Quaternion.Euler(0, 180f, 0);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

}
