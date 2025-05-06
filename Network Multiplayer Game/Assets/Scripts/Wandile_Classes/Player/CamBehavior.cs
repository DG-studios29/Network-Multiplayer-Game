using UnityEngine;

public class CamBehavior : MonoBehaviour
{
    #region Custom Vars

    [Header("Input"), Space(5f)]
    public PirateInput input;

    [Header("Transform/s"), Space(5f)]
    [SerializeField] private Transform localPivotPoint;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform waterEdgeChecker;

    [Header("Float/s"), Space(5f)]
    [SerializeField, Range(10f, 100f)] private float sensitivity;

    [SerializeField, Range(15f, 40f)] private float minRollLimit, maxRollLimit;
    [SerializeField, Range(0f, 50f)] private float checkRadius;

    [Header("LayerMask/s"), Space(5f)]
    public LayerMask edgeMask;

    private float xRot, yRot;
    public float normalMin = -15f;
    public float normalMax = 20f;

    private float edgeLimit = 40f;

    private bool isOnEdge = false;


    #endregion

    #region Built-In Methods
    void Start()
    {
        if (cam != null)
            cam.localRotation = Quaternion.Euler(15, 0, 0);
        cam.localPosition = new Vector3(0, 20, -54);
    }

    void LateUpdate()
    {
        if (localPivotPoint != null) OrbitOnCommand();

        SwitchToTopViewIfNecessary();
    }

    #endregion

    #region Custom Methods

    private void OrbitOnCommand()
    {
        localPivotPoint.position = localPivotPoint.parent ? localPivotPoint.localPosition = transform.position : localPivotPoint.position = transform.position;

        float x = input.camInput.x * Time.smoothDeltaTime * sensitivity;
        float y = input.camInput.y * Time.smoothDeltaTime * sensitivity;

        xRot -= y;
        yRot += x;

        xRot = Mathf.Clamp(xRot, minRollLimit, maxRollLimit);

        if (yRot > 180f)
        {
            yRot -= 360f;
        }

        localPivotPoint.rotation = Quaternion.Euler(xRot + transform.eulerAngles.x, yRot + transform.eulerAngles.y, 0);
    }

    private void SwitchToTopViewIfNecessary()
    {
        if (waterEdgeChecker == null) return;

        //THIS ONLY WORKS AGAINST CONVEX COLLIDERS 
        Collider[] colliders = Physics.OverlapSphere(waterEdgeChecker.position, checkRadius, edgeMask);

        isOnEdge = colliders.Length > 0 ? true : false;

        switch (isOnEdge)
        {
            case true:
                minRollLimit = Mathf.Lerp(minRollLimit, edgeLimit, Time.deltaTime * 5f);
                maxRollLimit = Mathf.Lerp(maxRollLimit, edgeLimit, Time.deltaTime * 5f);
                break;

            case false:
                minRollLimit = Mathf.Lerp(minRollLimit, normalMin, Time.deltaTime * 5f);
                maxRollLimit = Mathf.Lerp(maxRollLimit, normalMax, Time.deltaTime * 5f);
                break;

        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (waterEdgeChecker == null) return;
        Gizmos.DrawWireSphere(waterEdgeChecker.position, checkRadius);
    }

    #endregion
}
