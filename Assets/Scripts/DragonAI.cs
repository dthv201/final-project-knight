// File: Assets/Scripts/DragonAI.cs
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class DragonAI : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth       = 200f;
    public float attackDamage    = 25f;
    public float attackInterval  = 10f;
    public float attackRange = 3f;

    [Header("References")]
    public PlayerStats player;    // drag your Player GameObject here
    public Animator animator;     // optional: for Attack/Hurt/Die triggers
    [Header("Chase")]
    public float chaseSpeed = 3f;

    private float currentHealth;
    private bool fighting;

    [Header("UI")]
    public Slider bossHealthSlider;
    void Awake()
    {
        currentHealth = maxHealth;

        if (bossHealthSlider != null)
            bossHealthSlider.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called by BossFightTrigger to start the encounter.
    /// </summary>
    public void BeginFight()
    {
        if (!fighting)
        {
            fighting = true;
            // show the boss bar
            if (bossHealthSlider != null)
            {
                bossHealthSlider.gameObject.SetActive(true);
                bossHealthSlider.maxValue = maxHealth;
                bossHealthSlider.value    = currentHealth;
            }
            StartCoroutine(AttackRoutine());
        }
    }


private IEnumerator AttackRoutine()
{
    while (fighting && currentHealth > 0f && player.currentHealth > 0f)
    {
        // compute direction and distance
        Vector3 dir = (player.transform.position - transform.position);
        dir.y = 0; 
        float dist = dir.magnitude;

        if (dist <= attackRange)
        {
            // Face the player
            transform.rotation = Quaternion.LookRotation(dir.normalized);
            // Attack
            animator?.SetTrigger("Attack");
            yield return new WaitForSeconds(0.5f);
            player.TakeDamage(attackDamage);
            yield return new WaitForSeconds(attackInterval - 0.5f);
        }
        else
        {
            // Chase: face + move forward
            transform.rotation = Quaternion.LookRotation(dir.normalized);
            transform.Translate(Vector3.forward * chaseSpeed * Time.deltaTime, Space.Self);
            yield return null;
        }
    }
        // who died?
        if (currentHealth <= 0f)
            Die();
        else if (player.currentHealth <= 0f)
            EndFight();
    }

    /// <summary>
    /// Call this from your player’s attack logic.
    /// </summary>
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Dragon took {amount} damage, now at {currentHealth}");
        if (bossHealthSlider != null)
            bossHealthSlider.value = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (animator != null)
            animator.SetTrigger("Hurt");

        if (currentHealth <= 0f)
            fighting = false;
    }

    private void Die()
    {
        if (bossHealthSlider != null)
            bossHealthSlider.gameObject.SetActive(false);

        if (animator != null)
            animator.SetTrigger("Die");
        GetComponent<Collider>().enabled = false;
    }

    private void EndFight()
    {
        fighting = false;
        // TODO: trigger Game Over or restart zone
    }
}
