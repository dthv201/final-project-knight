using UnityEngine;

public class WreckingBallSwing : MonoBehaviour
{
	public float swingAmplitude = 150f;     // Lowered for gentler force
	public float swingFrequency = 1.5f;    // Lower frequency = slower swing

	public PlayerStats player;

	public float attackDamage    = 50f;

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
	
	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			player.TakeDamage(attackDamage);
		}
	}

}