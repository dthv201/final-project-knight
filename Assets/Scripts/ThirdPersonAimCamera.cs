using UnityEngine;
using UnityEngine.SceneManagement;

public class ThirdPersonAimCamera : MonoBehaviour
{
    [Header("Target (player root or NON-animated pivot)")]
    public Transform target;

    [Header("Mode")]
    public bool firstPerson = false;                         // ON for FP camera, OFF for TP camera
    public Vector3 fpLocalOffset = new Vector3(0f, 0f, 0.06f);

    [Header("Third-Person")]
    public float distance = 4.5f;
    public float height   = 2.0f;

    [Header("Look")]
    public float sensitivityX = 140f;
    public float sensitivityY = 120f;
    public float minPitch = -40f;
    public float maxPitch =  70f;
    public bool invertY   = false;

    [Header("Cursor")]
    public bool lockCursor = true;

    // Shared yaw/pitch for BOTH cameras (no snap when switching)
    private static bool  sInit;
    private static float sYaw;    // world yaw
    private static float sPitch;  // world pitch

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void OnEnable()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }

        // Initialize angles only once (prevents "jump back" on switch)
        if (!sInit)
        {
            sYaw = target ? target.eulerAngles.y : transform.eulerAngles.y;

            // signed pitch from current forward
            Vector3 fwd = transform.forward.normalized;
            sPitch = Mathf.Asin(Mathf.Clamp(fwd.y, -1f, 1f)) * Mathf.Rad2Deg;
            sPitch = Mathf.Clamp(sPitch, minPitch, maxPitch);
            sInit  = true;
        }
    }

    void LateUpdate()
    {
        if (!target || cam == null || !cam.enabled) return;

        float mx = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float my = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        sYaw   += mx;
        sPitch += (invertY ?  my : -my);          // tick invertY if you prefer opposite
        sPitch  = Mathf.Clamp(sPitch, minPitch, maxPitch);

        Quaternion rot = Quaternion.Euler(sPitch, sYaw, 0f);

        if (firstPerson)
        {
 
            // FP: DO NOT parent to animated head; place by offset and use rot directly
            transform.position = target.TransformPoint(fpLocalOffset);
            transform.rotation = rot;
        }
        else
        {
     
            // TP: orbit from pivot; NO LookAt (avoids fighting rotation)
            Vector3 pivot = target.position + Vector3.up * height;
            Vector3 back  = rot * Vector3.back;
            transform.position = pivot + back * distance;
            transform.rotation = rot;
        }
    }
}
