using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    // Called when "Yes" button is clicked
    public void OnYesClicked()
    {
        SceneManager.LoadScene("Forest"); // Replace with your actual starting scene name
    }

    // Called when "No" button is clicked
    public void OnNoClicked()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}