using UnityEngine;
using UnityEngine.SceneManagement;

public class TryAgain: MonoBehaviour
{
    // Called when "Yes" button is clicked
    public void OnYesClicked()
    {
        SceneManager.LoadScene("Forest"); // Replace with your actual puzzle scene name
    }

    // Called when "No" button is clicked
    public void OnNoClicked()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in editor
#endif
    }
}
