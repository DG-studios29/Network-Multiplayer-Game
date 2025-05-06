using UnityEngine;
using Mirror;

public class IslandTriggerWatcher : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer) return;

        if (other.CompareTag("Island"))
        {
            Island island = other.GetComponentInParent<Island>();
            if (island != null)
            {
                Debug.Log("[CLIENT] Entered island trigger, calling CmdAssignIslandUI");
                GetComponent<PlayerConnectionTracker>()?.CmdAssignIslandUI(island.netIdentity);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isLocalPlayer) return;

        if (other.CompareTag("Island"))
        {
            Debug.Log("[CLIENT] Exited island trigger, calling CmdClearIslandUI");
            GetComponent<PlayerConnectionTracker>()?.CmdClearIslandUI();
        }
    }
}
