using UnityEngine;

public class BlenderExplosionTrigger : MonoBehaviour
{
	public GameObject fracturedObject;
	public float explosionForce = 500f;
	public float explosionRadius = 5f;
	public string playerTag = "Sword"; // Ensure your player GameObject is tagged with "Player"

	private bool hasExploded = false;

	private void OnTriggerEnter(Collider other)
	{
		if (!hasExploded && other.CompareTag(playerTag))
		{
			hasExploded = true;

			// Spawn fractured object at same position and rotation
			GameObject fracturedInstance = Instantiate(fracturedObject, transform.position, transform.rotation);

			// Apply explosion force to each fragment
			foreach (Rigidbody rb in fracturedInstance.GetComponentsInChildren<Rigidbody>())
			{
				rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
			}

			// Destroy the original object
			Destroy(gameObject);
		}
	}
}
