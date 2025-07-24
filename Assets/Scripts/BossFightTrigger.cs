// File: BossFightTrigger.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BossFightTrigger : MonoBehaviour
{
    public DragonAI dragon;     // assign your Dragon GameObject's DragonAI component here

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // start the fight
            Debug.Log("BossFightTrigger: player entered trigger");
            dragon.BeginFight();
            // disable the trigger so it only fires once
            GetComponent<Collider>().enabled = false;
        }
    }
}
