using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using System.Collections;

public class SharkDistraction : NetworkBehaviour
{
    public float distractionRadius = 25f;
    public float distractionDuration = 5f;
    public float cooldownTime = 10f;
    private bool canUseBait = true;

    public GameObject baitPrefab;

    private PlayerInput playerInput;
    private InputAction dropBaitAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        dropBaitAction = playerInput.actions["DropBait"];
        dropBaitAction.performed += OnDropBait;
    }

    private void OnDisable()
    {
        if (isOwned && dropBaitAction != null)
            dropBaitAction.performed -= OnDropBait;
    }

    private void OnDropBait(InputAction.CallbackContext ctx)
    {
        if (!canUseBait) return;
        canUseBait = false;
        StartCoroutine(BaitCooldownRoutine());
        CmdDropBait(transform.position);
    }

    IEnumerator BaitCooldownRoutine()
    {
        yield return new WaitForSeconds(cooldownTime);
        canUseBait = true;
    }

    [Command]
    void CmdDropBait(Vector3 baitPosition)
    {
        RpcDistractSharks(baitPosition);

        if (baitPrefab != null)
        {
            GameObject bait = Instantiate(baitPrefab, baitPosition, Quaternion.identity);
            NetworkServer.Spawn(bait);
            Destroy(bait, distractionDuration);
        }
    }

    [ClientRpc]
    void RpcDistractSharks(Vector3 center)
    {
        Collider[] colliders = Physics.OverlapSphere(center, distractionRadius);
        foreach (var col in colliders)
        {
            SharkAI shark = col.GetComponent<SharkAI>();
            if (shark != null)
            {
                shark.Distract(distractionDuration);
            }
        }
    }
}