using UnityEngine;
using UnityEngine.SceneManagement;

public class Strarting : MonoBehaviour
{
	void Start()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None; // Unlocks the mouse
	}
	// Set this to the name of your main scene
	public string mainSceneName = "MainScene"; // <-- Replace with your scene's actual name

	// This function will be called when the button is clicked
	public void OnStartClicked()
	{
		SceneManager.LoadScene(mainSceneName);
	}


}
