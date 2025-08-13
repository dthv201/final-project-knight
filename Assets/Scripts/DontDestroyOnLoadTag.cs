using UnityEngine;

public class DontDestroyOnLoadTag : MonoBehaviour
{
    void Awake()
    {
        // Prevent duplicate on reload
        if (GameObject.FindObjectsOfType<DontDestroyOnLoadTag>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
