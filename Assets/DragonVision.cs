using UnityEngine;

public class DragonVision : MonoBehaviour
{
    [Tooltip("The player GameObject")]
    public Transform player;

    [Tooltip("Damage to apply on sight")]
    public float sightDamage = 50f;

    [Tooltip("Where to send the player when spotted")]
    public Transform savePoint;

    private PlayerStats playerStats;

    void Awake()
    {
        if (player != null)
            playerStats = player.GetComponent<PlayerStats>();
    }

    void OnTriggerEnter(Collider other)
    {
        // When the CharacterController walks into our trigger...
        if (other.transform.root == player && playerStats != null)
        {
            // 1) Damage
            playerStats.TakeDamage(sightDamage);
            Debug.Log($"DragonVision: spotted player, -{sightDamage} HP.");

            // 2) Teleport
            player.position = savePoint.position;
            Debug.Log("DragonVision: teleported player back to save point.");
        }
    }
}
