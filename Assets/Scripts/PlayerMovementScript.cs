using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerMovementScript : MonoBehaviour
{
    [Header("Walk/Run Speeds")]
    public float walkSpeed     = 5f;
    public float runSpeed      = 8f;
    public float rotationSpeed = 540f;

    [Header("Jump Settings")]
    public float jumpSpeed             = 5f;
    public float jumpButtonGracePeriod = 0.2f;

    private Animator            animator;
    private CharacterController cc;
    private float               yVelocity;
    private float?              lastGroundedTime;
    private float?              jumpPressedTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        cc       = GetComponent<CharacterController>();
        animator.applyRootMotion = false;
    }

    void Update()
    {
     // Cache stats once
var stats = GetComponent<PlayerStats>();

// ——— ATTACK ———
if (Input.GetButtonDown("Fire1"))
{
    // only flash on insufficient stamina
    if (stats.TryUseStamina(stats.attackCost))
    {
        animator.SetTrigger("Attack");
    }
    else
    {
        stats.FlashStaminaBar();
        // play “no stamina” SFX if you like
    }
}

// ——— DEFEND ———
if (Input.GetButton("Fire2"))
{
    float blockThisFrame = stats.blockCost * Time.deltaTime;
    if (stats.currentStamina >= blockThisFrame)
    {
        stats.currentStamina -= blockThisFrame;
        animator.SetBool("IsDefending", true);
    }
    else
    {
        animator.SetBool("IsDefending", false);
        stats.FlashStaminaBar();
    }
}
else
{
    // you released the button: just turn it off, no flash
    animator.SetBool("IsDefending", false);
}

    }
}
