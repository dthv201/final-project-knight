using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        GameObject spawn  = GameObject.Find("ResPoint");

        if (player != null && spawn != null)
        {
            player.transform.position = spawn.transform.position;
            player.transform.rotation = spawn.transform.rotation;

            // Optional: reset velocity if using Rigidbody
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;
        }
    }
}
