using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Health")]
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int health = 100;
    public Slider healthBar;

    [Header("Minimap")]
    public GameObject minimapIconPrefab;
    private GameObject minimapIcon;

    [Header("Camera")]
    public Camera playerCamera;
    public float mouseSensitivity = 100f;
    private float cameraPitch = 0f;

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // Initialize minimap icon
        if (minimapIconPrefab != null)
        {
            minimapIcon = Instantiate(minimapIconPrefab);
            minimapIcon.transform.SetParent(GameObject.Find("MinimapCanvas").transform, false);
        }

        // Initialize health bar
        if (healthBar != null)
        {
            Debug.Log("Health bar assigned and initialized.");
            healthBar.gameObject.SetActive(true);
            healthBar.maxValue = 100; // Set max health
            healthBar.value = health; // Initialize slider value
        }
        else
        {
            Debug.LogError("Health bar is not assigned!");
        }

        // Assign the local player's camera
        if (Camera.main != null)
        {
            playerCamera = Camera.main;
            playerCamera.transform.SetParent(transform);
            playerCamera.transform.localPosition = new Vector3(0, 2, -4); // Adjust as needed
            playerCamera.transform.localRotation = Quaternion.Euler(10, 0, 0); // Adjust as needed
        }

        // Lock the cursor for mouse rotation
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        HandleMovement();
        HandleCameraRotation();

        // Update minimap icon position and rotation
        if (minimapIcon != null)
        {
            minimapIcon.transform.position = new Vector3(transform.position.x, transform.position.z, 0);

            // Rotate the minimap icon to match the camera's rotation
            if (playerCamera != null)
            {
                minimapIcon.transform.rotation = Quaternion.Euler(0, 0, -playerCamera.transform.eulerAngles.y);
            }
        }

        // Test damage input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdTakeDamage(10);
        }
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move * moveSpeed * Time.deltaTime);
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -45f, 45f); // Limit vertical rotation
        playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    [Command]
    public void CmdTakeDamage(int damage)
    {
        if (!isServer) return;

        health -= damage;

        if (health <= 0)
        {
            health = 0;
            Debug.Log($"{gameObject.name} has died!");
        }
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        Debug.Log($"Health changed from {oldHealth} to {newHealth}");
        if (isLocalPlayer && healthBar != null)
        {
            healthBar.value = newHealth;
            Debug.Log($"Health bar updated to: {healthBar.value}");
        }
    }

    private void OnDestroy()
    {
        if (minimapIcon != null)
        {
            Destroy(minimapIcon);
        }
    }
}