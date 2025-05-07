using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class SharkDistraction : NetworkBehaviour
{
    [Header("Distraction Settings")]
    public float distractionRadius = 25f;
    public float distractionDuration = 5f;
    public float cooldownTime = 10f;

    [Header("Bait Settings")]
    public GameObject baitPrefab;
    public Transform baitSpawnPoint;

    [Header("Cooldown UI")]
    public Image cooldownOverlay;

    private bool canUseBait = true;
    private float cooldownTimer = 0f;

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
        if (dropBaitAction != null)
            dropBaitAction.performed += OnDropBait;
    }

    private void OnDisable()
    {
        if (isOwned && dropBaitAction != null)
            dropBaitAction.performed -= OnDropBait;
    }

    private void Update()
    {
        if (!isOwned || cooldownOverlay == null) return;

        if (!canUseBait)
        {
            if (!cooldownOverlay.gameObject.activeSelf)
                cooldownOverlay.gameObject.SetActive(true);

            cooldownTimer -= Time.deltaTime;
            cooldownOverlay.fillAmount = cooldownTimer / cooldownTime;

            if (cooldownTimer <= 0f)
            {
                canUseBait = true;
                cooldownOverlay.fillAmount = 0f;
                cooldownOverlay.gameObject.SetActive(false);
            }
        }
    }

    private void OnDropBait(InputAction.CallbackContext ctx)
    {
        if (!canUseBait) return;

        canUseBait = false;
        cooldownTimer = cooldownTime;
        cooldownOverlay.fillAmount = 1f;

        Vector3 spawnPos = baitSpawnPoint != null ? baitSpawnPoint.position : transform.position;
        CmdDropBait(spawnPos);
    }

    [Command]
    private void CmdDropBait(Vector3 baitPosition)
    {
        if (baitPrefab != null)
        {
            GameObject bait = Instantiate(baitPrefab, baitPosition, Quaternion.identity);
            NetworkServer.Spawn(bait);
            Destroy(bait, distractionDuration);
        }

        RpcDistractSharks(baitPosition);
    }

    [ClientRpc]
    private void RpcDistractSharks(Vector3 center)
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

    private void OnDrawGizmosSelected()
    {

        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.4f); // light blue
        Vector3 center = baitSpawnPoint != null ? baitSpawnPoint.position : transform.position;
        Gizmos.DrawWireSphere(center, distractionRadius);

    }
}
