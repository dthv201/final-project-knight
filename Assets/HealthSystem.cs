using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Health reduced by {damage}. Current health: {health}");
    }

}
