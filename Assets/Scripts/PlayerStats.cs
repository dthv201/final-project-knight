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
    public float maxStamina      = 100f;
    public float staminaRegen    = 10f;
    public float attackCost      = 20f;
    public float blockCost       = 50f;
    public float jumpStaminaCost = 20f;
    public float runStaminaCost  = 5f;
    [HideInInspector] public float currentStamina;

    [Header("UI References")]
    public Slider healthBar;
    public Slider staminaBar;
    public Image  staminaFill;
    public Color  normalColor = Color.yellow;
    public Color  alertColor  = Color.red;
    public float  flashDuration = 0.2f;

    private Coroutine flashRoutine;
    private CharacterController controller;

    [Header("Invulnerability")]
    public float invulnDuration = 1f;
    private float lastHitTime = -999f;

    [HideInInspector] internal bool hasSpeedBoots = false;

    void Awake()
    {
        controller      = GetComponent<CharacterController>();
        currentHealth   = maxHealth;
        currentStamina  = maxStamina;

        // If no respawn point set, use starting position
        if (respawnPoint == null)
        {
            GameObject go = new GameObject("RespawnPoint (Auto)");
            go.transform.position = transform.position;
            respawnPoint = go.transform;
        }
    }

    void OnEnable()  { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SaveToStash(); // ensure values are saved when object is destroyed on scene swap
    }

    void Start()
    {
        // Restore only if we actually saved something earlier
        if (PlayerData.hasValues)
        {
            Debug.Log("[PlayerStats] Restoring saved values.");
            currentHealth  = Mathf.Clamp(PlayerData.currentHealth,  0f, maxHealth);
            currentStamina = Mathf.Clamp(PlayerData.currentStamina, 0f, maxStamina);
            hasSpeedBoots  = PlayerData.hasSpeedBoots;
        }
        else
        {
            currentHealth  = maxHealth;
            currentStamina = maxStamina;
        }

        UpdateUI();
    }

    void Update()
    {
        // Regenerate stamina when not using it
        bool isUsingStamina = Input.GetButton("Fire1")
                           || Input.GetButton("Fire2")
                           || (Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") > 0.1f);
        if (!isUsingStamina)
            currentStamina += staminaRegen * Time.deltaTime;

        // Clamp values
        currentHealth  = Mathf.Clamp(currentHealth,  0f, maxHealth);
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        UpdateUI();
    }

    void UpdateUI()
    {
        if (healthBar)  healthBar.value  = currentHealth  / maxHealth;
        if (staminaBar) staminaBar.value = currentStamina / maxStamina;
        if (staminaFill && staminaBar)  staminaFill.color = normalColor; // ensure color reset
    }

    public void FlashStaminaBar()
    {
        if (!staminaFill) return;
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(DoFlash());
    }

    IEnumerator DoFlash()
    {
        staminaFill.color = alertColor;
        yield return new WaitForSeconds(flashDuration);
        staminaFill.color = normalColor;
    }

    public void TakeDamage(float amount)
    {
        if (Time.time < lastHitTime + invulnDuration)
        {
            Debug.Log("Damage skipped—still invulnerable");
            return;
        }

        lastHitTime   = Time.time;
        currentHealth -= amount;

        Debug.Log($"Player took {amount} damage, HP now {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            Debug.Log("Player died.");
            Respawn();
        }
    }

    void Respawn()
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

        UpdateUI();
        SaveToStash(); // keep stash up-to-date after respawn
    }

    // Keep if you have a Lose scene somewhere else
    void Die()
    {
        Debug.Log("Player died.");
        SaveToStash();
        SceneManager.LoadScene("Lose");
    }

    public bool TryUseStamina(float cost)
    {
        if (currentStamina >= cost)
        {
            currentStamina -= cost;
            UpdateUI();
            return true;
        }
        return false;
    }

    // ======== SCENE HANDOFF CORE ========

    public void SaveToStash()
    {
        PlayerData.currentHealth  = currentHealth;
        PlayerData.currentStamina = currentStamina;
        PlayerData.hasSpeedBoots  = hasSpeedBoots;
        PlayerData.hasValues      = true;
    }

    void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        // Rebind per-scene references that get destroyed between scenes

        // Respawn point by tag or name
        if (!respawnPoint)
        {
            var rpObj = GameObject.FindWithTag("RespawnPoint") ?? GameObject.Find("ResPoint");
            if (rpObj) respawnPoint = rpObj.transform;
        }

        // UI rebind (only if not assigned via Inspector in this scene)
        if (!healthBar)
            healthBar = GameObject.Find("Health Bar")?.GetComponent<Slider>();
        if (!staminaBar)
            staminaBar = GameObject.Find("Stamina Bar")?.GetComponent<Slider>();
        if (!staminaFill && staminaBar && staminaBar.fillRect)
            staminaFill = staminaBar.fillRect.GetComponent<Image>();

        UpdateUI();
    }
}
