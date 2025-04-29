using Bitgem.VFX.StylisedWater;
using UnityEngine;

//https://www.youtube.com/watch?v=eL_zHQEju8s&t=472s
//https://www.youtube.com/watch?v=fHuN7WkrmsI&t=152s
//https://www.youtube.com/watch?v=vzqoLJmpUqU&t=45s

[RequireComponent(typeof(Rigidbody))]
public class BuoyantObject : MonoBehaviour
{
    public Transform[] floatPoints;
    public float buoyancyForce = 10f;
    public float waterDrag = 1f;
    public float waterAngularDrag = 1f;
    public float rotationSmoothSpeed = 0.5f;
    public float maxTiltAngle = 25f; 

    public WaterVolumeHelper WaterVolumeHelper = null;

    private Rigidbody rb;
    private Vector3 smoothedNormal = Vector3.up;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.centerOfMass = Vector3.down * 0.5f; 
    }

    void FixedUpdate()
    {
        var helper = WaterVolumeHelper != null ? WaterVolumeHelper : WaterVolumeHelper.Instance;
        if (!helper) return;

        bool isUnderwater = false;
        Vector3[] wavePoints = new Vector3[floatPoints.Length];

        for (int i = 0; i < floatPoints.Length; i++)
        {
            Transform point = floatPoints[i];
            float? height = helper.GetHeight(point.position);

            if (height.HasValue)
            {
                float diff = (height.Value+17f) - point.position.y;
                if (diff > 0f)
                {
                    Vector3 uplift = Vector3.up * diff * buoyancyForce;
                    rb.AddForceAtPosition(uplift, point.position);
                    isUnderwater = true;
                }
                wavePoints[i] = new Vector3(point.position.x, height.Value, point.position.z);
            }
            else
            {
                wavePoints[i] = point.position; // fallback
            }
        }

        if (isUnderwater && floatPoints.Length >= 3)
        {
            Vector3 targetNormal = CalculateSurfaceNormal(wavePoints);

            // Smooth the rotation normal to prevent jittering
            smoothedNormal = Vector3.Lerp(smoothedNormal, targetNormal, Time.fixedDeltaTime * rotationSmoothSpeed);

            // Optional: Clamp tilt
            smoothedNormal = ClampTilt(smoothedNormal, maxTiltAngle);

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, smoothedNormal) * rb.rotation;
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 2f));
        }

        rb.linearDamping = isUnderwater ? waterDrag : 1f;
        rb.angularDamping = isUnderwater ? waterAngularDrag : 0.05f;
    }

    Vector3 CalculateSurfaceNormal(Vector3[] points)
    {
        if (points.Length < 3) return Vector3.up;
        Vector3 side1 = points[1] - points[0];
        Vector3 side2 = points[2] - points[0];
        return Vector3.Cross(side1, side2).normalized;
    }

    Vector3 ClampTilt(Vector3 normal, float maxDegrees)
    {
        Quaternion tiltRotation = Quaternion.FromToRotation(Vector3.up, normal);
        tiltRotation = Quaternion.RotateTowards(Quaternion.identity, tiltRotation, maxDegrees);
        return tiltRotation * Vector3.up;
    }
}
