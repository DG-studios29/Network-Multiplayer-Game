using System.Collections.Generic;
using UnityEngine;

public class BuoyancyEffect : MonoBehaviour
{
    #region Custom Variables

    public float underwaterDrag = 3f;
    public float underwaterAngularDrag = 1f;

    public float waterHeight = 0f;

    public Rigidbody rb;

    #endregion

    #region Built-In Methods

    void Start()
    {
        rb = transform.root.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        BuoyancyPhysics();
    }

    #endregion

    #region Custom Methods

    private void BuoyancyPhysics()
    {
        if (rb == null) return;

        rb.AddForceAtPosition(Physics.gravity / 4, transform.position, ForceMode.Acceleration);

        float difference = transform.position.y - waterHeight;

        if (difference < waterHeight)
        {
            float diff = Mathf.Clamp01(waterHeight - transform.position.y) * 1;
            rb.AddForceAtPosition(new Vector3(0, Mathf.Abs(Physics.gravity.y) * diff, 0)
                * -difference, transform.position, ForceMode.Acceleration);

            rb.AddForce(diff * -rb.linearVelocity * underwaterDrag
                * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rb.AddTorque(diff * -rb.angularVelocity * underwaterAngularDrag
                * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

    #endregion
}
