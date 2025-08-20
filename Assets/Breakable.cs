using UnityEngine;

public class Breakable : MonoBehaviour {
    public GameObject fracturedPrefab;
    public string activatorTag = "Player"; // או "Sword"
    public float explosionForce = 400f;
    public float explosionRadius = 3f;

    bool broken;

    void OnTriggerEnter(Collider other) {
        if (broken) return;
        if (!other.CompareTag(activatorTag)) return;

        broken = true;
        var frac = Instantiate(fracturedPrefab, transform.position, transform.rotation);
        frac.transform.localScale = transform.lossyScale;

        foreach (var rb in frac.GetComponentsInChildren<Rigidbody>())
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

        Destroy(gameObject);
    }
}
