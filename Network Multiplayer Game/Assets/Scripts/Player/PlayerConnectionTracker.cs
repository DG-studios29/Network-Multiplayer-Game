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
        Debug.Log("[SERVER] CmdAssignIslandUI received from client");
        TargetAssignIslandUI(islandIdentity); // will go back to correct client
    }

    [TargetRpc]
    private void TargetAssignIslandUI(NetworkIdentity islandIdentity)
    {
        Debug.Log("[CLIENT] TargetAssignIslandUI CALLED");

        Island island = islandIdentity.GetComponent<Island>();
        PlayerIslandUI ui = GetComponent<PlayerIslandUI>();

        if (ui != null)
        {
            ui.SetTargetIsland(island);
            Debug.Log("[CLIENT] UI updated with island");
        }
        else
        {
            Debug.LogWarning("[CLIENT] PlayerIslandUI not found!");
        }
    }




    [Command]
    public void CmdClearIslandUI()
    {
        TargetClearIslandUI();
    }

    [TargetRpc]
    private void TargetClearIslandUI()
    {
        Debug.Log("[CLIENT] Clearing island UI");

        PlayerIslandUI ui = GetComponent<PlayerIslandUI>();
        if (ui != null)
        {
            ui.SetTargetIsland(null);
        }
    }
}
