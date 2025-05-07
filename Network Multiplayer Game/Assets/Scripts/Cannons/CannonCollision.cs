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

    public GameObject impactFX;
    public GameObject smallImpactFX;

    public Transform parentPos;

    private float lifeTotal;

    void Start()
    {
        lifeTotal = cannonLifeSpan;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Update()
    {
        if (!isServer) return;

        cannonLifeSpan -= Time.deltaTime;
        if (cannonLifeSpan <= 0f)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            Vector3 gravity = globalGravity * Vector3.up * gravityScale;
            rb.AddForce(gravity, ForceMode.Acceleration);
        }
    }

    //Place Command and Client Rcp
  
    private void OnCollisionEnter(Collision collision)
    {
        //if (!isServer) return;

   /*     float percentage = cannonLifeSpan / lifeTotal;

        if (percentage >= 0.99f)
        {
            Debug.Log("Skipping early collision.");
            return;
        }*/



        GameObject target = collision.gameObject;


        if (target.CompareTag("Player"))
        {
            NetworkIdentity targetNetID = target.gameObject.GetComponent<NetworkIdentity>();

            //Another method
            if (targetNetID.netId.ToString() == parentNetID.netId.ToString())
            {
                Debug.Log("Avoided self hit");
                return;
               
            }
           
            PlayerHealthUI playerHealth = target.GetComponent<PlayerHealthUI>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)cannonDamage);

                //Impact 
                GameObject hit = Instantiate(impactFX,this.transform.position,Quaternion.identity);
                Destroy(hit, 2f);
                NetworkServer.Destroy(gameObject);
            }
        }

        else if (target.CompareTag("Island"))
        {
            Island island = target.GetComponent<Island>();
            if (island != null)
            {
                island.TakeDamage(cannonDamage);

                //Impact 
                GameObject hit = Instantiate(impactFX, this.transform.position, Quaternion.identity);
                Destroy(hit, 2f);
                NetworkServer.Destroy(gameObject);
            }
        }

        else if (target.CompareTag("KrakenAI"))
        {
            KrakenHealth kraken = target.GetComponent<KrakenHealth>();
            if (kraken != null)
            {
                kraken.TakeDamage((int)cannonDamage);

                //Impact 
                GameObject hit = Instantiate(impactFX, this.transform.position, Quaternion.identity);
                Destroy(hit, 2f);
                NetworkServer.Destroy(gameObject);
            }
        }

        else if (target.CompareTag("BigIsland"))
        {
            BigIslandHealth islandBig = target.GetComponent<BigIslandHealth>();
            if (islandBig != null)
            {
                islandBig.TakeDamage((int)cannonDamage);

                //Impact 
                GameObject hit = Instantiate(impactFX, transform.position, Quaternion.identity);
                Destroy(hit, 2f);
                NetworkServer.Destroy(gameObject);
            }
        }

        else if (target.CompareTag("Rocks"))
        {
            //just for whatever we may collide with without tags

            GameObject hit = Instantiate(impactFX, transform.position, Quaternion.identity);
            Destroy(hit, 2f);
            NetworkServer.Destroy(gameObject);
        }

      

        GameObject noSpecific = Instantiate(smallImpactFX, transform.position, Quaternion.identity);
        Destroy(noSpecific, 2f);
        
        NetworkServer.Destroy(gameObject);
    }
}


