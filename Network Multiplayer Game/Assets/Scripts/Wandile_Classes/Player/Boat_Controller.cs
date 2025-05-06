using Mirror;
using UnityEngine;

public class Boat_Controller : NetworkBehaviour
{
    #region Custom Variables

    [SerializeField] private PirateInput pirateInput;
    private Rigidbody rb;

    [Header("Floats/ numbers"), Space(5f)]
    [SerializeField] private float shipSpeed = 5f;
    [SerializeField] private float shipSteerMultiplier = 3f;

    [SyncVar]private float speed;
    [SyncVar]private float sailsAssist = 0f;
    [SyncVar]private float sailAngle = 0f;
    private const float acceptableAngle = .7f;

    [Header("Sliders && numbers"), Space(5f)]
    [Range(0f, .5f)] public float turningThreshhold;
    [Range(0f, 10f)] public float sailMultiplier;
    [Range(0f, 100f)] public float nonPhysicsSteerMltiplier = 60;

    [Header("Animation Curves"), Space(5f)]
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve turnCurve;

    [Header("Transforms"), Space(5f)]
    [SerializeField] private Transform windDirIndicatorUI;
    [SerializeField] private Transform sailIndicatorUI;


    [Header("Audio"), Space(5f)]
    [SerializeField] private AudioSource waterSFx, sailSFx;

    [Header("All About Speed Gauge"), Space(5f)]
    [SerializeField] private Transform speedNeedleUI;
    [SerializeField] private float minNeedleRot, maxNeedleRot;
    private float speedInKm;
    private float nonPhysicsSteerFactor;

    #endregion

    #region Built-In Methods
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null) { Debug.Log("No RD"); }
    }
    void OnEnable()
    {
        if (!isLocalPlayer) return;

        //pirateInput = GetComponent<PirateInput>();
        Debug.Log("Local is Connected");
        if (waterSFx != null)
            waterSFx.volume = .01f;
    }

    [ClientCallback]
    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        ShipSail();
        HandleSailing();
        
    }

    #endregion

    #region Custom Methods

    private void ShipSail()
    {
        if (rb == null || pirateInput == null || waterSFx == null) return;

        float v = pirateInput.sailInput.y;
        float h = pirateInput.sailInput.x;


        //speed Evaluation
        switch (Mathf.Abs(sailAngle))
        {
            case < acceptableAngle:
                speed = speedCurve.Evaluate(Mathf.Abs(sailAngle)) + shipSpeed;
                break;

            case >= acceptableAngle:
                speed = shipSpeed;
                break;

            default:
                speed = shipSpeed;
                break;
        }

        var shipVel = rb.linearVelocity.normalized;
        var forward = rb.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        rb.AddForce(forward * v * speed, ForceMode.Acceleration);

        //seed gauge
        speedInKm = Mathf.Abs(Mathf.Ceil(rb.linearVelocity.magnitude));

        speedNeedleUI.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(minNeedleRot, maxNeedleRot, speedInKm / 64));

        float sqrSpeed = Mathf.Max(rb.linearVelocity.sqrMagnitude, 0.0001f);
        float clampedSqrSpeed = Mathf.Clamp01(sqrSpeed);
        waterSFx.volume = Mathf.Lerp(waterSFx.volume, clampedSqrSpeed * .1f, Time.fixedDeltaTime);

        float shipDir = Vector3.Dot(shipVel, forward);

        if (shipVel.sqrMagnitude != 0 && v> 0.2f || v< -0.2f)
        {
            rb.AddRelativeTorque(new Vector3(0, turnCurve.Evaluate(Mathf.Abs(shipDir))
                * shipSteerMultiplier, 0) * h, ForceMode.Acceleration);
        }

        //Non-Physics Steer
        if(shipVel.sqrMagnitude<= Mathf.Abs(1) && v < 0.1f || v > -0.1f)
        {
            nonPhysicsSteerFactor += Time.fixedDeltaTime * h * nonPhysicsSteerMltiplier;
            transform.rotation = Quaternion.Euler(0f, nonPhysicsSteerFactor, 0f);
        }
    }

    private void HandleSailing()
    {
        Quaternion playerInverseEulerAngles = Quaternion.Inverse(Quaternion.Euler(0f, transform.eulerAngles.y, 0f));
        Quaternion normalizedWindDir = Quaternion.LookRotation(WindManager.instance.GetWindDirection());

        Quaternion windDirRelativeToPlayer = playerInverseEulerAngles * normalizedWindDir;

        if (sailIndicatorUI && windDirIndicatorUI)
        {
            windDirIndicatorUI.localRotation = Quaternion.Euler(0f, 0f, -windDirRelativeToPlayer.eulerAngles.y);

            sailAngle = Quaternion.Angle(sailIndicatorUI.localRotation,
                windDirIndicatorUI.localRotation) * Mathf.Deg2Rad;

            sailsAssist += sailMultiplier * pirateInput.sailingInput;
            sailIndicatorUI.localRotation = Quaternion.Euler(0f, 0f, -sailsAssist);

            if(sailSFx!= null)
            sailSFx.volume = pirateInput.sailingInput > 0.5f || pirateInput.sailingInput < -0.5f ? .06f : 0f;
        }
    }

    #endregion
}