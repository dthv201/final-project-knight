// PlayerMovementScript.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerMovementScript : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 720f;

    [Header("Jump Settings")]
    public float jumpSpeed = 5f;
    public float jumpButtonGracePeriod = 0.2f;

    private Animator            animator;
    private CharacterController cc;
    private float               yVelocity;
    private float?              lastGroundedTime;
    private float?              jumpPressedTime;
    private float               originalStepOffset;

    void Start()
    {
        animator = GetComponent<Animator>();
        cc       = GetComponent<CharacterController>();
        originalStepOffset = cc.stepOffset;
        animator.applyRootMotion = false; // manual movement
    }

    void Update()
    {
        // ─── JUMP & GRAVITY ──────────────────────────────────────────────
        if (cc.isGrounded) lastGroundedTime = Time.time;
        if (Input.GetButtonDown("Jump")) jumpPressedTime = Time.time;

        bool canJump =
            lastGroundedTime.HasValue
            && Time.time - lastGroundedTime <= jumpButtonGracePeriod
            && jumpPressedTime.HasValue
            && Time.time - jumpPressedTime <= jumpButtonGracePeriod;

        if (canJump)
        {
            yVelocity = jumpSpeed;
            animator.SetBool("IsJumping", true);
            lastGroundedTime = jumpPressedTime = null;
        }
        else if (cc.isGrounded)
        {
            yVelocity = -0.5f;
            animator.SetBool("IsJumping", false);
        }
        else
        {
            yVelocity += Physics.gravity.y * Time.deltaTime;
        }

        // ─── INPUT & MOVEMENT ────────────────────────────────────────────
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dir = (transform.forward * v + transform.right * h).normalized;

        Vector3 motion = dir * speed;
        motion.y = yVelocity;
        cc.stepOffset = cc.isGrounded ? originalStepOffset : 0;
        cc.Move(motion * Time.deltaTime);

        // ─── ROTATION ────────────────────────────────────────────────────
        Vector3 look = new Vector3(dir.x, 0, dir.z);
        if (look.sqrMagnitude > 0f)
        {
            Quaternion tgt = Quaternion.LookRotation(look);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                tgt,
                rotationSpeed * Time.deltaTime
            );
        }

        // ─── WALK / RUN ──────────────────────────────────────────────────
        bool walking = v > 0.1f;
        bool running = walking && Input.GetKey(KeyCode.LeftShift);
        animator.SetBool("IsWalking", walking);
        animator.SetBool("IsRunning", running);

        // ─── ATTACK & DEFEND AS BOOLS ───────────────────────────────────
        // Attack is only true the single frame you click Fire1:
        animator.SetBool("IsAttacking", Input.GetButtonDown("Fire1"));

        // Defend is true while you hold Fire2:
        animator.SetBool("IsDefending", Input.GetButton("Fire2"));
    }
}
