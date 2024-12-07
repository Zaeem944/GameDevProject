using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Existing serialized field (optional, depending on your use case)
    [SerializeField] private string defaultSceneName;

    // Singleton instance
    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }
    }

    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            HealthManager.Instance.ResetLives();
            SceneManager.LoadScene(sceneName);
            Debug.Log($"Loading scene: {sceneName}");
        }
        else
        {
            Debug.LogWarning("Attempted to load a scene with an empty name.");
        }
    }

    /// <summary>
    /// Loads the default scene assigned in the inspector.
    /// </summary>
    public void LoadDefaultScene()
    {
        LoadScene(defaultSceneName);
    }

    /// <summary>
    /// Loads the win scene.
    /// </summary>
    /// <param name="winSceneName">The name of the win scene to load.</param>
    public void LoadWinScene(string winSceneName)
    {
        if (!string.IsNullOrEmpty(winSceneName))
        {
            // Optionally, reset lives or perform other actions
            HealthManager.Instance.ResetLives();
            SceneManager.LoadScene(winSceneName);
            Debug.Log($"Level won! Loading win scene: {winSceneName}");
        }
        else
        {
            Debug.LogWarning("Win scene name is empty.");
        }
    }
}
