using UnityEngine;

public class WreckingBallSwing : MonoBehaviour
{
	public float swingAmplitude = 150f;     // Lowered for gentler force
	public float swingFrequency = 1.5f;    // Lower frequency = slower swing

	private PlayerStats player;

	public float attackDamage    = 50f;

	public Vector3 swingAxis = Vector3.right;

	private Rigidbody rb;

	private float lastHitTime = 0f;
	public float hitCooldown = 1f;

	void Start()
	{
		rb = GetComponent<Rigidbody>();

		 GameObject playerGO = GameObject.FindWithTag("Player");
		if (playerGO != null)
		{
			player = playerGO.GetComponent<PlayerStats>();
		}
	}


	void FixedUpdate()
	{
		// Apply smooth oscillating force using sine wave
		float force = Mathf.Sin(Time.time * swingFrequency) * swingAmplitude;
		rb.AddForce(swingAxis.normalized * force);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && Time.time - lastHitTime > hitCooldown)
		{
			lastHitTime = Time.time;
			player.TakeDamage(attackDamage);
		}
	}

}