using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName; // Name of the scene to load
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
        else
        {
            Debug.Log($"GameManager found: {gameManager.gameObject.name}");
        }
    }

    // This method will be called by the UI Button
    public void LoadTargetScene()
    {
        if (gameManager != null)
        {
            gameManager.LoadScene(sceneName);
            Debug.Log($"Requested GameManager to load scene: {sceneName}");
        }
        else
        {
            Debug.LogError("GameManager instance is null. Cannot load scene.");
        }
    }
}
