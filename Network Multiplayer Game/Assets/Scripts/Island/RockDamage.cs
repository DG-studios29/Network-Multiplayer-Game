using UnityEngine;
using Mirror;

public class RockDamage : MonoBehaviour
{
    public int damageAmount = 10;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit Rock");

            // Try to get the health component (assuming it's on the same GameObject)
            //PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            //if (playerHealth != null)
            //{
            //    // In multiplayer, damage should only be applied on the server
            //    if (NetworkServer.active)
            //    {
            //        playerHealth.TakeDamage(damageAmount);
            //    }
            //}
        }
    }
}
