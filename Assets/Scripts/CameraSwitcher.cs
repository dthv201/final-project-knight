using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Camera GameObjects (each has Camera + ThirdPersonAimCamera)")]
    public GameObject firstPersonCamGO;
    public GameObject thirdPersonCamGO;

    [Header("Keybinds")]
    public KeyCode switchKey = KeyCode.V;

    private Camera fpCam;
    private Camera tpCam;

    private ThirdPersonAimCamera fpLook;
    private ThirdPersonAimCamera tpLook;

    void Awake()
    {
        if (!firstPersonCamGO || !thirdPersonCamGO)
        {
            Debug.LogError("CameraSwitcher: Assign both camera GameObjects.");
            enabled = false;
            return;
        }

        // Cache all components
        fpCam  = firstPersonCamGO.GetComponent<Camera>();
        tpCam  = thirdPersonCamGO.GetComponent<Camera>();
        fpLook = firstPersonCamGO.GetComponent<ThirdPersonAimCamera>();
        tpLook = thirdPersonCamGO.GetComponent<ThirdPersonAimCamera>();

        if (!fpCam || !tpCam || !fpLook || !tpLook)
        {
            Debug.LogError("CameraSwitcher: Both cameras must have Camera and ThirdPersonAimCamera.");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        // Start in third person
        SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            Debug.Log("Switching camera mode");
            bool isTPActive = thirdPersonCamGO.activeSelf;
            SetActive(!isTPActive);
        }
    }

    void SetActive(bool useThirdPerson)
{
    thirdPersonCamGO.SetActive(useThirdPerson);
    firstPersonCamGO.SetActive(!useThirdPerson);

    if (tpLook) tpLook.firstPerson = !useThirdPerson;
    if (fpLook) fpLook.firstPerson = useThirdPerson;

    // Camera tag
    if (tpCam) tpCam.tag = useThirdPerson ? "MainCamera" : "Untagged";
    if (fpCam) fpCam.tag = useThirdPerson ? "Untagged"   : "MainCamera";

    // Audio
    var fpAL = firstPersonCamGO.GetComponent<AudioListener>();
    var tpAL = thirdPersonCamGO.GetComponent<AudioListener>();
    if (fpAL) fpAL.enabled = !useThirdPerson;
    if (tpAL) tpAL.enabled =  useThirdPerson;
}

}