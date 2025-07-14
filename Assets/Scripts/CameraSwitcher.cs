using UnityEngine;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineFreeLook thirdPersonCam;   // your existing 3rd-person FreeLook
    public CinemachineVirtualCamera firstPersonCam; // new FPS virtual camera

    [Header("Key to Switch")]
    public KeyCode switchKey = KeyCode.V;

    private bool usingFirstPerson = false;

    void Start()
    {
        // start in third-person
        thirdPersonCam.gameObject.SetActive(true);
        firstPersonCam.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
            ToggleCameras();
    }

    void ToggleCameras()
    {
        usingFirstPerson = !usingFirstPerson;
        thirdPersonCam.gameObject.SetActive(!usingFirstPerson);
        firstPersonCam.gameObject.SetActive(usingFirstPerson);
    }
}
