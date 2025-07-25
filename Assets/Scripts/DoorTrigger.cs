using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class DoorTrigger : MonoBehaviour
{
    [Tooltip("Name of the scene to load when the player enters the trigger")]
    public string nextSceneName;

    [Tooltip("Tag of the GameObject that can open this door (e.g. \"Player\")")]
    public string activatingTag = "Player";

    void Awake()
    {
        // Ensure the collider is set as a trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Only react to the tagged object (e.g. your Player)
        if (other.CompareTag(activatingTag))
        {
            if (string.IsNullOrEmpty(nextSceneName))
            {
                Debug.LogError($"[DoorTrigger] nextSceneName is empty on {gameObject.name}");
                return;
            }
            Debug.Log($"[DoorTrigger] Loading scene '{nextSceneName}'");
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
