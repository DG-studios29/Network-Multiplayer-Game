using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class KrakenDistraction : NetworkBehaviour
{
    [Header("Distraction Settings")]
    public float smokeRadius = 30f;
    public float smokeDuration = 6f;
    public float cooldownTime = 12f;

    [Header("Smoke Bomb")]
    public GameObject smokeBombPrefab;
    public Transform smokeSpawnPoint; 

    [Header("Cooldown UI")]
    public Image cooldownOverlay;

    private bool canUseSmoke = true;
    private float cooldownTimer = 0f;

    private PlayerInput playerInput;
    private InputAction throwSmokeAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        throwSmokeAction = playerInput.actions["ThrowSmoke"];
        if (throwSmokeAction != null)
            throwSmokeAction.performed += OnThrowSmoke;
    }

    private void OnDisable()
    {
        if (isOwned && throwSmokeAction != null)
            throwSmokeAction.performed -= OnThrowSmoke;
    }

    private void Update()
    {
        if (!isOwned || cooldownOverlay == null) return;

        if (!canUseSmoke)
        {
            if (!cooldownOverlay.gameObject.activeSelf)
                cooldownOverlay.gameObject.SetActive(true);

            cooldownTimer -= Time.deltaTime;
            cooldownOverlay.fillAmount = cooldownTimer / cooldownTime;

            if (cooldownTimer <= 0f)
            {
                canUseSmoke = true;
                cooldownOverlay.fillAmount = 0f;
                cooldownOverlay.gameObject.SetActive(false);
            }
        }
    }

    private void OnThrowSmoke(InputAction.CallbackContext ctx)
    {
        if (!canUseSmoke) return;

        canUseSmoke = false;
        cooldownTimer = cooldownTime;
        cooldownOverlay.fillAmount = 1f;

        Vector3 spawnPos = smokeSpawnPoint != null ? smokeSpawnPoint.position : transform.position;
        CmdThrowSmoke(spawnPos);
    }

    [Command]
    private void CmdThrowSmoke(Vector3 position)
    {
        if (smokeBombPrefab != null)
        {
            GameObject smoke = Instantiate(smokeBombPrefab, position, Quaternion.identity);
            NetworkServer.Spawn(smoke);
            Destroy(smoke, smokeDuration);
        }

        RpcDistractKrakens(position);
    }

    [ClientRpc]
    private void RpcDistractKrakens(Vector3 center)
    {
        Collider[] colliders = Physics.OverlapSphere(center, smokeRadius);
        foreach (var col in colliders)
        {
            KrakenAI kraken = col.GetComponent<KrakenAI>();
            if (kraken != null)
            {
                kraken.Distract(smokeDuration);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.8f, 0.2f, 1f, 0.4f); // purple smoke radius
        Vector3 center = smokeSpawnPoint != null ? smokeSpawnPoint.position : transform.position;
        Gizmos.DrawWireSphere(center, smokeRadius);
    }
}
