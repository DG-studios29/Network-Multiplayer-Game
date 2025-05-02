using UnityEngine;

public class CannonDefence : MonoBehaviour
{
    public float detectionRadius = 70f;
    public float fieldOfView = 150f;
    public float rotationSpeed = 5f;
    public float fireInterval = 3f;
    public GameObject projectilePrefab;
    public Transform firePoint;

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

        // Check if in cone
        if (distance < detectionRadius && angle < fieldOfView / 2f)
        {
            // Rotate toward player
            Quaternion targetRotation = Quaternion.LookRotation(dirToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Fire timer
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireInterval)
            {
                FireCannon();
                fireTimer = 0f;
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
            // Add arc by adjusting upward force
            Vector3 toTarget = (player.position + Vector3.up * 1.5f) - firePoint.position;
            rb.linearVelocity = toTarget.normalized * Random.Range(20f, 30f) + Vector3.up * Random.Range(2f, 5f);
        }

        // Optional: play firing effects
        // e.g. ParticleSystem muzzleFlash = firePoint.GetComponentInChildren<ParticleSystem>();
        // muzzleFlash?.Play();
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
