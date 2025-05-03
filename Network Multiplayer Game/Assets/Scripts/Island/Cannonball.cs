using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkRigidbodyReliable))]
public class Cannonball : NetworkBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Only the server should apply damage
        if (!isServer) return;

        if (collision.gameObject.CompareTag("Island"))
        {
            Island island = collision.gameObject.GetComponent<Island>();
            if (island != null)
            {
                island.TakeDamage(10f);
            }
        }

        // Destroy the cannonball across the network
        NetworkServer.Destroy(gameObject);
    }
}
