using Bitgem.VFX.StylisedWater;
using Mirror;
using UnityEngine;

//https://www.youtube.com/watch?v=eL_zHQEju8s&t=472s
//https://www.youtube.com/watch?v=fHuN7WkrmsI&t=152s
//https://www.youtube.com/watch?v=vzqoLJmpUqU&t=45s

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransformReliable))]
public class BuoyantObject : NetworkBehaviour
{
    [Header("Float Points & Forces")]
    public Transform[] floatPoints;
    public float buoyancyForce = 10f;
    public float waterDrag = 1f;
    public float waterAngularDrag = 1f;

    [Header("Rotation")]
    public float rotationSmoothSpeed = 0.5f;
    public float maxTiltAngle = 25f;

    public WaterVolumeHelper WaterVolumeHelper = null;

    private Rigidbody rb;
    private Vector3 smoothedNormal = Vector3.up;

    // Only run this on the server:
    public override void OnStartServer()
    {
        base.OnStartServer();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.centerOfMass = Vector3.down * 0.5f;
    }

    // Only the server drives the physics simulation
    [ServerCallback]
    void FixedUpdate()
    {
        var helper = WaterVolumeHelper != null ? WaterVolumeHelper : WaterVolumeHelper.Instance;
        if (!helper) return;

        bool isUnderwater = false;
        Vector3[] wavePoints = new Vector3[floatPoints.Length];

        // Apply buoyancy per floatPoint
        for (int i = 0; i < floatPoints.Length; i++)
        {
            Transform pt = floatPoints[i];
            float? height = helper.GetHeight(pt.position);

            if (height.HasValue)
            {
                float diff = (height.Value + 17f) - pt.position.y;
                if (diff > 0f)
                {
                    Vector3 uplift = Vector3.up * diff * buoyancyForce;
                    rb.AddForceAtPosition(uplift, pt.position);
                    isUnderwater = true;
                }
                wavePoints[i] = new Vector3(pt.position.x, height.Value, pt.position.z);
            }
            else
            {
                wavePoints[i] = pt.position;
            }
        }

        // Tilt to match surface normal
        if (isUnderwater && floatPoints.Length >= 3)
        {
            Vector3 targetNormal = CalculateSurfaceNormal(wavePoints);
            smoothedNormal = Vector3.Lerp(smoothedNormal, targetNormal, Time.fixedDeltaTime * rotationSmoothSpeed);
            smoothedNormal = ClampTilt(smoothedNormal, maxTiltAngle);

            Quaternion rot = Quaternion.FromToRotation(transform.up, smoothedNormal) * rb.rotation;
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, rot, Time.fixedDeltaTime * 2f));
        }

        // Water drag
        rb.linearDamping = isUnderwater ? waterDrag : 0f;
        rb.angularDamping = isUnderwater ? waterAngularDrag : 0.05f;
    }

    Vector3 CalculateSurfaceNormal(Vector3[] pts)
    {
        Vector3 side1 = pts[1] - pts[0];
        Vector3 side2 = pts[2] - pts[0];
        return Vector3.Cross(side1, side2).normalized;
    }

    Vector3 ClampTilt(Vector3 normal, float maxDeg)
    {
        Quaternion tilt = Quaternion.FromToRotation(Vector3.up, normal);
        tilt = Quaternion.RotateTowards(Quaternion.identity, tilt, maxDeg);
        return tilt * Vector3.up;
    }
}