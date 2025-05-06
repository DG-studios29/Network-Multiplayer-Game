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
    private BigIslandHealth targetBigIsland;

    private void Start()
    {
        if (!isLocalPlayer) return;
        ShowUI(false);
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

    // Set Island target (standard island)
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

    // Set BigIsland target (boss island)
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
        Debug.Log($"[UI] ShowUI({show}) called");
        if (islandUIRoot != null)
            islandUIRoot.SetActive(show);
        else
            Debug.LogWarning("[UI] islandUIRoot is NOT assigned!");
    }

    public void UpdateIslandHealth(float current, float max)
    {
        Debug.Log($"[UI] Slider updated to: {current}/{max}");

        if (islandHealthSlider != null)
        {
            islandHealthSlider.maxValue = max;
            islandHealthSlider.value = current;
        }
        else
        {
            Debug.LogWarning("[UI] islandHealthSlider is NOT assigned!");
        }
    }

    public void UpdateLootProgress(float progress)
    {
        Debug.Log($"[UI] Setting loot progress: {progress}");

        if (lootingProgressSlider != null)
        {
            lootingProgressSlider.value = progress;
        }
        else
        {
            Debug.LogWarning("[UI] lootingProgressSlider is NOT assigned!");
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
