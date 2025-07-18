using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPatrolAndChase : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] waypoints;
    public float waitTimeAtPoint = 1f;

    [Header("Detection / Chase")]
    public float detectionRadius = 15f;
    [Range(0,180)] public float viewAngle = 90f;   // half-angle, so 90 => 180° cone
    public float giveUpAfter = 4f;                 // seconds out of sight before patrol resumes
    public float attackRadius = 2f;

    NavMeshAgent agent;
    Animator anim;
    Transform player;
    int curIndex;
    float lastTimeSeen;

    void Awake()
    {
        agent  = GetComponent<NavMeshAgent>();
        anim   = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        anim.applyRootMotion = false;
    }

    void Start() => StartCoroutine(PatrolLoop());

    IEnumerator PatrolLoop()
    {
        while (true)
        {
            // 1. If player detected -> chase
            if (PlayerVisible())
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                lastTimeSeen = Time.time;

                // attack when close enough
                if (agent.remainingDistance <= attackRadius)
                {
                    agent.isStopped = true;
                    anim.SetTrigger("Attack");
                }
            }
            // 2. If chasing but player lost -> keep chasing a bit, then return
            else if (Time.time - lastTimeSeen < giveUpAfter)
            {
                // keep last chase destination; nothing to do
            }
            // 3. Patrol behaviour
            else
            {
                // reached current waypoint?
                if (!agent.pathPending && agent.remainingDistance < 0.2f)
                {
                    yield return new WaitForSeconds(waitTimeAtPoint);
                    curIndex = (curIndex + 1) % waypoints.Length;
                    agent.SetDestination(waypoints[curIndex].position);
                }
            }

            // animator drive
            anim.SetFloat("Speed", agent.velocity.magnitude);
            yield return null;
        }
    }

    bool PlayerVisible()
    {
        if (player == null) return false;

        Vector3 dir = player.position - transform.position;
        if (dir.magnitude > detectionRadius) return false;

        // FOV check
        if (Vector3.Angle(transform.forward, dir) > viewAngle) return false;

        // line-of-sight (optional): raycast
        if (Physics.Raycast(transform.position + Vector3.up * 1.2f, dir.normalized,
                            out RaycastHit hit, detectionRadius))
        {
            if (!hit.transform.CompareTag("Player")) return false;
        }
        return true;
    }
}
