using UnityEngine;

public class WreckingBallSwing : MonoBehaviour
{
    [Header("Swing Settings")]
    public float swingAmplitude = 150f;
    public float swingFrequency = 1.5f;
    public Vector3 swingAxis = Vector3.right;

    [Header("Damage Settings")]
    public float attackDamage = 50f;
    public float hitCooldown = 1f;

    [Header("Respawn")]
    public Transform respawnPoint;  // Drag the respawn point here (NOT from the player)

    private float lastHitTime = 0f;
    private Rigidbody rb;
    private PlayerStats player;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
        {
            player = playerGO.GetComponent<PlayerStats>();
        }
    }

    void FixedUpdate()
    {
        float force = Mathf.Sin(Time.time * swingFrequency) * swingAmplitude;
        rb.AddForce(swingAxis.normalized * force);
    }

  void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player") && Time.time - lastHitTime > hitCooldown)
    {
        lastHitTime = Time.time;

        if (player != null)
        {
            float hpBeforeHit = player.currentHealth;

            player.TakeDamage(attackDamage);
            Debug.Log($"[WreckingBall] Player hit! -{attackDamage} HP (from {hpBeforeHit})");

            if (Mathf.Approximately(hpBeforeHit, 50f))
            {
                TeleportPlayerToRespawn(player.transform);
            }
        }
    }
}


    void TeleportPlayerToRespawn(Transform playerTransform)
    {
        if (respawnPoint == null)
        {
            Debug.LogWarning("WreckingBall: Respawn point is not assigned!");
            return;
        }

        var controller = playerTransform.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        playerTransform.position = respawnPoint.position;

        if (controller != null) controller.enabled = true;
    }
}
