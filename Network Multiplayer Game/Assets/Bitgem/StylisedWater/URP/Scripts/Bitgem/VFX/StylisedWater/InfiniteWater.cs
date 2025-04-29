using UnityEngine;

public class InfiniteWater : MonoBehaviour
{
    public Transform followTarget; // Assign this to your Camera or Player in the Inspector
   
    void LateUpdate()
    {
        if (followTarget == null) return;

        Vector3 targetPosition = followTarget.position;

       
    }
}
