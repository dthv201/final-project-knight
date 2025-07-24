// put this on your PlayerMovementScript (or a new Teleport script on the Player)
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Tooltip("Drag your Dragon's 'TeleportPoint' here")]
    public Transform teleportPoint;

    CharacterController cc;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // e.g. press 'T' to teleport
        if (Input.GetKeyDown(KeyCode.T))
            DoTeleport();
    }

    void DoTeleport()
    {
        // disable the CharacterController so we can warp
        cc.enabled = false;

        // snap the player to the point
        transform.position = teleportPoint.position;

        // if you want to face the dragon:
        Vector3 flatDir = teleportPoint.parent.position - transform.position;
        flatDir.y = 0;
        if (flatDir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(flatDir);

        // re-enable
        cc.enabled = true;
    }
}
