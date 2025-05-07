using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField portInputField;
    [SerializeField] private GameObject controlsPanel;

    private const ushort DefaultPort = 7777;

    private void Start()
    {
        hostButton?.onClick.AddListener(OnHostClicked);
        joinButton?.onClick.AddListener(OnJoinClicked);
        controlsButton?.onClick.AddListener(OnControlsClicked);
        backButton?.onClick.AddListener(OnBackClicked);
        exitButton?.onClick.AddListener(OnExitClicked);

        controlsPanel?.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(hostButton.gameObject);

        if (portInputField != null)
        {
            portInputField.text = DefaultPort.ToString();
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame || Gamepad.current?.buttonEast.wasPressedThisFrame == true)
        {
            if (EventSystem.current.currentSelectedGameObject != null &&
                EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                hostButton.Select();
            }
        }
    }

    private void OnHostClicked()
    {
        Debug.Log("Starting as Host");
        if (NetworkManager.singleton != null)
        {
            ApplyPort();
            NetworkManager.singleton.StartHost();
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("No NetworkManager found.");
        }
    }

    private void OnJoinClicked()
    {
        Debug.Log("Attempting to join as Client");
        if (NetworkManager.singleton != null)
        {
            string ip = ipInputField != null ? ipInputField.text : "localhost";
            ApplyPort();
            NetworkManager.singleton.networkAddress = ip;
            NetworkManager.singleton.StartClient();
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("No NetworkManager found.");
        }
    }

   private void ApplyPort()
{
    ushort port = DefaultPort;

    if (portInputField != null && ushort.TryParse(portInputField.text, out ushort parsedPort))
    {
        port = parsedPort;
    }

    if (NetworkManager.singleton.transport is TelepathyTransport telepathy)
    {
        telepathy.port = port;
        Debug.Log($"Using port: {port}");
    }
    else
    {
        Debug.LogWarning("cant set port.");
    }
}

    private void OnControlsClicked()
    {
        controlsPanel?.SetActive(true);
        if (backButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(backButton.gameObject);
        }
    }

    private void OnBackClicked()
    {
        controlsPanel?.SetActive(false);
        if (hostButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(hostButton.gameObject);
        }
    }

    private void OnExitClicked()
    {
        Debug.Log("Exiting game.");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
