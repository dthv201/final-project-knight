using UnityEngine;

public class WreckingBallSwing : MonoBehaviour
{
	public float swingAmplitude = 10f;     // Lowered for gentler force
	public float swingFrequency = 0.5f;    // Lower frequency = slower swing
	public Vector3 swingAxis = Vector3.right;

	private Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		// Apply smooth oscillating force using sine wave
		float force = Mathf.Sin(Time.time * swingFrequency) * swingAmplitude;
		rb.AddForce(swingAxis.normalized * force);
	}

}