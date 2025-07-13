// PlayerMovementScript.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerMovementScript : MonoBehaviour
{
    [Header("Walk/Run Speeds")]
    public float walkSpeed     = 5f;   // speed when just walking
    public float runSpeed      = 8f;   // speed when holding Shift
    public float rotationSpeed = 540f; // degrees per second to turn

    [Header("Jump Settings")]
    public float jumpSpeed           = 5f;
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
        animator.applyRootMotion = false;  // manual movement
    }

    void Update()
    {
        // ─── JUMP & GRAVITY ────────────────────────────────────
        if (cc.isGrounded)
            lastGroundedTime = Time.time;
        if (Input.GetButtonDown("Jump"))
            jumpPressedTime = Time.time;

        bool canJump =
            lastGroundedTime.HasValue &&
            Time.time - lastGroundedTime <= jumpButtonGracePeriod &&
            jumpPressedTime.HasValue &&
            Time.time - jumpPressedTime <= jumpButtonGracePeriod;

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

        // ─── INPUT & CAMERA-RELATIVE MOVEMENT ───────────────────
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // get camera axes
        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward; camForward.y = 0; camForward.Normalize();
        Vector3 camRight   = cam.right;   camRight.y   = 0; camRight.Normalize();

        // direction relative to camera
        Vector3 dir = (camForward * v + camRight * h).normalized;

        // determine walk vs run
        bool walking = v > 0.1f || h != 0;  // any input
        bool running = walking && Input.GetKey(KeyCode.LeftShift);
        animator.SetBool("IsWalking", walking);
        animator.SetBool("IsRunning", running);

        // pick speed
        float currentSpeed = running ? runSpeed : walkSpeed;

        // move
        Vector3 motion = dir * currentSpeed;
        motion.y = yVelocity;
        cc.stepOffset = cc.isGrounded ? originalStepOffset : 0;
        cc.Move(motion * Time.deltaTime);

        // ─── ROTATION TOWARDS MOVE DIRECTION ───────────────────
        if (dir.sqrMagnitude > 0f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        // ─── ATTACK & DEFEND BOOLEAN FLAGS ─────────────────────
        // Attack triggers one frame when clicking Fire1
        animator.SetBool("IsAttacking", Input.GetButtonDown("Fire1"));
        // Defend is true while holding Fire2
        animator.SetBool("IsDefending", Input.GetButton("Fire2"));
    }
}
