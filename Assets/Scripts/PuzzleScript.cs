// File: Assets/Scripts/PuzzleScript.cs
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

    [Header("Player / Respawn (no longer used for wrong)")]
    public Transform player;
    public Transform respawnPoint;
    [Tooltip("Auto-found; leave empty")]
    public PlayerMovementScript playerMover;

    [Header("Behavior")]
    public bool   loadWinOnSolve = true;
    public string winSceneName   = "Win";
    public string loseSceneName  = "TryAgain";   // 👈 new lose scene
    public bool   resetOnSolve   = false;
    public float  teleportLockSeconds = 0.35f;

    private int currentStep = 0;
    private readonly string[] correctOrder = { "Castle", "Hat", "Sword" };

    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        if (puzzlePanel) puzzlePanel.SetActive(false);

        if (castleButton) castleButton.onClick.AddListener(() => CheckStep("Castle"));
        if (HatButton)    HatButton.onClick.AddListener(() => CheckStep("Hat"));
        if (SwordButton)  SwordButton.onClick.AddListener(() => CheckStep("Sword"));

        Debug.Log("[Puzzle] Ready.");
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m) { /* keep if needed */ }

    // ── Open / Close Puzzle ──────────────────────────────────────
    public void ShowPuzzle()
    {
        if (playerMover) playerMover.EnableInput(false);
        if (puzzlePanel) puzzlePanel.SetActive(true);
        currentStep = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public void HidePuzzle()
    {
        if (puzzlePanel) puzzlePanel.SetActive(false);
        if (playerMover) playerMover.EnableInput(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    // ── Button logic ─────────────────────────────────────────────
    private void CheckStep(string name)
    {
        if (correctOrder[currentStep] == name)
        {
            currentStep++;
            Debug.Log($"[Puzzle] Correct '{name}' ({currentStep}/{correctOrder.Length})");

            if (currentStep >= correctOrder.Length)
            {
                Debug.Log("[Puzzle] SOLVED!");
                if (puzzlePanel) puzzlePanel.SetActive(false);
                if (playerMover) playerMover.EnableInput(true);

                if (loadWinOnSolve && !string.IsNullOrEmpty(winSceneName))
                    SceneManager.LoadScene(winSceneName);
            }
        }
        else
        {
            Debug.Log($"[Puzzle] WRONG '{name}' → go to Lose scene");
            if (puzzlePanel) puzzlePanel.SetActive(false);

            // 👇 Jump straight to Lose screen
            if (!string.IsNullOrEmpty(loseSceneName))
                SceneManager.LoadScene(loseSceneName);
            else
                Debug.LogError("[Puzzle] loseSceneName is empty.");
        }
    }
}
