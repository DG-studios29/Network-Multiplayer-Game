using UnityEngine;
using UnityEngine.InputSystem;

public class PirateInput : MonoBehaviour
{
    #region Custom Variables

    private PlayerInput playerInput;

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
    }

    private void OnEnable()
    {
        currentMap.Enable();
    }

    private void OnDisable()
    {
        currentMap.Disable();
    }

    #endregion

    #region Custom Variables

    public void OnSail(InputAction.CallbackContext context)
    {
        sailInput = context.ReadValue<Vector2>();
    }

    public void NoSail(InputAction.CallbackContext context)
    {
        sailInput = Vector2.zero;
    }

    public void OnCamera(InputAction.CallbackContext context)
    {
        camInput = context.ReadValue<Vector2>();
    }

    public void NoCamera(InputAction.CallbackContext context)
    {
        camInput = Vector2.zero;
    }

    public void OnSailing(InputAction.CallbackContext context)
    {
        sailingInput = context.ReadValue<float>();
    }

    public void NoSailing(InputAction.CallbackContext context)
    {
        sailingInput = 0f;
    }

    #endregion
}
