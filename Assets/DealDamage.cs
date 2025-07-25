using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] private float  damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
           HealthSystem enemy = other.GetComponent<HealthSystem>();
            enemy.TakeDamage(damage);
        }
    }

}
