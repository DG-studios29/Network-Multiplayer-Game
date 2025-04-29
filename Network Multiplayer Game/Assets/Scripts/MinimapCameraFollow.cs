using UnityEngine;
using UnityEngine.UI;  

public class MinimapCameraFollow : MonoBehaviour
{
    public Transform target;  
    public Vector3 offset = new Vector3(0, 100f, 0); 

    public RawImage minimapUI;  

    void LateUpdate()
    {
        if (target)
        {
            transform.position = target.position + offset;

           
            transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);

            if (minimapUI != null)
            {
             
                minimapUI.rectTransform.rotation = Quaternion.Euler(0f, 0f, target.eulerAngles.y);
            }
        }
    }
}
