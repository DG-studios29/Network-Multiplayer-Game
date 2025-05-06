using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class PlayerIslandUI : NetworkBehaviour
{
    public GameObject islandUIRoot;
    public Slider islandHealthSlider;
    public Slider lootingProgressSlider;
    public TMP_Text lootCollectedText;

    private Island targetIsland;

    private void Start()
    {
        if (!isLocalPlayer) return;
        ShowUI(false); // hide at start
    }

    private void Update()
    {
        if (!isLocalPlayer || targetIsland == null) return;

        // Update health
        float health = targetIsland.GetCurrentHealth();
        float max = targetIsland.maxHealth;
        UpdateIslandHealth(health, max);

        // Update loot progress
        float progress = targetIsland.GetLootProgress();
        UpdateLootProgress(progress);

        UpdateIslandHealth(targetIsland.GetCurrentHealth(), targetIsland.maxHealth);
        UpdateLootProgress(targetIsland.GetLootProgress());
    }


    public void SetTargetIsland(Island island)
    {
        targetIsland = island;

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


    public void ShowUI(bool show)
    {
        if (islandUIRoot != null)
            islandUIRoot.SetActive(show);
    }

    public void UpdateIslandHealth(float current, float max)
    {
        if (islandHealthSlider != null)
        {
            islandHealthSlider.maxValue = max;
            islandHealthSlider.value = current;
        }
    }


    public void UpdateLootProgress(float progress)
    {
        if (lootingProgressSlider != null)
        {
            lootingProgressSlider.value = progress;
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
        Debug.Log($"[CLIENT] Loot popup: {amount}");
    }

}
