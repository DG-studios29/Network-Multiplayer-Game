using UnityEngine;
using Mirror;


public class CannonDefence : NetworkBehaviour
{
    public float detectionRadius = 70f;
    public float fieldOfView = 150f;
    public float rotationSpeed = 5f;
    public float fireInterval = 3f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireForce = 2f;
    public LayerMask visionBlockingLayers;

    private float fireTimer;

    void Update()
    {
        if (!isServer) return; // Only the server handles AI logic

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject playerObj in players)
        {
            Transform player = playerObj.transform;
            if (!CanSeePlayer(player)) continue;

            Vector3 dirToPlayer = player.position - transform.position;
            float distance = dirToPlayer.magnitude;
            float angle = Vector3.Angle(transform.forward, dirToPlayer);

            if (distance < detectionRadius && angle < fieldOfView / 2f)
            {
                // Rotate toward the player
                Quaternion targetRotation = Quaternion.LookRotation(dirToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                fireTimer += Time.deltaTime;
                if (fireTimer >= fireInterval)
                {
                    FireCannon(player);
                    fireTimer = 0f;
                }

                break; // Only fire at one player per frame
            }
        }
    }

    void FireCannon(Transform target)
    {
        if (!projectilePrefab || !firePoint) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = proj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 toTarget = (target.position + Vector3.up * 2f) - firePoint.position;
            rb.linearVelocity = toTarget.normalized * Random.Range(20f, 30f) + Vector3.up * Random.Range(2f, 5f) * fireForce;
        }

        // Spawn cannonball over the network
        NetworkServer.Spawn(proj);
    }

    private bool CanSeePlayer(Transform target)
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > detectionRadius) return false;

        float angle = Vector3.Angle(transform.forward, dirToTarget);
        if (angle > fieldOfView / 2f) return false;

        // Check line of sight
        if (Physics.Raycast(transform.position, dirToTarget, out RaycastHit hit, detectionRadius, visionBlockingLayers))
        {
            if (hit.transform != target) return false;
        }

        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 rightLimit = Quaternion.Euler(0, fieldOfView / 2f, 0) * transform.forward;
        Vector3 leftLimit = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, rightLimit * detectionRadius);
        Gizmos.DrawRay(transform.position, leftLimit * detectionRadius);
    }
}
