// PlayerMovementScript.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerMovementScript : MonoBehaviour
{
    [Header("Walk/Run Speeds")]
    public float walkSpeed     = 5f;
    public float runSpeed      = 8f;
    public float rotationSpeed = 540f;

    [Header("Jump Settings")]
    public float jumpSpeed            = 5f;
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
        // ─── 1) Ground-buffered jump input ───────────────────────────────
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

        // ─── 2) Apply gravity every frame ───────────────────────────────
        yVelocity += Physics.gravity.y * Time.deltaTime;

        // ─── 3) Handle horizontal input & camera-relative motion ────────
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Transform cam = Camera.main.transform;
        Vector3 camF = cam.forward; camF.y = 0; camF.Normalize();
        Vector3 camR = cam.right;   camR.y = 0; camR.Normalize();

        Vector3 dir = (camF * v + camR * h).normalized;

        bool walking = dir.sqrMagnitude > 0f;
        bool running = walking && Input.GetKey(KeyCode.LeftShift);
        animator.SetBool("IsWalking", walking);
        animator.SetBool("IsRunning", running);

        float currentSpeed = running ? runSpeed : walkSpeed;
        Vector3 motion    = dir * currentSpeed;
        motion.y          = yVelocity;

        // ─── 4) Move & detect ground collision ─────────────────────────
        CollisionFlags flags = cc.Move(motion * Time.deltaTime);

        if ((flags & CollisionFlags.Below) != 0 && yVelocity < 0f)
        {
            // you’ve just landed — clamp and stick to ground
            yVelocity = -2f;
            animator.SetBool("IsJumping", false);
        }

        // ─── 5) Rotate toward move direction ──────────────────────────
        if (dir.sqrMagnitude > 0f)
        {
            Quaternion tgt = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                tgt,
                rotationSpeed * Time.deltaTime
            );
        }

        // ─── 6) Combat flags ──────────────────────────────────────────
        animator.SetBool("IsAttacking", Input.GetButtonDown("Fire1"));
        animator.SetBool("IsDefending", Input.GetButton("Fire2"));
    }
}
