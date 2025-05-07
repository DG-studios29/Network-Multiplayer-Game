using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PirateInput : NetworkBehaviour
{
    #region Custom Variables

    [SerializeField] private PlayerHealthUI playerHealthUI;
    [SerializeField] private PlayerInput playerInput;

    private InputActionMap currentMap;
    private InputAction SailAction;
    private InputAction SailingAction;
    public InputAction CamAction { get; set; }
    public Vector2 sailInput { get; set; }
    public Vector2 camInput { get; set; }
    public float sailingInput { get; set; }

    #endregion

    #region Built-In Methods

    private void Awake()
    {
        currentMap = playerInput.currentActionMap;

        SailAction = currentMap.FindAction("Sail");
        SailingAction = currentMap.FindAction("Sailing");
        CamAction = currentMap.FindAction("Look");

        SailAction.performed += OnSail;
        SailAction.canceled += NoSail;

        SailingAction.performed += OnSailing;
        SailingAction.canceled += NoSailing;

        CamAction.performed += OnCamera;
        CamAction.canceled += NoCamera;
    }

    private void OnEnable()
    {
        currentMap.Enable();
    }

    private void OnDisable()
    {
        currentMap.Disable();
    }

    private void Update()
    {
        if (!isLocalPlayer || playerHealthUI == null) return;

        // If health is 0 or less, forcibly stop all input
        if (playerHealthUI.currentHealth <= 0)
        {
            sailInput = Vector2.zero;
            sailingInput = 0f;
            camInput = Vector2.zero;
        }
    }

    #endregion

    #region Custom Input Callbacks

    public void OnSail(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer || playerHealthUI == null || playerHealthUI.currentHealth <= 0) return;
        sailInput = context.ReadValue<Vector2>();
    }

    public void NoSail(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        sailInput = Vector2.zero;
    }

    public void OnCamera(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer || playerHealthUI == null || playerHealthUI.currentHealth <= 0) return;
        camInput = context.ReadValue<Vector2>();
    }

    public void NoCamera(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        camInput = Vector2.zero;
    }

    public void OnSailing(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer || playerHealthUI == null || playerHealthUI.currentHealth <= 0) return;
        sailingInput = context.ReadValue<float>();
    }

    public void NoSailing(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        sailingInput = 0f;
    }

    #endregion
}
