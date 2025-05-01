using UnityEngine;
using Mirror;

[RequireComponent(typeof(Canvas))]
public class CustomNetworkHUD : MonoBehaviour
{
    private NetworkManager manager;

    private void Awake()
    {
        // Get the NetworkManager singleton
        manager = NetworkManager.singleton;

        if (manager == null)
        {
            Debug.LogError("NetworkManager not found. Ensure it exists in the scene.");
        }
    }

    private void OnGUI()
    {
        if (manager == null) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (GUILayout.Button("Host"))
            {
                manager.StartHost();
            }

            if (GUILayout.Button("Client"))
            {
                manager.StartClient();
            }

            if (GUILayout.Button("Server"))
            {
                manager.StartServer();
            }
        }
        else
        {
            if (NetworkServer.active)
            {
                GUILayout.Label("Server: Active");
            }

            if (NetworkClient.isConnected)
            {
                GUILayout.Label($"Client: Connected to {manager.networkAddress}");
            }

            if (GUILayout.Button("Stop"))
            {
                manager.StopHost();
            }
        }

        GUILayout.EndArea();
    }
}