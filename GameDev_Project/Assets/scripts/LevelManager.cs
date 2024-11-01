using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Public variable to store the name of the scene to load
    public string sceneName;

    // This function will be called when the button is clicked
    public void LoadSceneByName()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is empty. Please specify a scene name in the Inspector.");
        }
    }
}
