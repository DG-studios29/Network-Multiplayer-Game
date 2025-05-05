using UnityEngine;
using UnityEngine.UI;
//
public class SliderRepairTest : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider healthBar;

    [Header("Repair Settings")]
    [SerializeField] private KeyCode repairKey = KeyCode.R;
    [SerializeField] private float repairDuration = 5f; 

    private bool isRepairing = false;
    private float repairProgress = 0f;
    private float initialHealth;

    private void Start()
    {
        if (healthBar == null)
        {
            Debug.LogError("HealthBar slider not assigned!");
            return;
        }

    
        if (healthBar.maxValue == 0)
        {
            healthBar.maxValue = 100;
            healthBar.value = 50;
        }

        Debug.Log("SliderRepairTest initialized. Hold 'R' to start repairing.");
    }

    private void Update()
    {
        if (healthBar == null) return;

        if (Input.GetKeyDown(repairKey) && !isRepairing && healthBar.value < healthBar.maxValue)
        {
            StartRepair();
        }

        if (Input.GetKeyUp(repairKey) && isRepairing)
        {
            CancelRepair();
        }

        if (isRepairing)
        {
            repairProgress += Time.deltaTime;
            float fraction = repairProgress / repairDuration;
            healthBar.value = Mathf.Lerp(initialHealth, healthBar.maxValue, fraction);
            Debug.Log($"Repairing... {Mathf.RoundToInt(fraction * 100)}%");

            if (repairProgress >= repairDuration)
            {
                CompleteRepair();
            }
        }
    }

    private void StartRepair()
    {
        isRepairing = true;
        repairProgress = 0f;
        initialHealth = healthBar.value;
        Debug.Log("Repair started.");
    }

    private void CancelRepair()
    {
        isRepairing = false;
        healthBar.value = initialHealth;
        Debug.Log("Repair canceled. Reverting to initial value.");
    }

    private void CompleteRepair()
    {
        isRepairing = false;
        healthBar.value = healthBar.maxValue;
        Debug.Log("Repair complete. Health is full.");
    }
}
