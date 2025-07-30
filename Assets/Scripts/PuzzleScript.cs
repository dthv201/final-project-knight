using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PuzzleScript : MonoBehaviour
{
    public GameObject puzzlePanel;
    public Button castleButton;
    public Button HatButton;
    public Button SwordButton;

    private int currentStep = 0;
    private string[] correctOrder = { "Castle", "Hat", "Sword" };

    void Start()
    {
        puzzlePanel.SetActive(false); // hide panel at start

        castleButton.onClick.AddListener(() => CheckStep("Castle"));
        HatButton.onClick.AddListener(() => CheckStep("Hat"));
        SwordButton.onClick.AddListener(() => CheckStep("Sword"));
    }

    public void ShowPuzzle()
    {
        puzzlePanel.SetActive(true);
        currentStep = 0;
    }

    void CheckStep(string name)
    {
        if (correctOrder[currentStep] == name)
        {
            currentStep++;
            if (currentStep >= correctOrder.Length)
            {
                Debug.Log("Puzzle Solved!");
                SceneManager.LoadScene("Win"); // Load Win scene
            }
        }
        else
        {
            currentStep = 0;
            Debug.Log("Wrong choice! Restarting...");
            SceneManager.LoadScene("TryAgain"); // Load TryAgain scene
        }
    }
}