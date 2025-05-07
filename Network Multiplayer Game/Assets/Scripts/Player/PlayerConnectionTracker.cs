using Mirror;
using UnityEngine;

public class PlayerConnectionTracker : NetworkBehaviour
{
    public static PlayerConnectionTracker localPlayer;

    public override void OnStartLocalPlayer()
    {
        localPlayer = this;
    }

    [Command]
    public void CmdAssignIslandUI(NetworkIdentity islandIdentity)
    {
        TargetAssignIslandUI(connectionToClient, islandIdentity);
    }

    [Command]
    public void CmdAssignBigIslandUI(NetworkIdentity bigIslandIdentity)
    {
        TargetAssignBigIslandUI(connectionToClient, bigIslandIdentity);
    }

    [TargetRpc]
    private void TargetAssignIslandUI(NetworkConnection target, NetworkIdentity islandIdentity)
    {
        Island island = islandIdentity.GetComponent<Island>();
        PlayerIslandUI ui = GetComponent<PlayerIslandUI>();

        if (ui != null)
        {
            ui.SetTargetIsland(island);
        }
        else
        {
            Debug.LogWarning("PlayerIslandUI not found!");
        }
    }

    [TargetRpc]
    private void TargetAssignBigIslandUI(NetworkConnection target, NetworkIdentity bigIslandIdentity)
    {
        Debug.Log("TargetAssignBigIslandUI called");

        if (!isLocalPlayer) return;

        BigIslandHealth bigIsland = bigIslandIdentity.GetComponent<BigIslandHealth>();
        PlayerIslandUI ui = GetComponent<PlayerIslandUI>();

        if (ui != null)
        {
            ui.SetTargetBigIsland(bigIsland);
            Debug.Log($"SetTargetBigIsland with health: {bigIsland.currentHealth}/{bigIsland.maxHealth}");
        }
        else
        {
            Debug.LogWarning("PlayerIslandUI not found for BigIsland!");
        }
    }

    [Command]
    public void CmdClearIslandUI()
    {
        TargetClearIslandUI(connectionToClient);
    }

    [TargetRpc]
    private void TargetClearIslandUI(NetworkConnection target)
    {
        PlayerIslandUI ui = GetComponent<PlayerIslandUI>();
        if (ui != null)
        {
            ui.SetTargetIsland(null);
            ui.ShowUI(false);
        }
    }


  
}
