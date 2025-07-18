using UnityEngine;

public class MouseLook : MonoBehaviour {
  public float sensitivity = 2f;
  float yaw, pitch;
  void Update() {
    yaw   += Input.GetAxis("Mouse X") * sensitivity;
    pitch -= Input.GetAxis("Mouse Y") * sensitivity;
    pitch = Mathf.Clamp(pitch, -80, 80);
    transform.localEulerAngles = new Vector3(pitch, yaw, 0);
    // rotate the body on Y only
    Camera.main.transform.parent.rotation = Quaternion.Euler(0, yaw, 0);
  }
}
