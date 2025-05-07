using Mirror;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(SphereCollider))]
public class BigIslandHealth : NetworkBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 500;
    [SyncVar(hook = nameof(OnHealthChanged))] public int currentHealth = 500;

    [Header("Loot Settings")]
    public float partialLootDuration = 10f;
    public float bigLootDuration = 20f;
    private bool isDestroyed = false;

    [SyncVar] private float currentLootProgress;
    private bool partialLootGiven = false;
    private bool bigLootGiven = false;

    private HashSet<GameObject> playersInside = new();
    private Coroutine lootCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
    }

    [Server]
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0 || bigLootGiven) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            bigLootGiven = true;

            Debug.Log("[SERVER] BigIsland destroyed!");

            foreach (var player in playersInside)
            {
                GiveBigLoot(player);
            }

            RpcOnDestroyed();

            GameTimer timer = FindObjectOfType<GameTimer>();
            if (timer != null)
            {
                timer.EndGameEarly("Big Island destroyed!");
            }
        }
    }

    void OnHealthChanged(int oldVal, int newVal)
    {
        if (!isClient) return;

        var player = NetworkClient.localPlayer;
        if (player != null)
        {
            var ui = player.GetComponent<PlayerIslandUI>();
            if (ui != null)
            {
                ui.ShowUI(true);
                ui.UpdateIslandHealth(newVal, maxHealth);
            }
        }
    }

    [ClientRpc]
    private void RpcOnDestroyed()
    {
        var player = NetworkClient.localPlayer;
        if (player != null)
        {
            var ui = player.GetComponent<PlayerIslandUI>();
            if (ui != null)
            {
                ui.ShowUI(true);
                ui.UpdateLootCollected(Random.Range(1000, 5000));
            }
        }
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameObject player = other.gameObject;
        playersInside.Add(player);

        if (lootCoroutine == null && playersInside.Count == 1)
        {
            lootCoroutine = StartCoroutine(CollectTreasureRoutine());
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameObject player = other.gameObject;
        playersInside.Remove(player);

        if (playersInside.Count == 0 && lootCoroutine != null)
        {
            StopCoroutine(lootCoroutine);
            lootCoroutine = null;
            currentLootProgress = 0f;
        }
    }

    [Server]
    private IEnumerator CollectTreasureRoutine()
    {
        currentLootProgress = 0f;

        while (playersInside.Count == 1)
        {
            currentLootProgress += Time.deltaTime;

            if (!partialLootGiven && currentLootProgress >= partialLootDuration && !isDestroyed)
            {
                GameObject player = playersInside.FirstOrDefault();
                if (player != null)
                {
                    GivePartialLoot(player);
                    partialLootGiven = true;
                }
            }

            yield return null;
        }

        lootCoroutine = null;
    }

    [Server]
    private void GivePartialLoot(GameObject player)
    {
        int lootAmount = Random.Range(300, 700);
        Debug.Log($"[SERVER] Partial loot: {lootAmount} to {player.name}");

        ScoreboardManager scoreboard = player.GetComponent<ScoreboardManager>();
        scoreboard?.CmdIncreaseScore(lootAmount);

        NetworkIdentity netId = player.GetComponent<NetworkIdentity>();
        if (netId != null)
            RpcNotifyLootCollected(netId.netId, lootAmount);
    }

    [Server]
    private void GiveBigLoot(GameObject player)
    {
        int lootAmount = Random.Range(1000, 5000);
        Debug.Log($"[SERVER] Big loot: {lootAmount} to {player.name}");

        ScoreboardManager scoreboard = player.GetComponent<ScoreboardManager>();
        scoreboard?.CmdIncreaseScore(lootAmount);

        NetworkIdentity netId = player.GetComponent<NetworkIdentity>();
        if (netId != null)
            RpcNotifyLootCollected(netId.netId, lootAmount);
    }

    [ClientRpc]
    private void RpcNotifyLootCollected(uint targetNetId, int amount)
    {
        var localPlayer = NetworkClient.localPlayer;
        if (localPlayer != null && localPlayer.netId == targetNetId)
        {
            var ui = localPlayer.GetComponent<PlayerIslandUI>();
            if (ui != null)
            {
                ui.ShowUI(true);
                ui.UpdateLootCollected(amount);
                Debug.Log($"[CLIENT] Loot UI updated: {amount}");
            }
        }
    }

    public float GetLootProgress()
    {
        float duration = isDestroyed ? bigLootDuration : partialLootDuration;
        return Mathf.Clamp01(currentLootProgress / duration);
    }

    public float GetCurrentHealth() => currentHealth;
}