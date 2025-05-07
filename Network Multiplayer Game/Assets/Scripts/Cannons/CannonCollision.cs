using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class CannonCollision : NetworkBehaviour
{
    public NetworkIdentity parentNetID;

    public float cannonLifeSpan = 5f;
    public float cannonDamage = 10f;

    public float gravityScale = 1f;
    private float globalGravity = -9.81f;
    private Rigidbody rb;

    private float lifeTotal;

    private bool hasCollided = false;

    void Start()
    {
        lifeTotal = cannonLifeSpan;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Update()
    {
        //if (!isServer) return;

        //cannonLifeSpan -= Time.deltaTime;
        //if (cannonLifeSpan <= 0f)
        //{
        //    RpcHandleCannonDeath();
        //    NetworkServer.Destroy(gameObject);
        //}
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            Vector3 gravity = globalGravity * Vector3.up * gravityScale;
            rb.AddForce(gravity, ForceMode.Acceleration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       
        

        Debug.Log($"Hit: {collision.gameObject.name}");

        GameObject target = collision.gameObject;

        if (target.CompareTag("Player"))
        {
            PlayerHealthUI playerHealth = target.GetComponent<PlayerHealthUI>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)cannonDamage);
            }
        }
        else if (target.CompareTag("Island"))
        {
            Island island = target.GetComponent<Island>();
            if (island != null)
            {
                island.TakeDamage(cannonDamage);
            }
        }
        else if (target.CompareTag("KrakenAI"))
        {
            KrakenHealth kraken = target.GetComponent<KrakenHealth>();
            if (kraken != null)
            {
                kraken.TakeDamage((int)cannonDamage);
            }
        }
        else if (target.CompareTag("BigIsland"))
        {
            BigIslandHealth islandBig = target.GetComponent<BigIslandHealth>();
            if (islandBig != null)
            {
                islandBig.TakeDamage((int)cannonDamage);
            }
        }

        hasCollided = true; 

       
        NetworkServer.Destroy(gameObject);
    }


}
