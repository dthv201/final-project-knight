using UnityEngine;
using UnityEngine.UI;

public class PuzzleScript : MonoBehaviour
{
	public GameObject puzzlePanel;
	public GameObject successMessage;
	public Button castleButton;
	public Button HatButton;
	public Button SwordButton;

	private int currentStep = 0;
	private string[] correctOrder = { "Castle", "Hat", "Sword" };

	void Start()
	{
		puzzlePanel.SetActive(false); // hide panel at start
		successMessage.SetActive(false);

		castleButton.onClick.AddListener(() => CheckStep("Castle"));
		HatButton.onClick.AddListener(() => CheckStep("Hat"));
		SwordButton.onClick.AddListener(() => CheckStep("Sword"));
	}

	public void ShowPuzzle()
	{
		puzzlePanel.SetActive(true);
		currentStep = 0;
		successMessage.SetActive(false);
	}

	void CheckStep(string name)
	{
		if (correctOrder[currentStep] == name)
		{
			currentStep++;
			if (currentStep >= correctOrder.Length)
			{
				successMessage.SetActive(true);
				Debug.Log("Puzzle Solved!");
			}
		}
		else
		{
			currentStep = 0;
			Debug.Log("Wrong choice! Restarting...");
		}
	}
}
