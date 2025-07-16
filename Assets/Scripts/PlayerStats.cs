using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth;

    [Header("Stamina")]
    public float maxStamina       = 100f;
    public float staminaRegen     = 10f;
    public float attackCost       = 20f;
    public float blockCost        = 50f;
    public float jumpStaminaCost  = 20f;
    public float runStaminaCost   = 5f;
    [HideInInspector] public float currentStamina;

    [Header("UI References")]
    public Slider healthBar;
    public Slider staminaBar;
    
    public Image staminaFill;    // hook up the Fill Image of your StaminaBar
    public Color normalColor = Color.yellow;
    public Color alertColor  = Color.red;
    public float flashDuration = 0.2f;
    private Coroutine flashRoutine;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    private void Update()
    {
        // Regen when neither attacking nor blocking nor running
        bool isUsingStamina = Input.GetButton("Fire1")
                            || Input.GetButton("Fire2")
                            || Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") > 0.1f;
        if (!isUsingStamina)
            currentStamina += staminaRegen * Time.deltaTime;

        // Clamp
        currentHealth  = Mathf.Clamp(currentHealth,  0f, maxHealth);
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // Update UI
        if (healthBar  != null) healthBar.value  = currentHealth  / maxHealth;
        if (staminaBar != null) staminaBar.value = currentStamina / maxStamina;
    }

    public void FlashStaminaBar()
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(DoFlash());
    }

    private IEnumerator DoFlash()
    {
        staminaFill.color = alertColor;
        yield return new WaitForSeconds(flashDuration);
        staminaFill.color = normalColor;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
    }

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
