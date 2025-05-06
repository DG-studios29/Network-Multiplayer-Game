using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private GameObject controlsPanel;

    [Header("Controller Navigation")]
    [SerializeField] private GameObject firstSelected; 

    private void Start()
    {
        hostButton?.onClick.AddListener(OnHostClicked);
        joinButton?.onClick.AddListener(OnJoinClicked);
        controlsButton?.onClick.AddListener(OnControlsClicked);
        backButton?.onClick.AddListener(OnBackClicked);
        exitButton?.onClick.AddListener(OnExitClicked);

        controlsPanel?.SetActive(false);

        
        EventSystem.current.SetSelectedGameObject(null); 
        EventSystem.current.SetSelectedGameObject(firstSelected); 
    }

    private void OnHostClicked()
    {
        Debug.Log("Starting as Host");
        if (NetworkManager.singleton != null)
        {
            NetworkManager.singleton.StartHost();
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
            NetworkManager.singleton.networkAddress = ip;
            NetworkManager.singleton.StartClient();
        }
        else
        {
            Debug.LogError("No NetworkManager found.");
        }
    }

    private void OnControlsClicked()
    {
        controlsPanel?.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(backButton.gameObject); 
    }

    private void OnBackClicked()
    {
        controlsPanel?.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected); 
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
