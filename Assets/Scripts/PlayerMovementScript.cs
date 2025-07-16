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
        var stats      = GetComponent<PlayerStats>();
        bool pressJump = Input.GetButtonDown("Jump");
        bool holdShift = Input.GetKey(KeyCode.LeftShift);
        bool pressAtk  = Input.GetButtonDown("Fire1");
        bool holdBlk   = Input.GetButton("Fire2");
        float h        = Input.GetAxisRaw("Horizontal");
        float v        = Input.GetAxisRaw("Vertical");

        // ─── 1) Jump Input & Stamina ───────────────────────────
        if (cc.isGrounded) lastGroundedTime = Time.time;
        if (pressJump)   jumpPressedTime  = Time.time;

        bool canJump =
            lastGroundedTime.HasValue &&
            Time.time - lastGroundedTime <= jumpButtonGracePeriod &&
            jumpPressedTime.HasValue &&
            Time.time - jumpPressedTime <= jumpButtonGracePeriod;

        if (canJump)
        {
            if (stats.TryUseStamina(stats.jumpStaminaCost))
            {
                // FIRE THE TRIGGER, not a Bool
                animator.SetTrigger("JumpTrigger");
                yVelocity = jumpSpeed;
            }
            else
            {
                stats.FlashStaminaBar();
            }
            lastGroundedTime = jumpPressedTime = null;
        }

        // ─── 2) Gravity ────────────────────────────────────────
        yVelocity += Physics.gravity.y * Time.deltaTime;

        // ─── 3) Camera-relative input ──────────────────────────
        Transform cam = Camera.main.transform;
        Vector3 camF  = cam.forward; camF.y = 0; camF.Normalize();
        Vector3 camR  = cam.right;   camR.y = 0; camR.Normalize();
        Vector3 dir   = (camF * v + camR * h).normalized;

        // ─── 4) Run/walk + drain ──────────────────────────────
        bool walking = dir.sqrMagnitude > 0f;
        bool running = walking && holdShift;
        if (running)
        {
            float cost = stats.runStaminaCost * Time.deltaTime;
            if (stats.currentStamina >= cost)
                stats.currentStamina -= cost;
            else
                running = false;
        }
        animator.SetBool("IsWalking", walking);
        animator.SetBool("IsRunning", running);

        float speed = running ? runSpeed : walkSpeed;
        Vector3 motion = dir * speed;
        motion.y = yVelocity;

        // ─── 5) Move & stick ───────────────────────────────────
        var flags = cc.Move(motion * Time.deltaTime);
        if ((flags & CollisionFlags.Below) != 0 && yVelocity < 0f)
        {
            yVelocity = -2f;
            // no more animator.SetBool("IsJumping", false);
        }

        // ─── 6) Rotation ───────────────────────────────────────
        if (dir.sqrMagnitude > 0f)
        {
            var tgt = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                tgt,
                rotationSpeed * Time.deltaTime
            );
        }

        // ─── 7) Attack ─────────────────────────────────────────
        if (pressAtk)
        {
            if (stats.TryUseStamina(stats.attackCost))
                animator.SetTrigger("Attack");
            else
                stats.FlashStaminaBar();
        }

        // ─── 8) Defend ─────────────────────────────────────────
        if (holdBlk)
        {
            float blockCostThisFrame = stats.blockCost * Time.deltaTime;
            if (stats.currentStamina >= blockCostThisFrame)
            {
                stats.currentStamina -= blockCostThisFrame;
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
            animator.SetBool("IsDefending", false);
        }
    }
}
