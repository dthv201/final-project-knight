using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public float damage = 25f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            Debug.Log("Hit the boss!");
            var dragon = other.GetComponent<DragonAI>();
            if (dragon != null)
                dragon.TakeDamage(damage);
        }
    }
}
