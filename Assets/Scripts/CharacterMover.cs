using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMover : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    [Header("Detection Settings")]
    public float rayLength    = 20f;      // how far the dragon “sees”
    public LayerMask playerLayer;         // assign to your “Player” layer
    public Transform player;              // drag your Player transform here

    [Header("Penalty Settings")]
    public float spotDamage   = 50f;      // HP to lose when seen
    public Transform savePoint;           // where to teleport when seen

    private Vector3 target;
    private Animator animator;
    private CharacterController controller;
    private bool isTurning = false;
    private bool isWalking = true;

    void Start()
    {
        animator   = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        target     = pointB.position;
    }

    void Update()
    {
        if (isTurning) return;

        PatrolMovement();
        CheckSightAndPenalize();
    }

    void PatrolMovement()
    {
        Vector3 dir = target - transform.position;
        dir.y = 0f;

        if (dir.magnitude > 0.5f)
        {
            isWalking = true;
            controller.Move(dir.normalized * speed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 5f
            );
            if (animator) animator.SetFloat("Speed", speed);
        }
        else if (isWalking)
        {
            isWalking = false;
            if (animator)
            {
                animator.SetFloat("Speed", 0f);
                animator.SetTrigger("Turn");
            }
            StartCoroutine(HandleTurnAndSwitchTarget());
        }
    }

    IEnumerator HandleTurnAndSwitchTarget()
    {
        isTurning = true;
        yield return new WaitForSeconds(1f);
        target = (target == pointA.position) ? pointB.position : pointA.position;
        isTurning = false;
    }

    void CheckSightAndPenalize()
    {
        Vector3 origin  = transform.position + Vector3.up * 0.75f;
        Vector3 forward = transform.forward;
        Debug.DrawRay(origin, forward * rayLength, Color.red);

        if (Physics.Raycast(origin, forward, out RaycastHit hit, rayLength, playerLayer))
        {
            // Did we hit the player?
            var stats = hit.transform.GetComponentInParent<PlayerStats>();
            if (stats != null)
            {
                // 1) Deal 50 damage
                stats.TakeDamage(spotDamage);
                Debug.Log($"[Dragon] Spotted player! -{spotDamage} HP (now {stats.currentHealth})");

                // 2) Teleport back to save point
                TeleportPlayerToSave();

                // 3) Prevent multiple penalties until next frame
                //    (so they don't lose all 50*frames HP instantly)
                //    simply disable further checks this update
                return;
            }
        }
    }

    void TeleportPlayerToSave()
    {
        if (savePoint == null || player == null)
        {
            Debug.LogWarning("SavePoint or Player not assigned!");
            return;
        }

        // Temporarily disable CharacterController if present
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.position = savePoint.position;

        if (cc != null) cc.enabled = true;
    }
}
