using UnityEngine;
using TMPro;

public class LootPopupManager : MonoBehaviour
{
    public static LootPopupManager Instance;

    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Transform popupParent;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowLootPopup(int goldAmount)
    {
        GameObject popup = Instantiate(popupPrefab, popupParent);
        TMP_Text text = popup.GetComponentInChildren<TMP_Text>();
        text.text = $"+{goldAmount} Gold!";

        popup.SetActive(true);

        // Optional: auto destroy after animation
        Destroy(popup, 2.5f); // matches animation duration
    }
}
