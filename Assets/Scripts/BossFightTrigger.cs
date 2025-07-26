// File: BossFightTrigger.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BossFightTrigger : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // start the fight
            Debug.Log("BossFightTrigger: player entered trigger");
            // disable the trigger so it only fires once
            GetComponent<Collider>().enabled = false;
        }
    }
}
