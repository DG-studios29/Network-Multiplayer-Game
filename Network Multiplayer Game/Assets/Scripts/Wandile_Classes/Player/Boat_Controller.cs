using Mirror;
using UnityEngine;

public class Boat_Controller : NetworkBehaviour

{
    #region Custom Variables

    private PirateInput pirateInput;
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

    [Header("Animation Curves"), Space(5f)]
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve turnCurve;

    [Header("Transforms"), Space(5f)]
    [SerializeField] private Transform windDirIndicatorUI;
    [SerializeField] private Transform sailIndicatorUI;

    #endregion

    #region Built-In Methods

    void Start()
    {
        if (!isLocalPlayer) return;
        rb = GetComponent<Rigidbody>();
        pirateInput = GetComponent<PirateInput>();
    }

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
        if(rb == null || pirateInput == null) return;

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

        float shipDir = Vector3.Dot(shipVel, forward);

        if (shipVel.sqrMagnitude != 0 && (v > turningThreshhold || v < -turningThreshhold))
        {
            rb.AddRelativeTorque(new Vector3(0, turnCurve.Evaluate(Mathf.Abs(shipDir))
                * shipSteerMultiplier, 0) * h, ForceMode.Acceleration);
        }
    }

    private void HandleSailing()
    {
        Quaternion playerInverseEulerAngles = Quaternion.Inverse(Quaternion.Euler(0f, transform.eulerAngles.y, 0f));
        Quaternion normalizedWindDir = Quaternion.LookRotation(WindManager.instance.GetWindDirection());

        Quaternion windDirRelativeToPlayer = playerInverseEulerAngles * normalizedWindDir;

        if(sailIndicatorUI && windDirIndicatorUI)
        {
            windDirIndicatorUI.localRotation = Quaternion.Euler(0f, 0f, -windDirRelativeToPlayer.eulerAngles.y);

            sailAngle = Quaternion.Angle(sailIndicatorUI.localRotation, 
                windDirIndicatorUI.localRotation) * Mathf.Deg2Rad;

            sailsAssist += sailMultiplier * pirateInput.sailingInput;
            sailIndicatorUI.localRotation = Quaternion.Euler(0f, 0f, -sailsAssist);
        }
    }

    #endregion
}
