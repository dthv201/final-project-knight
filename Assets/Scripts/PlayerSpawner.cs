// using UnityEngine;

// public class PlayerSpawner : MonoBehaviour
// {
//     public GameObject playerPrefab;  // assign in Inspector
//     public Transform spawnPoint;     // assign in Inspector

//     void Start()
//     {
//         if (GameObject.FindWithTag("Player") != null)
//         {
//             Debug.Log("[PlayerSpawner] Player already exists — not spawning again.");
//             return;
//         }

//         if (!playerPrefab || !spawnPoint)
//         {
//             Debug.LogError("PlayerSpawner is missing prefab or spawn point.");
//             return;
//         }

//         GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
//         player.tag = "Player";

//         // DontDestroyOnLoad(player);

//         Debug.Log("[PlayerSpawner] Spawned new player.");
//     }
// }
