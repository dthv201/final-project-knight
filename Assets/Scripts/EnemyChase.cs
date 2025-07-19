using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChase : MonoBehaviour
{
    public float detectionRadius = 15f;
    public float attackRadius    = 2f;

    Transform     target;
    NavMeshAgent  agent;
    Animator      anim;

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent  = GetComponent<NavMeshAgent>();
        anim   = GetComponent<Animator>();

        // Root-motion OFF so the agent drives the movement
        anim.applyRootMotion = false;
    }

    void Update()
    {
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        // —-- Path & movement --—
        if (dist <= detectionRadius)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.isStopped = true;
        }

        // —-- Animation param --—
        anim.SetFloat("Speed", agent.velocity.magnitude);   // <-- drives the blend tree

        if (dist <= attackRadius)
        {
            agent.isStopped = true;
            anim.SetTrigger("Attack");                      // uses your attack state
        }
    }
}
