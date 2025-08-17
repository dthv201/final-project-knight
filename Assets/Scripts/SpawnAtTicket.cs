// File: Assets/Scripts/SpawnAtTicket.cs
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;

[DefaultExecutionOrder(-1000)]
public class SpawnAtTicket : MonoBehaviour
{
    void OnEnable()  { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    IEnumerator Start()
    {
        // Cover the very first scene load (before any reloads)
        yield return ApplyTicketIfAny_Co("[Start]");
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        // Handle every reload (works even if this GameObject is DontDestroyOnLoad)
        StartCoroutine(ApplyTicketIfAny_Co($"[sceneLoaded:{s.name}]"));
    }

    private IEnumerator ApplyTicketIfAny_Co(string src)
    {
        if (!RespawnTicket.Consume(out var p, out var q, out var reopen)) yield break;

        Debug.Log($"[SpawnAtTicket]{src} Applying ticket → pos={p} rot={q.eulerAngles} reopen={reopen}");

        var agent = GetComponentInChildren<NavMeshAgent>(true);
        var cc    = GetComponentInChildren<CharacterController>(true);
        var rbs   = GetComponentsInChildren<Rigidbody>(true);

        // quiet physics/controllers
        if (agent) agent.enabled = false;
        if (cc)    cc.enabled    = false;
        foreach (var rb in rbs) { rb.isKinematic = true; rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }

        // snap
        transform.SetPositionAndRotation(p, q);
        Physics.SyncTransforms();
        yield return new WaitForFixedUpdate();

        // navmesh-friendly warp (if agent exists and navmesh is present)
        if (agent)
        {
            agent.enabled = true;
            agent.Warp(p);          // succeeds only on baked NavMesh
            transform.rotation = q; // ensure final rotation
        }

        // re-enable
        if (cc) cc.enabled = true;
        foreach (var rb in rbs) rb.isKinematic = false;

        // trigger auto-open for the puzzle after reload
        PuzzleScript.AutoOpenNextPuzzle = reopen;
    }
}
