using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PirateInput : NetworkBehaviour
{
    #region Custom Variables

    [SerializeField] private PlayerHealthUI playerHealthUI;  
    private PlayerInput playerInput; 

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
        
        playerInput = GetComponent<PlayerInput>();  

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component is missing from this GameObject.");
            return;
        }

    
        currentMap = playerInput.currentActionMap;

        SailAction = currentMap.FindAction("Sail");
        SailingAction = currentMap.FindAction("Sailing");
        CamAction = currentMap.FindAction("Look");
    }

    private void OnEnable()
    {
        if (!isLocalPlayer) return;

        
        SailAction.Enable();
        SailingAction.Enable();
        CamAction.Enable();
    }

    private void OnDisable()
    {
        if (!isLocalPlayer) return;

      
        SailAction.Disable();
        SailingAction.Disable();
        CamAction.Disable();
    }

    private void Update()
    {
        if (!isLocalPlayer || playerHealthUI.currentHealth <= 0) return;  

        sailInput = SailAction.ReadValue<Vector2>();
        sailingInput = SailingAction.ReadValue<float>();

      
        if (playerHealthUI.currentHealth <= 0)
        {
            sailInput = Vector2.zero;
            sailingInput = 0f;
        }
    }
    #endregion
}
