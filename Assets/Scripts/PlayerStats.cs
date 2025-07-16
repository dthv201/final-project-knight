using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth;

    [Header("Stamina")]
    public float maxStamina   = 100f;
    public float staminaRegen = 10f;
    public float attackCost   = 20f;
    public float blockCost    = 10f;
    [HideInInspector] public float currentStamina;

    [Header("UI References")]
    public Slider healthBar;
    public Slider staminaBar;

    private void Awake()
    {
        currentHealth  = maxHealth;
        currentStamina = maxStamina;
    }

    private void Update()
    {
        // Regen stamina if not depleting
        if (!Input.GetButton("Fire1") && !Input.GetButton("Fire2"))
        {
            currentStamina += staminaRegen * Time.deltaTime;
        }

        // Clamp values
        currentHealth  = Mathf.Clamp(currentHealth,  0f, maxHealth);
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // Update UI
        if (healthBar != null)
            healthBar.value = currentHealth / maxHealth;
        if (staminaBar != null)
            staminaBar.value = currentStamina / maxStamina;
    }

    // Call this to damage the player
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        // TODO: trigger hurt animation, sounds, death check...
    }

    // Call this when you swing an attack
    public bool TryUseStamina(float cost)
    {
        if (currentStamina >= cost)
        {
            currentStamina -= cost;
            return true;
        }
        return false;
    }
}
