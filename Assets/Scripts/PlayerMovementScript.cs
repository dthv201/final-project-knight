using UnityEngine;
using UnityEngine.SceneManagement; // ← for LoadScene

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerMovementScript : MonoBehaviour
{
    // === Teleport / external pause support =========================
    public static float TeleportLockUntil = 0f;
    public static void LockFor(float seconds)
    {
        var until = Time.time + Mathf.Max(0f, seconds);
        if (until > TeleportLockUntil) TeleportLockUntil = until;
    }

    // === DEV CHEAT: jump to scene 3 ================================
    [Header("Dev / Cheats")]
    public bool enableCheats = true;
    [Tooltip("Hold LeftCtrl and press this number key to jump scenes")]
    public KeyCode cheatNumberKey = KeyCode.Alpha3; // Ctrl+3
    public string cheatSceneName  = "ThirdMission";

    // External input toggle (e.g., during puzzle UI)
    public bool inputEnabled = true;
    public void EnableInput(bool enabled) => inputEnabled = enabled;

    [Header("Walk/Run Speeds")]
    public float walkSpeed     = 5f;
    public float runSpeed      = 8f;
    public float rotationSpeed = 75f;

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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void Update()
    {
       // ─── CHEAT: Press P → load ThirdMission ───────────────
        if (enableCheats && Input.GetKeyDown(KeyCode.P))
        {
            TeleportLockUntil = Time.time + 0.25f; // brief lock so movers don’t fight
            SceneManager.LoadScene(cheatSceneName);
            return;
        }



        // ─── 0) Early-outs: paused by UI or teleport lock ───────
        if (!inputEnabled || Time.time < TeleportLockUntil)
        {
            yVelocity += Physics.gravity.y * Time.deltaTime;

            Vector3 lockMotion = new Vector3(0f, yVelocity, 0f);
            var lockFlags = cc.Move(lockMotion * Time.deltaTime);
            if ((lockFlags & CollisionFlags.Below) != 0 && yVelocity < 0f)
                yVelocity = -2f;

            if (animator)
            {
                animator.SetBool("IsWalking",  false);
                animator.SetBool("IsRunning",  false);
                animator.SetBool("IsBlocking", false);
            }
            return;
        }

        var   stats     = GetComponent<PlayerStats>();
        bool  pressJump = Input.GetButtonDown("Jump");
        bool  holdShift = Input.GetKey(KeyCode.LeftShift);
        bool  pressAtk  = Input.GetButtonDown("Fire1");
        bool  holdBlk   = Input.GetButton("Fire2");
        float h         = Input.GetAxisRaw("Horizontal");
        float v         = Input.GetAxisRaw("Vertical");

        // ─── 1) Jump Input & Stamina ───────────────────────────
        if (cc.isGrounded) lastGroundedTime = Time.time;
        if (pressJump)     jumpPressedTime  = Time.time;

        bool canJump =
            lastGroundedTime.HasValue &&
            Time.time - lastGroundedTime <= jumpButtonGracePeriod &&
            jumpPressedTime.HasValue &&
            Time.time - jumpPressedTime <= jumpButtonGracePeriod;

        if (canJump)
        {
            if (stats.TryUseStamina(stats.jumpStaminaCost))
            {
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
            if (stats.currentStamina >= cost) stats.currentStamina -= cost;
            else running = false;
        }
        animator.SetBool("IsWalking", walking);
        animator.SetBool("IsRunning", running);

        float speed = running ? runSpeed : walkSpeed;
        if (stats.hasSpeedBoots) speed *= 1.3f;

        Vector3 motion = dir * speed;
        motion.y = yVelocity;

        // ─── 5) Move & stick ───────────────────────────────────
        var flags = cc.Move(motion * Time.deltaTime);
        if ((flags & CollisionFlags.Below) != 0 && yVelocity < 0f)
            yVelocity = -2f;

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

        // ─── 8) Defend (hold + stamina) ────────────────────────
        if (holdBlk && stats.currentStamina >= stats.blockCost * Time.deltaTime)
        {
            stats.currentStamina -= stats.blockCost * Time.deltaTime;
            animator.SetBool("IsBlocking", true);
        }
        else
        {
            animator.SetBool("IsBlocking", false);
        }
    }
}
