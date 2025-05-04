using UnityEngine;

public class CannonFire : MonoBehaviour
{

    [SerializeField] private int index;


    private GameObject ballPrefab;
    private Rigidbody rb;


    [Header("Projectile Motion")]
    [SerializeField] private CannonData ballType;

    private float launchAngle; //will say in inspector
    private float tanAlpha, cosAlpha,sinAlpha;
    private float rangeZ;  //will say in inspector
    private float Uz, Uy, Uo;
    private float gravity;
    private float heightY;
    private float heightMax;
    private float timeTaken;

    private float lifespan;
    private float damage;

    [SerializeField] private Vector3 initialVelocity;
    [SerializeField] private Vector3 globalVelocity;

    [SerializeField] private Transform spawnPoint;

    CannonCollision cannonCtrl;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ballType = 

        InitialzeVariables();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void InitialzeVariables()
    {

        if(ballType != null)
        {
            ballPrefab = ballType.BallPrefab;
            launchAngle = ballType.LaunchAngle;
            rangeZ = ballType.RangeZ;
            heightY = ballType.HeightY;
            timeTaken = ballType.TimeTaken;
            lifespan = ballType.Lifespan;
            damage = ballType.Damage;

        }

        gravity = Physics.gravity.y;
        tanAlpha = Mathf.Tan(launchAngle * Mathf.Deg2Rad);
        cosAlpha = Mathf.Cos(launchAngle * Mathf.Deg2Rad);
        sinAlpha = Mathf.Sin(launchAngle * Mathf.Deg2Rad);


        Uz = Mathf.Sqrt((gravity*rangeZ * rangeZ)/(2*(heightY - rangeZ*tanAlpha)));
        Uy = tanAlpha * Uz;

        Uo = Mathf.Sqrt((Uz * Uz) + (Uy * Uy));

        timeTaken = -(Uo * sinAlpha * 2 / gravity);

        //Uo = -(timeTaken * gravity / sinAlpha * 2);
        //rangeZ = Uo * cosAlpha * timeTaken;

        //Uo = rangeZ / (timeTaken * cosAlpha);

        Uz = Uo * cosAlpha;
        Uy = Uo * sinAlpha;

       
        initialVelocity = new Vector3(0, Uy , Uz);
      

    }


    public void ChangeCannonType(CannonData cnData)
    {
        ballType = cnData;

        InitialzeVariables();
    }

    public void SetIndex(int idx)
    {
        index = idx;
    }


    public int GetIndex()
    {
        return index;
    }

    public void FireProjectile()
    {
        Debug.Log("FIVE STAR " + this.name);

        globalVelocity = spawnPoint.transform.TransformDirection(initialVelocity);
        GameObject cannonObj = Instantiate(ballPrefab,spawnPoint.position,spawnPoint.transform.rotation);

        cannonCtrl = cannonObj.GetComponent<CannonCollision>();

        cannonCtrl.cannonLifeSpan = lifespan;
        cannonCtrl.cannonDamage = damage;
        cannonCtrl.gravityScale = 1f;
        cannonCtrl.parentObj = this.gameObject;


        rb = cannonObj.GetComponent<Rigidbody>();
        rb.linearVelocity = globalVelocity;

      

        
        

    }

}
