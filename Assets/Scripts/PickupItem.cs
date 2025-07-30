using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum PickupType { SpeedBoost, HealthRestore }

    [Header("Pickup Settings")]
    public PickupType type;
    public float healthAmount = 50f;
    public float speedMultiplier = 1.3f;   // used for speed boost
    public AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats == null) return;

        switch (type)
        {
            case PickupType.SpeedBoost:
                if (!stats.hasSpeedBoots)
                {
                    stats.hasSpeedBoots = true;
                    Debug.Log("Speed boots collected!");
                }
                break;

            case PickupType.HealthRestore:
                float before = stats.currentHealth;
                stats.currentHealth = Mathf.Min(stats.currentHealth + healthAmount, stats.maxHealth);
                Debug.Log($"Health restored: {before} → {stats.currentHealth}");
                break;
        }

        // Optional pickup sound
        if (pickupSound)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        Destroy(gameObject);
    }
}
