using UnityEngine;
using Unity.Cinemachine;


public class CameraSwitcher : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineFreeLook thirdPersonCam;     // your FreeLook rig
    public CinemachineVirtualCamera firstPersonCam; // your FPS rig

    [Header("Key to Switch")]
    public KeyCode switchKey = KeyCode.V;

    // Name of the layer you created for your player mesh
    const string PlayerBodyLayerName = "PlayerBody";

    private bool usingFirstPerson = false;
    private Camera mainCam;
    private int playerBodyLayerMask;

    void Start()
    {
        // Cache references
        mainCam = Camera.main;
        thirdPersonCam.gameObject.SetActive(true);
        firstPersonCam.gameObject.SetActive(false);

        // Build a bitmask for your PlayerBody layer
        int layerIndex = LayerMask.NameToLayer(PlayerBodyLayerName);
        playerBodyLayerMask = 1 << layerIndex;
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
            ToggleCameras();
    }

    void ToggleCameras()
    {
        usingFirstPerson = !usingFirstPerson;

        // Switch the rigs
        thirdPersonCam.gameObject.SetActive(!usingFirstPerson);
        firstPersonCam.gameObject.SetActive(usingFirstPerson);

        // Modify culling mask so PlayerBody is only visible in 3rd-person
        if (usingFirstPerson)
        {
            // remove PlayerBody bit
            mainCam.cullingMask &= ~playerBodyLayerMask;
        }
        else
        {
            // add PlayerBody bit back
            mainCam.cullingMask |= playerBodyLayerMask;
        }
    }
}
