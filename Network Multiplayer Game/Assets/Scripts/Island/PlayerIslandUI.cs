using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using System.Collections;

public class PlayerIslandUI : NetworkBehaviour
{
    public GameObject islandUIRoot;
    public Slider islandHealthSlider;
    public Slider lootingProgressSlider;
    public TMP_Text lootCollectedText;

    private Island targetIsland;
    private BigIslandHealth targetBigIsland;

    private ScoreboardManager scoreboardManager;

    private void Start()
    {
        if (!isLocalPlayer) return;

        ShowUI(false);

        scoreboardManager = GetComponent<ScoreboardManager>();
        if (scoreboardManager == null)
        {
            Debug.LogWarning("ScoreboardManager not found on player!");
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (targetIsland != null)
        {
            UpdateIslandHealth(targetIsland.GetCurrentHealth(), targetIsland.maxHealth);
            UpdateLootProgress(targetIsland.GetLootProgress());
        }
        else if (targetBigIsland != null)
        {
            UpdateIslandHealth(targetBigIsland.GetCurrentHealth(), targetBigIsland.maxHealth);
            UpdateLootProgress(targetBigIsland.GetLootProgress());
        }
    }

    public void SetTargetIsland(Island island)
    {
        targetIsland = island;
        targetBigIsland = null;

        if (island != null)
        {
            ShowUI(true);
            UpdateIslandHealth(island.GetCurrentHealth(), island.maxHealth);
            UpdateLootProgress(island.GetLootProgress());
        }
        else
        {
            ShowUI(false);
        }
    }

    public void SetTargetBigIsland(BigIslandHealth bigIsland)
    {
        targetBigIsland = bigIsland;
        targetIsland = null;

        if (bigIsland != null)
        {
            ShowUI(true);
            UpdateIslandHealth(bigIsland.GetCurrentHealth(), bigIsland.maxHealth);
            UpdateLootProgress(bigIsland.GetLootProgress());
        }
        else
        {
            ShowUI(false);
        }
    }

    public void ShowUI(bool show)
    {
        if (islandUIRoot != null)
            islandUIRoot.SetActive(show);
        else
            Debug.LogWarning("islandUIRoot is NOT assigned!");
    }

    public void UpdateIslandHealth(float current, float max)
    {
        if (islandHealthSlider != null)
        {
            islandHealthSlider.maxValue = max;
            islandHealthSlider.value = current;
        }
        else
        {
            Debug.LogWarning("islandHealthSlider is NOT assigned!");
        }
    }

    public void UpdateLootProgress(float progress)
    {
        if (lootingProgressSlider != null)
        {
            lootingProgressSlider.value = progress;
        }
        else
        {
            Debug.LogWarning("lootingProgressSlider is NOT assigned!");
        }
    }

    public void UpdateLootCollected(int amount)
    {
        if (lootCollectedText != null)
        {
            lootCollectedText.text = $"Loot: {amount}";
        }
    }

    public void ShowLootPopup(int amount)
    {
        UpdateLootCollected(amount);

        
        if (isLocalPlayer && NetworkClient.connection?.identity != null)
        {
            var scoreboard = NetworkClient.connection.identity.GetComponent<ScoreboardManager>();
            if (scoreboard != null)
            {
                scoreboard.CmdIncreaseScore(amount);
            }
            else
            {
                Debug.LogWarning("ScoreboardManager not found on player identity!");
            }
        }
        else
        {
            Debug.LogWarning("Not local player or missing connection identity!");
        }

        StartCoroutine(ClearLootTextAfterDelay(2f));
    }

    private IEnumerator ClearLootTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (lootCollectedText != null)
        {
            lootCollectedText.text = "Loot: 0";
        }
    }
}
