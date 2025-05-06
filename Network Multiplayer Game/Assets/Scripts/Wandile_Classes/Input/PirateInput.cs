using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PirateInput : NetworkBehaviour
{
    #region Custom Variables

    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private PlayerHealthUI playerHealthUI;


    private InputActionMap currentMap;
    private InputAction SailAction;
    private InputAction SailingAction;
    public InputAction CamAction { get; set; }
    public Vector2 sailInput { get; set; }
    public Vector2 camInput { get; set; }
    public float sailingInput {  get; set; }

    #endregion

    #region Built-In Methods

    private void Awake()
    {
        if (!isLocalPlayer) return;
        playerInput = GetComponent<PlayerInput>();
        currentMap = playerInput.currentActionMap;

        //playerInput.defaultControlScheme = currentMap.controlSchemes[0].name;

        SailAction = currentMap.FindAction("Sail");
        SailingAction = currentMap.FindAction("Sailing");
        CamAction = currentMap.FindAction("Look");

        SailAction.performed += OnSail;
        SailAction.canceled += NoSail;

        SailingAction.performed += OnSailing;
        SailingAction.canceled += NoSailing;

        CamAction.performed += OnCamera;
        CamAction.canceled += NoCamera;

        currentMap.Enable();
    }

    private void OnEnable()
    {
        if (isLocalPlayer && currentMap != null)
        currentMap.Enable();
    }

    private void OnDisable()
    {
        if (isLocalPlayer && currentMap != null)
            currentMap.Disable();
    }

    private void Update()
    {
        if (!isLocalPlayer || SailAction == null|| SailingAction == null ||playerHealthUI == null) return;

        sailInput = SailAction.ReadValue<Vector2>();
        sailingInput = SailingAction.ReadValue<float>();


        if (playerHealthUI.currentHealth <= 0)
        {
            sailInput = Vector2.zero;
            sailingInput = 0f;
        }
    }

    #endregion

    #region Custom Variables

    public void OnSail(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        sailInput = context.ReadValue<Vector2>();
    }

    public void NoSail(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        sailInput = Vector2.zero;
    }

    public void OnCamera(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        camInput = context.ReadValue<Vector2>();
    }

    public void NoCamera(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        camInput = Vector2.zero;
    }

    public void OnSailing(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        sailingInput = context.ReadValue<float>();
    }

    public void NoSailing(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        sailingInput = 0f;
    }

    #endregion
}
