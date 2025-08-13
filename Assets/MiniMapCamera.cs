using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 50, 0);

    void LateUpdate()
    {
        // Fix: reconnect player if needed
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
            else
                return; // still no player, skip this frame
        }

        transform.position = player.position + offset;
    }
}
