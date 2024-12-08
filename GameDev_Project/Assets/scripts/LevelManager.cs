using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private string defaultSceneName; // Assign via Inspector

    // Public property to access defaultSceneName
    public string DefaultSceneName
    {
        get { return defaultSceneName; }
        private set { defaultSceneName = value; } // Optional: If you need to set it from other classes
    }

    private void Awake()
    {
        // Register with GameManager Singleton
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterLevelManager(this);
            Debug.Log($"LevelManager: Registered with GameManager: {GameManager.Instance.gameObject.name}");
        }
        else
        {
            Debug.LogError("LevelManager: GameManager Singleton instance not found.");
        }
    }

    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            if (GameManager.Instance.healthManager != null)
            {
                GameManager.Instance.healthManager.ResetLives();
                Debug.Log("LevelManager: Health lives reset.");
            }
            else
            {
                Debug.LogError("LevelManager: HealthManager not registered in GameManager.");
            }

            SceneManager.LoadScene(sceneName);
            Debug.Log($"LevelManager: Loading scene: {sceneName}");
        }
        else
        {
            Debug.LogWarning("LevelManager: Attempted to load a scene with an empty name.");
        }
    }

    public void LoadDefaultScene()
    {
        if (GameManager.Instance != null && !string.IsNullOrEmpty(DefaultSceneName))
        {
            Debug.Log($"LevelManager: Loading default scene: {DefaultSceneName}");
            LoadScene(DefaultSceneName);
        }
        else
        {
            Debug.LogWarning("LevelManager: Default scene name is not set in LevelManager or GameManager is null.");
        }
    }

    public void LoadWinScene(string winSceneName)
    {
        if (!string.IsNullOrEmpty(winSceneName))
        {
            if (GameManager.Instance.healthManager != null)
            {
                GameManager.Instance.healthManager.ResetLives();
                Debug.Log("LevelManager: Health lives reset for win scene.");
            }
            else
            {
                Debug.LogError("LevelManager: HealthManager not registered in GameManager.");
            }

            SceneManager.LoadScene(winSceneName);
            Debug.Log($"LevelManager: Level won! Loading win scene: {winSceneName}");
        }
        else
        {
            Debug.LogWarning("LevelManager: Win scene name is empty.");
        }
    }
}
