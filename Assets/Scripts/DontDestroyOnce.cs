using UnityEngine;

public class DontDestroyOnce : MonoBehaviour
{
    private static string rootName;

    void Awake()
    {
        rootName = gameObject.name;

        GameObject[] all = GameObject.FindGameObjectsWithTag("MainCamera");
        foreach (var cam in all)
        {
            if (cam != gameObject && cam.name == rootName)
            {
                Debug.Log($"[DontDestroyOnce] Destroying duplicate: {gameObject.name}");
                Destroy(gameObject);
                return;
            }
        }

        DontDestroyOnLoad(gameObject);
    }
}
