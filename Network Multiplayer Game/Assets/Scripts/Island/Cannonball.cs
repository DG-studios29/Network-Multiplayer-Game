using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkRigidbodyReliable))]
public class Cannonball : NetworkBehaviour
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Let the server handle physics
        if (!isServer)
        {
            rb.isKinematic = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isServer) return;

        if (collision.gameObject.CompareTag("Island"))
        {
            Island island = collision.gameObject.GetComponent<Island>();
            if (island != null)
            {
                island.TakeDamage(10f);
            }
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            // Handle player hit
        }

        // Destroy the cannonball across the network
        NetworkServer.Destroy(gameObject);
    }
}
