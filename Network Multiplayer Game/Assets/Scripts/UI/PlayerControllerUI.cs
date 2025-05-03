using UnityEngine;
using Mirror;

public class PlayerControllerUI : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; 
    public float rotationSpeed = 100f; 

    [Header("Camera Settings")]
    public Camera playerCamera; 
    public float mouseSensitivity = 100f; 
    private float cameraPitch = 0f; 

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        
        if (Camera.main != null)
        {
            playerCamera = Camera.main;
            playerCamera.transform.SetParent(transform);
            playerCamera.transform.localPosition = new Vector3(0, 2, -4); 
            playerCamera.transform.localRotation = Quaternion.Euler(10, 0, 0); 
        }

        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        HandleMovement();
        HandleCameraRotation();
    }

    private void HandleMovement()
    {
        
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

      
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

       
        rb.MovePosition(rb.position + move * moveSpeed * Time.deltaTime);
    }

    private void HandleCameraRotation()
    {
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

       
        transform.Rotate(Vector3.up * mouseX);

        
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -45f, 45f); 
        playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
}