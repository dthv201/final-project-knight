// File: Assets/Scripts/WreckingBallSwing.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WreckingBallSwing : MonoBehaviour
{
    [Header("Swing Settings")]
    public float swingAmplitude = 150f;
    public float swingFrequency = 1.5f;
    public Vector3 swingAxis = Vector3.right;

    [Header("Damage Settings")]
    public float attackDamage = 50f;
    public float hitCooldown  = 1f;

    [Header("Respawn (assign a point in the scene)")]
    public Transform respawnPoint;

    [Header("Optional Teleport Rule")]
    public bool  teleportOnLowHP   = true;
    public float teleportBelowHP   = 50f;   // teleport if HP after hit is <= this

    Rigidbody rb;
    float     lastHitTime;
    Vector3   _axisNorm;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _axisNorm = swingAxis.sqrMagnitude > 0f ? swingAxis.normalized : Vector3.right;

        // Safety: recommend trigger on the ball collider so OnTriggerEnter fires reliably.
        var col = GetComponent<Collider>();
        if (col && !col.isTrigger)
            Debug.LogWarning("[WreckingBall] Collider is not set as Trigger. Consider enabling isTrigger.");
    }

    void FixedUpdate()
    {
        // Simple driven “pendulum-like” push
        float force = Mathf.Sin(Time.time * swingFrequency) * swingAmplitude;
        rb.AddForce(_axisNorm * force);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastHitTime < hitCooldown) return;

        // Find PlayerStats on the collider OR any of its parents (handles child-bone colliders)
        var stats = other.GetComponent<PlayerStats>() ?? other.GetComponentInParent<PlayerStats>();
        if (!stats)
        {
            // Fallback in edge cases (late spawns / tag on child): grab the first PlayerStats in scene
            stats = FindObjectOfType<PlayerStats>();
            if (!stats)
            {
                Debug.LogWarning("[WreckingBall] PlayerStats component not found!");
                return;
            }
        }

        lastHitTime = Time.time;

        float hpBefore = stats.currentHealth;
        stats.TakeDamage(attackDamage);
        Debug.Log($"[WreckingBall] Hit for -{attackDamage}. HP: {hpBefore} → {stats.currentHealth}");

        // Teleport decision is based on HP AFTER damage, not exactly-equal checks
        if (teleportOnLowHP && stats.currentHealth <= teleportBelowHP && respawnPoint)
        {
            TeleportPlayerToRespawn(stats.transform);
        }
    }

    void TeleportPlayerToRespawn(Transform playerTransform)
    {
        if (!respawnPoint)
        {
            Debug.LogWarning("[WreckingBall] Respawn point not assigned!");
            return;
        }

        var controller = playerTransform.GetComponent<CharacterController>();
        if (controller) controller.enabled = false;

        playerTransform.position = respawnPoint.position;
        playerTransform.rotation = respawnPoint.rotation;

        if (controller) controller.enabled = true;
    }
}
