using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerConnectionTracker))]
public class IslandTriggerWatcher : NetworkBehaviour
{
    private PlayerConnectionTracker tracker;

    private void Awake()
    {
        tracker = GetComponent<PlayerConnectionTracker>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer || tracker == null) return;

        if (other.CompareTag("Island"))
        {
            Island island = other.GetComponentInParent<Island>();
            if (island != null)
            {
                tracker.CmdAssignIslandUI(island.netIdentity);
            }
        }
        else if (other.CompareTag("BigIsland"))
        {
            Debug.Log("[DEBUG] BigIsland trigger logic hit!");

            BigIslandHealth bigIsland = other.GetComponentInParent<BigIslandHealth>();
            if (bigIsland != null)
            {
                Debug.Log("[DEBUG] BigIslandHealth component found, assigning UI.");
                tracker.CmdAssignBigIslandUI(bigIsland.netIdentity);
            }
            else
            {
                Debug.LogWarning("[DEBUG] BigIslandHealth component NOT found!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isLocalPlayer || tracker == null) return;

        if (other.CompareTag("Island") || other.CompareTag("BigIsland"))
        {
            tracker.CmdClearIslandUI();
        }
    }
}
