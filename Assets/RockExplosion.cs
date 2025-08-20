using UnityEngine;

public class RockExplosion : MonoBehaviour
{
    [Header("Setup")]
    public GameObject fracturedObject;       // פריפאב השברים בלבד!
    public float explosionForce = 500f;
    public float explosionRadius = 5f;
    public string activatorTag = "Player";   // או "Sword"

    [Header("Mode")]
    public bool useTrigger = true;           // true=OnTriggerEnter, false=OnCollisionEnter

    bool hasExploded;

    void Reset() {
        // מבטיח שהקוליידר יהיה טריגר כשצריך
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = useTrigger;
    }

    void OnValidate() {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = useTrigger;
    }

    void Start() {
        Debug.Log($"[Breakable] ready on {name}, trigger={useTrigger}, fractured={(fracturedObject?fracturedObject.name:"NULL")}");
    }

    void OnTriggerEnter(Collider other) {
        if (!useTrigger) return;
        Debug.Log($"[Breakable] trigger with {other.name} tag={other.tag}");
        if (other.CompareTag(activatorTag)) Explode();
    }

    void OnCollisionEnter(Collision c) {
        if (useTrigger) return;
        Debug.Log($"[Breakable] collision with {c.collider.name} tag={c.collider.tag}, v={c.relativeVelocity.magnitude:F1}");
        if (c.collider.CompareTag(activatorTag)) Explode(c.GetContact(0).point);
    }

    void Explode(Vector3? center = null) {
        if (hasExploded) return;
        if (fracturedObject == null) { Debug.LogWarning("[Breakable] fracturedObject is NULL"); return; }

        hasExploded = true;

        // יצירת השברים באותו מיקום/סיבוב + התאמת סקייל
        var frac = Instantiate(fracturedObject, transform.position, transform.rotation);
        frac.transform.localScale = transform.lossyScale;

        var origin = center ?? transform.position;

        foreach (var rb in frac.GetComponentsInChildren<Rigidbody>())
            rb.AddExplosionForce(explosionForce, origin, explosionRadius);

        // לא להשאיר סקריפט זה על הפריפאב השבור!
        Destroy(gameObject);
    }
}
