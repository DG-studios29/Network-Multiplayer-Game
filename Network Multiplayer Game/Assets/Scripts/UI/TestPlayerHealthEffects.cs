using UnityEngine;
using UnityEngine.UI;

public class TestPlayerHealthEffects : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Repair Settings")]
    public KeyCode repairKey = KeyCode.R;
    public float repairDuration = 5f;

    private bool isRepairing = false;
    private float repairProgress = 0f;
    private int healthBeforeRepair;

    [Header("UI")]
    [Tooltip("Drag the HealthBar Slider here manually from the prefab hierarchy.")]
    public Slider healthBar;

    [Header("Particle Effects")]
    public ParticleSystem damageEffect;
    public ParticleSystem healingEffect;
    public int criticalHealthThreshold = 30;

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBar == null)
        {
            Debug.LogWarning("⚠️ Health bar not assigned in the Inspector!");
        }
        else
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

       
        if (damageEffect != null) damageEffect.Stop();
        if (healingEffect != null) healingEffect.Stop();
    }

    private void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }

        
        if (Input.GetKey(repairKey) && !isRepairing && currentHealth < maxHealth)
        {
            Debug.Log("🔧 Repair started");
            isRepairing = true;
            repairProgress = 0f;
            healthBeforeRepair = currentHealth;

            
            if (healingEffect != null && !healingEffect.isPlaying)
            {
                healingEffect.transform.position = transform.position;
                healingEffect.Play();
            }
        }

        
        if (Input.GetKeyUp(repairKey) && isRepairing)
        {
            Debug.Log("❌ Repair cancelled");
            isRepairing = false;
            repairProgress = 0f;
            currentHealth = healthBeforeRepair;
            UpdateHealthBar();

          
            if (healingEffect != null && healingEffect.isPlaying)
            {
                healingEffect.Stop();
            }
        }

        
        if (isRepairing)
        {
            repairProgress += Time.deltaTime;
            if (repairProgress >= repairDuration)
            {
                Debug.Log("✅ Repair completed");
                isRepairing = false;
                currentHealth = maxHealth;
                UpdateHealthBar();

                
                if (healingEffect != null && healingEffect.isPlaying)
                {
                    healingEffect.Stop();
                }
            }
        }

       
        if (currentHealth <= criticalHealthThreshold)
        {
            
            if (damageEffect != null && !damageEffect.isPlaying)
            {
                damageEffect.transform.position = transform.position;
                damageEffect.Play();
            }
        }
        else
        {
          
            if (damageEffect != null && damageEffect.isPlaying)
            {
                damageEffect.Stop();
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"🔥 Took {amount} damage. Health: {currentHealth}/{maxHealth}");
        UpdateHealthBar();

        
        if (currentHealth <= criticalHealthThreshold)
        {
            if (damageEffect != null && !damageEffect.isPlaying)
            {
                
                damageEffect.transform.position = transform.position;
                damageEffect.Play();
            }
        }
        else
        {
            if (damageEffect != null && damageEffect.isPlaying)
            {
                damageEffect.Stop();
            }
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
    }
}
