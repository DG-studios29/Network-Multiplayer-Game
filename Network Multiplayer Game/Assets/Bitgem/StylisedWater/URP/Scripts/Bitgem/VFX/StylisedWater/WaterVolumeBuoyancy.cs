using Mirror;
using UnityEngine;
using Bitgem.VFX.StylisedWater;

[RequireComponent(typeof(NetworkIdentity))]
public class BuoyantObject : NetworkBehaviour
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

        // Automatically find float points by tag
        GameObject[] foundPoints = GameObject.FindGameObjectsWithTag("FloatPoint");
        floatPoints = new Transform[foundPoints.Length];
        for (int i = 0; i < foundPoints.Length; i++)
        {
            floatPoints[i] = foundPoints[i].transform;
        }
    }

    void FixedUpdate()
    {
        // ONLY RUN PHYSICS ON SERVER
        if (!isServer) return;

        var helper = WaterVolumeHelper != null ? WaterVolumeHelper : WaterVolumeHelper.Instance;

        if (helper == null || helper.WaterVolume == null)
        {
            return;
        }

        if (floatPoints == null || floatPoints.Length == 0)
        {
            return;
        }

        bool isUnderwater = false;
        Vector3[] wavePoints = new Vector3[floatPoints.Length];

        for (int i = 0; i < floatPoints.Length; i++)
        {
            Transform point = floatPoints[i];
            float? height = null;

            try
            {
                height = helper.GetHeight(point.position);
            }
            catch (System.Exception)
            {
                continue;
            }

            if (height.HasValue)
            {
                float diff = (height.Value + 17f) - point.position.y;
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
                wavePoints[i] = point.position;
            }
        }

        if (isUnderwater && floatPoints.Length >= 3)
        {
            Vector3 targetNormal = CalculateSurfaceNormal(wavePoints);
            smoothedNormal = Vector3.Lerp(smoothedNormal, targetNormal, Time.fixedDeltaTime * rotationSmoothSpeed);
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