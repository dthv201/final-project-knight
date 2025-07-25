using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class TeleportTrigger : MonoBehaviour
{
    [Tooltip("Where the player will end up")]
    public Transform teleportPoint;

    void Awake()
    {
        // ensure it really is a trigger
        GetComponent<Collider>().isTrigger = true;

        // kinematic Rigidbody to receive trigger events
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // grab their CharacterController so we can disable it
            var cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                other.transform.position = teleportPoint.position;
                other.transform.rotation = teleportPoint.rotation;
                cc.enabled = true;
            }
            else
            {
                // fallback if you somehow don't have a CC
                other.transform.position = teleportPoint.position;
                other.transform.rotation = teleportPoint.rotation;
            }
        }
    }
}