using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class CannonCollision : NetworkBehaviour
{
    public NetworkIdentity parentNetID;

    public float cannonLifeSpan;
    public float cannonDamage;

    public float gravityScale;
    private float globalGravity;
    private Rigidbody rb;

    //public GameObject 

    float lifeTotal;
    float percentage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lifeTotal = cannonLifeSpan;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        globalGravity = -9.81f;

    }

    // Update is called once per frame
    void Update()
    {
        cannonLifeSpan -= Time.deltaTime;
        if (cannonLifeSpan <= 0)
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        Vector3 gravity = globalGravity * Vector3.up * gravityScale;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only the server should process collisions to avoid desync
        //if (!isServer) return;

        if (collision.gameObject.CompareTag("Player"))
        {

            if (cannonLifeSpan / lifeTotal >= 0.99f)
            {
                Debug.Log("Escape this");
                return;
            }

            PlayerHealthUI playerHealth = collision.gameObject.GetComponent<PlayerHealthUI>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)cannonDamage);
            }

            // Destroy the cannonball on all clients
            NetworkServer.Destroy(gameObject);
        }

        else if (collision.gameObject.CompareTag("Island"))
        {
            Debug.Log("Cannon hit an Island!");

            Island island = collision.gameObject.GetComponent<Island>();
            if (island != null)
            {
                island.TakeDamage(cannonDamage);
            }

            // Destroy the cannonball on all clients
            NetworkServer.Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("KrakenAI"))
        {
            KrakenHealth kraken = collision.gameObject.GetComponent<KrakenHealth>();
            if (kraken != null)
            {
                kraken.TakeDamage((int)cannonDamage);
            }

            NetworkServer.Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("BigIsland"))
        {
            Debug.Log("Cannon hit an Island!");

            BigIslandHealth islandBig = collision.gameObject.GetComponent<BigIslandHealth>();
            if (islandBig != null)
            {
                islandBig.TakeDamage((int)cannonDamage);
            }

            // Destroy the cannonball on all clients
            NetworkServer.Destroy(gameObject);
        }
        else
        {
            // Destroy the cannonball on all clients
            if (cannonLifeSpan / lifeTotal >= 0.99f)
            {
                Debug.Log("Escape this");
                return;
            }
            NetworkServer.Destroy(gameObject);
        }



    }


}
