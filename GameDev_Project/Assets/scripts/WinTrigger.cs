using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    [SerializeField] private string winSceneName = "LevelWon"; // Name of the winning scene to load

    private void Start()
    {
        // Ensure the collider is set as Trigger
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("WinTrigger: No Collider component found on the GameObject. Please add a Collider and set it as Trigger.");
        }
        else if (!collider.isTrigger)
        {
            Debug.LogWarning("WinTrigger: Collider is not set as Trigger. Setting it as Trigger now.");
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("WinTrigger: OnTriggerEnter triggered.");

            Debug.Log("WinTrigger: Player has entered the win trigger.");

            if (GameManager.Instance != null && GameManager.Instance.levelManager != null)
            {
                GameManager.Instance.levelManager.LoadWinScene(winSceneName);
                Debug.Log($"WinTrigger: LevelManager requested to load win scene '{winSceneName}'.");
            }
            else
            {
                if (GameManager.Instance == null)
                {
                    Debug.LogError("WinTrigger: GameManager instance is null. Cannot load win scene.");
                }
                else if (GameManager.Instance.levelManager == null)
                {
                    Debug.LogError("WinTrigger: LevelManager instance is null in GameManager. Cannot load win scene.");
                }
            }
    }
}
