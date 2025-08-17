using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.AI;

public class PuzzleScript : MonoBehaviour
{
    [Header("UI")]
    public GameObject puzzlePanel;
    public Button castleButton;
    public static bool AutoOpenNextPuzzle = false;

    public Button HatButton;
    public Button SwordButton;

    [Header("Player / Respawn")]
    public Transform player;                 // auto-found by tag "Player"
    public Transform respawnPoint;           // drag "ResPoint" or tag "Respawn"
    [Tooltip("Auto-found; leave empty")]
    public PlayerMovementScript playerMover; // auto-found

    [Header("Behavior")]
    public bool   loadWinOnSolve = true;
    public string winSceneName   = "Win";
    public bool   resetOnSolve   = false;
    public float  teleportLockSeconds = 0.35f;

    private int currentStep = 0;
    private readonly string[] correctOrder = { "Castle", "Hat", "Sword" };

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Awake()
    {
        TryBindOnce();         // first pass
        TryBindRespawn();      // find ResPoint if not set
    }

    void Start()
    {
        if (puzzlePanel) puzzlePanel.SetActive(false);

        // Wire buttons safely
        if (castleButton) castleButton.onClick.AddListener(() => CheckStep("Castle"));
        else Debug.LogError("[Puzzle] Castle button not assigned.");
        if (HatButton) HatButton.onClick.AddListener(() => CheckStep("Hat"));
        else Debug.LogError("[Puzzle] Hat button not assigned.");
        if (SwordButton) SwordButton.onClick.AddListener(() => CheckStep("Sword"));
        else Debug.LogError("[Puzzle] Sword button not assigned.");

        // In case player spawns a frame later, keep trying briefly
        if (!player || !playerMover) StartCoroutine(TryBindLoop_Co());
        Debug.Log($"[Puzzle] Start. Player={(player ? player.name : "NULL")} Respawn={(respawnPoint ? respawnPoint.name : "NULL")}");

        if (AutoOpenNextPuzzle)
        {
            AutoOpenNextPuzzle = false;
            ShowPuzzle();            // step resets to 0 inside ShowPuzzle()
        }


    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        // Scene changed (e.g., Second → ThirdMission) → re-bind everything
        StartCoroutine(RebindAfterScene_Co());
    }

    IEnumerator RebindAfterScene_Co()
    {
        // wait a couple frames so DDOL player/camera settle
        yield return null;
        yield return null;

        TryBindRespawn();
        TryBindOnce();

        if (!player || !playerMover)
            yield return TryBindLoop_Co();

        EnsureActiveCamera();

        Debug.Log($"[Puzzle] Rebind after scene. Player={(player?player.name:"NULL")}, Mover={(playerMover?playerMover.name:"NULL")}, Respawn={(respawnPoint?respawnPoint.name:"NULL")}");
    }

    void TryBindRespawn()
    {
        if (!respawnPoint)
        {
            var byName = GameObject.Find("ResPoint");
            if (byName) respawnPoint = byName.transform;
            else
            {
                var byTag = GameObject.FindGameObjectWithTag("Respawn");
                if (byTag) respawnPoint = byTag.transform;
            }
        }
    }

    void TryBindOnce()
    {
        if (!player)
        {
            var pGo = GameObject.FindWithTag("Player");
            if (pGo) player = pGo.transform;
        }
        if (!playerMover && player)
        {
            playerMover = player.GetComponentInChildren<PlayerMovementScript>(true)
                        ?? player.GetComponent<PlayerMovementScript>();
        }
    }

    IEnumerator TryBindLoop_Co()
    {
        float end = Time.time + 3f;
        while (Time.time < end && (!player || !playerMover))
        {
            TryBindOnce();
            if (player && playerMover) break;
            yield return null;
        }
        Debug.Log($"[Puzzle] Bind result: player={(player?player.name:"NULL")}, mover={(playerMover?playerMover.name:"NULL")}");
    }

    // Ensure a camera is active (fixes “No cameras rendering”)
    void EnsureActiveCamera()
    {
        if (Camera.main != null && Camera.main.enabled) return;

        var anyCam = Object.FindAnyObjectByType<Camera>();
        if (anyCam != null) anyCam.enabled = true;

        // if still none, warn loudly
        if (Camera.main == null && anyCam == null)
            Debug.LogError("[Puzzle] No active Camera found in scene. Add/enable a Camera or keep the player camera across scenes.");
    }

    // ── Public API to open the puzzle ───────────────────────────
    public void ShowPuzzle()
    {
        TryBindOnce();
        EnsureActiveCamera();

        if (playerMover) playerMover.EnableInput(false);

        if (puzzlePanel) puzzlePanel.SetActive(true);
        currentStep = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        Debug.Log("[Puzzle] ShowPuzzle() opened panel, step=0");
    }

    public void HidePuzzle()
    {
        if (puzzlePanel) puzzlePanel.SetActive(false);
        if (playerMover) playerMover.EnableInput(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    // ── Buttons ─────────────────────────────────────────────────
    private void CheckStep(string name)
    {
        if (correctOrder[currentStep] == name)
        {
            currentStep++;
            Debug.Log($"[Puzzle] Correct '{name}'  ({currentStep}/{correctOrder.Length})");

            if (currentStep >= correctOrder.Length)
            {
                Debug.Log("[Puzzle] SOLVED!");
                if (puzzlePanel) puzzlePanel.SetActive(false);
                if (playerMover) playerMover.EnableInput(true);

                if (resetOnSolve) StartCoroutine(TeleportPlayerToRespawn_Co());

                if (loadWinOnSolve)
                {
                    if (!string.IsNullOrEmpty(winSceneName)) SceneManager.LoadScene(winSceneName);
                    else Debug.LogError("[Puzzle] winSceneName is empty.");
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
        else
        {
            Debug.Log($"[Puzzle] WRONG '{name}'. Restart via scene reload.");
            currentStep = 0;

            if (puzzlePanel) puzzlePanel.SetActive(false);

            // make sure we have a respawn
            TryBindRespawn();

            // set ticket so the reloaded scene spawns us at ResPoint and reopens the puzzle
            RespawnTicket.Set(respawnPoint, reopen: true);

            // reload current scene
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene.buildIndex);
        }


        
    }

    // ── Rock-solid teleport ─────────────────────────────────────
    private IEnumerator TeleportPlayerToRespawn_Co()
    {
        TryBindOnce();
        TryBindRespawn();

        if (!player || !respawnPoint)
        {
            Debug.LogError("[Puzzle] Teleport failed: missing player or respawnPoint.");
            yield break;
        }

        PlayerMovementScript.LockFor(teleportLockSeconds);
        if (playerMover) playerMover.EnableInput(false);

        var agents = player.GetComponentsInChildren<NavMeshAgent>(true);
        var ccs    = player.GetComponentsInChildren<CharacterController>(true);
        var rbs    = player.GetComponentsInChildren<Rigidbody>(true);

        foreach (var a in agents) if (a.enabled) a.enabled = false;
        foreach (var c in ccs)    if (c.enabled) c.enabled = false;
        foreach (var rb in rbs)   { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; rb.isKinematic = true; }

        yield return new WaitForFixedUpdate();

        bool warped = false;
        if (agents.Length > 0)
        {
            var agent = System.Array.Find(agents, a => a != null);
            if (agent != null)
            {
                player.rotation = respawnPoint.rotation;
                warped = agent.Warp(respawnPoint.position);
            }
        }
        if (!warped)
        {
            player.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);
            foreach (var c in ccs)
                c.transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);
        }

        yield return null;

        if (!warped)
        {
            player.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);
            foreach (var c in ccs)
                c.transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);
        }

        foreach (var rb in rbs) rb.isKinematic = false;
        yield return new WaitForFixedUpdate();
        foreach (var c in ccs)    c.enabled = true;
        foreach (var a in agents) a.enabled = true;

        yield return null;
        if (playerMover) playerMover.EnableInput(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        Debug.Log($"[Puzzle] Teleported to spawn @ {respawnPoint.position} | Found: agents={agents.Length}, ccs={ccs.Length}, rbs={rbs.Length}");
    }
}
