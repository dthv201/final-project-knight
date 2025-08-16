using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth;

    [Header("Respawn")]
    [Tooltip("Drag the GameObject you want to respawn at here")]
    public Transform respawnPoint;

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
    public Image staminaFill;
    public Color normalColor = Color.yellow;
    public Color alertColor  = Color.red;
    public float flashDuration = 0.2f;

    private Coroutine flashRoutine;
    private CharacterController controller;
    
      [Header("Invulnerability")]
    public float invulnDuration = 1f;
    private float lastHitTime = -999f;
    
    [HideInInspector]
    internal bool hasSpeedBoots = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        // If no respawn point set, use starting position
        if (respawnPoint == null)
        {
            GameObject go = new GameObject("RespawnPoint (Auto)");
            go.transform.position = transform.position;
            respawnPoint = go.transform;
        }
    }
    void Start()
    {
        currentHealth    = PlayerData.currentHealth;
        currentStamina   = PlayerData.currentStamina;
        hasSpeedBoots    = PlayerData.hasSpeedBoots;
    }


    private void Update()
    {
        // Regenerate stamina when not attacking, blocking, or sprinting
        bool isUsingStamina = Input.GetButton("Fire1")
                            || Input.GetButton("Fire2")
                            || (Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") > 0.1f);
        if (!isUsingStamina)
            currentStamina += staminaRegen * Time.deltaTime;

        // Clamp values
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // Update UI sliders
        if (healthBar != null) healthBar.value = currentHealth / maxHealth;
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

    /// <summary>
    /// Call this to damage the player.
    /// </summary>
public void TakeDamage(float amount)
{
    if (Time.time < lastHitTime + invulnDuration)
    {
        Debug.Log("Damage skipped—still invulnerable");
        return;
    }

    lastHitTime = Time.time;
    currentHealth -= amount;

    Debug.Log($"Player took {amount} damage, HP now {currentHealth}/{maxHealth}");

    if (currentHealth <= 0f)
    {
        Debug.Log("Player died.");
        Respawn(); // 🛡 Respawn instead of dying
    }
}


    
private void Respawn()
{
    Debug.Log("Player died — respawning.");

    currentHealth  = maxHealth;
    currentStamina = maxStamina;

    if (controller != null && respawnPoint != null)
    {
        controller.enabled = false;
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        controller.enabled = true;
    }

    if (healthBar)  healthBar.value  = currentHealth  / maxHealth;
    if (staminaBar) staminaBar.value = currentStamina / maxStamina;
}


    private void Die()
    {
        Debug.Log("Player died.");
        SceneManager.LoadScene("Lose"); 
    }

    /// <summary>
    /// Attempt to spend stamina; returns true if successful.
    /// </summary>
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
