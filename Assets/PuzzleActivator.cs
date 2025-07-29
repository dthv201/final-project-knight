using UnityEngine;

public class PuzzleActivator : MonoBehaviour
{
	public GameObject puzzleManager;  // Assign this via Inspector

	public void OpenPuzzle()
	{
		puzzleManager.SetActive(true);
	}
}
