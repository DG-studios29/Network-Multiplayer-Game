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
    private Transform player;

    void Update()
    {
        if (!isServer) return;

        if (player == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            float closestDistance = Mathf.Infinity;
            foreach (var go in players)
            {
                float dist = Vector3.Distance(go.transform.position, transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    player = go.transform;
                }
            }
            if (player == null) return;
        }

        Vector3 dirToPlayer = player.position - transform.position;
        float distance = dirToPlayer.magnitude;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (CanSeePlayer(player.transform))
        {
            if (distance < detectionRadius && angle < fieldOfView / 2f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dirToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                fireTimer += Time.deltaTime;
                if (fireTimer >= fireInterval)
                {
                    FireCannon();
                    fireTimer = 0f;
                }
            }
        }
    }

    void FireCannon()
    {
        if (!projectilePrefab || !firePoint) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = proj.GetComponent<Rigidbody>();

        if (rb != null && player != null)
        {
            Vector3 toTarget = (player.position + Vector3.up * 2f) - firePoint.position;
            rb.linearVelocity = toTarget.normalized * Random.Range(20f, 30f) + Vector3.up * Random.Range(2f, 5f) * fireForce;
        }

        NetworkServer.Spawn(proj);
    }

    private bool CanSeePlayer(Transform target)
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > detectionRadius) return false;

        float angle = Vector3.Angle(transform.forward, dirToTarget);
        if (angle > fieldOfView / 2f) return false;

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