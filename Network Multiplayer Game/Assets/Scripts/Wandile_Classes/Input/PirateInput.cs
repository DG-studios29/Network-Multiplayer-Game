using UnityEngine;
using UnityEngine.InputSystem;

public class PirateInput : MonoBehaviour
{
    #region Custom Variables

    private PlayerInput playerInput;

    private InputActionMap currentMap;
    private InputAction SailAction;
    private InputAction SailingAction;
    public Vector2 sailInput { get; set; }
    public float sailingInput {  get; set; }

    #endregion

    #region Built-In Methods

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        currentMap = playerInput.currentActionMap;

        playerInput.defaultControlScheme = currentMap.controlSchemes[0].name;

        SailAction = currentMap.FindAction("Sail");
        SailingAction = currentMap.FindAction("Sailing");

        SailAction.performed += OnSail;
        SailAction.canceled += NoSail;

        SailingAction.performed += OnSailing;
        SailingAction.canceled += NoSailing;
    }

    private void OnEnable()
    {
        SailAction.Enable();
        SailingAction.Enable();
    }

    private void OnDisable()
    {
        SailAction.Disable();
        SailingAction.Disable();
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
