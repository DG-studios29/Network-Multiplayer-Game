using UnityEngine;

public class RippleEmitter : MonoBehaviour
{
    public float rippleStrength = 1f;
    public float speedThreshold = 0.1f;

    private Vector3 lastPos;

    void Update()
    {
        float movement = (transform.position - lastPos).magnitude / Time.deltaTime;

        if (movement > speedThreshold)
        {
            Shader.SetGlobalVector("_RippleOrigin", transform.position);
            Shader.SetGlobalFloat("_RippleStrength", rippleStrength);
            Shader.SetGlobalFloat("_RippleTime", Time.time);
        }

        lastPos = transform.position;
    }

}
