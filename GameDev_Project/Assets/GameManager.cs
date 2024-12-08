using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // References to other managers
    public HealthManager healthManager;
    public LevelManager levelManager;
    public AudioManager audioManager;

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            Debug.LogWarning("GameManager: Duplicate instance detected and destroyed.");
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("GameManager: Initialized and set to DontDestroyOnLoad.");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Registration methods called by managers
    public void RegisterLevelManager(LevelManager lm)
    {
        if (lm == null)
        {
            Debug.LogError("GameManager: Attempted to register a null LevelManager.");
            return;
        }

        levelManager = lm;
        Debug.Log($"GameManager: LevelManager registered: {lm.gameObject.name}");
    }

    public void RegisterAudioManager(AudioManager am)
    {
        if (am == null)
        {
            Debug.LogError("GameManager: Attempted to register a null AudioManager.");
            return;
        }

        audioManager = am;
        Debug.Log($"GameManager: AudioManager registered: {am.gameObject.name}");
    }

    public void RegisterHealthManager(HealthManager hm)
    {
        if (hm == null)
        {
            Debug.LogError("GameManager: Attempted to register a null HealthManager.");
            return;
        }

        healthManager = hm;
        Debug.Log($"GameManager: HealthManager registered: {hm.gameObject.name}");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GameManager: Scene Loaded: {scene.name}");
    }

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("GameManager: Attempted to load a scene with an empty name.");
            return;
        }

        if (healthManager != null)
        {
            healthManager.ResetLives();
            Debug.Log("GameManager: Health lives reset.");
        }
        else
        {
            Debug.LogError("GameManager: HealthManager not registered. Cannot reset lives.");
        }

        SceneManager.LoadScene(sceneName);
        Debug.Log($"GameManager: Loading scene: {sceneName}");
    }

    public void LoadWinScene(string winSceneName)
    {
        if (string.IsNullOrEmpty(winSceneName))
        {
            Debug.LogWarning("GameManager: Win scene name is empty.");
            return;
        }

        if (healthManager != null)
        {
            healthManager.ResetLives();
            Debug.Log("GameManager: Health lives reset for win scene.");
        }
        else
        {
            Debug.LogError("GameManager: HealthManager not registered. Cannot reset lives for win scene.");
        }

        SceneManager.LoadScene(winSceneName);
        Debug.Log($"GameManager: Level won! Loading win scene: {winSceneName}");
    }

    public void LoadDefaultScene()
    {
        if (levelManager != null && !string.IsNullOrEmpty(levelManager.DefaultSceneName))
        {
            Debug.Log($"GameManager: Loading default scene: {levelManager.DefaultSceneName}");
            LoadScene(levelManager.DefaultSceneName);
        }
        else
        {
            Debug.LogWarning("GameManager: Default scene name is not set in LevelManager or LevelManager is null.");
        }
    }
    public void PlayBackgroundMusic()
    {
        if (audioManager != null)
        {
            audioManager.PlaySixthAudio();
        }
        else
        {
            Debug.LogError("GameManager: AudioManager is not registered. Cannot play background music.");
        }
    }
}
