using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 20f;               // m/s
    public float lifetime = 5f;             // auto-destroy
    public bool useRigidbody = true;        // toggle if your asset already uses RB

    [Header("Damage")]
    public float damage = 25f;
    public LayerMask hittableLayers;        // e.g. Default, Player

    [Header("FX (optional)")]
    public GameObject hitVfxPrefab;

    [HideInInspector] public Transform owner;    // set by spawner to ignore self

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        // kill after lifetime
        if (lifetime > 0f) Destroy(gameObject, lifetime);

        // start velocity if using RB
        if (useRigidbody && rb != null)
            rb.linearVelocity = transform.forward * speed;
    }

    void Update()
    {
        if (!useRigidbody)
            transform.position += transform.forward * (speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // ignore owner (dragon) & any children
        if (owner != null && (other.transform == owner || other.transform.IsChildOf(owner))) return;

        // layer filter (0 mask == accept all)
        if (hittableLayers.value != 0 && ((1 << other.gameObject.layer) & hittableLayers) == 0) return;

        // damage player if present
        var stats = other.GetComponentInParent<PlayerStats>();
        if (stats != null)
            stats.TakeDamage(damage);

        // spawn hit VFX
        if (hitVfxPrefab)
            Instantiate(hitVfxPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
