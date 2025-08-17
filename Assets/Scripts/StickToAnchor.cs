// File: Assets/Scripts/StickToAnchor.cs
using UnityEngine;

[DefaultExecutionOrder(10000)]
public class StickToAnchor : MonoBehaviour
{
    public Transform anchor;                    // drag: cameraTarget
    public Vector3   localOffset = new Vector3(0f, 1.7f, 0.05f);
    public bool      matchRotation = true;      // set false if your look script handles rotation

    void LateUpdate()
    {
        if (!anchor) return;
        transform.position = anchor.TransformPoint(localOffset);
        if (matchRotation) transform.rotation = anchor.rotation;
    }
}
