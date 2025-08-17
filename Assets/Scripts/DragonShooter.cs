using UnityEngine;

public class DragonShooter : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject fireballPrefab;
    public Transform fireSpawnPoint;
    public float fireballSpeed = 20f;
    public float fireCooldown = 2f;
    public Transform dragonRespawnPoint;


    [Header("Vision Settings")]
    public float rayLength = 70f;
    public LayerMask playerLayer;
    public Transform player;

    [Header("Debug")]
    public bool drawRays = true;

    float lastFireTime = -999f;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!player) { Debug.LogWarning("[Dragon] No player set"); return; }
        if (!fireSpawnPoint) { Debug.LogWarning("[Dragon] No fireSpawnPoint set"); return; }
        if (!fireballPrefab) { Debug.LogWarning("[Dragon] No fireballPrefab set"); return; }

        // face player (Y only)
        Vector3 flat = player.position - transform.position; flat.y = 0f;
        if (flat.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(flat), Time.deltaTime * 3f);

        // ray from muzzle to player's chest
        Vector3 origin = fireSpawnPoint.position;
        Vector3 target = player.position + Vector3.up * 1.0f;
        Vector3 dir = (target - origin).normalized;

        if (drawRays) Debug.DrawRay(origin, dir * rayLength, Color.red);

        if (Physics.Raycast(origin, dir, out RaycastHit hit, rayLength, playerLayer))
        {
            if (drawRays) Debug.DrawRay(hit.point, Vector3.up * 0.5f, Color.green, 0.05f);
            if (hit.transform.CompareTag("Player"))
            {
                TryShoot(dir);
            }
            else
            {
                Debug.Log($"[Dragon] LOS blocked by {hit.collider.name} (layer {LayerMask.LayerToName(hit.collider.gameObject.layer)})");
            }
        }
    }

    void TryShoot(Vector3 dir)
    {
        if (Time.time < lastFireTime + fireCooldown) return;
        lastFireTime = Time.time;

        GameObject fireball = Instantiate(fireballPrefab, fireSpawnPoint.position, Quaternion.LookRotation(dir));

        if (fireball.GetComponent<Rigidbody>() is Rigidbody rb)
        {
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = dir * fireballSpeed;
        }

        if (animator) animator.SetTrigger("Shoot");
        Debug.Log("[Dragon] Shoot");
    }
    
    void TeleportPlayerToDragonRespawn()
    {
        if (dragonRespawnPoint == null || player == null)
        {
            Debug.LogWarning("[Dragon] No dragon respawn point assigned!");
            return;
        }

        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.position = dragonRespawnPoint.position;
        player.rotation = dragonRespawnPoint.rotation;

        if (cc != null) cc.enabled = true;
    }

}
