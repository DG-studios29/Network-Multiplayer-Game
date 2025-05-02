using UnityEngine;

public class CannonDefence : MonoBehaviour
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
        if (player == null)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Player");
            if (target) player = target.transform;
            else return;
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
        if (rb != null)
        {
            Vector3 toTarget = (player.position + Vector3.up * 2f) - firePoint.position;
            rb.linearVelocity = toTarget.normalized * Random.Range(20f, 30f) + Vector3.up * Random.Range(2f, 5f) * fireForce;

        }




        //ParticleSystem muzzleFlash = firePoint.GetComponentInChildren<ParticleSystem>();
        //muzzleFlash?.Play();
    }

    private bool CanSeePlayer(Transform target)
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > detectionRadius) return false;

        float angle = Vector3.Angle(transform.forward, dirToTarget);
        if (angle > detectionRadius / 2f) return false;

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
