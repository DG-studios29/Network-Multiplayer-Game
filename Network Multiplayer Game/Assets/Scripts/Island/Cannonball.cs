using UnityEngine;

public class Cannonball : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Reduce player health here
        }

        if (collision.gameObject.CompareTag("Island"))
        {
            Island island = collision.gameObject.GetComponent<Island>();
            if (island != null)
            {
                island.TakeDamage(10f);
            }
        }

        Destroy(gameObject);
    }
}
