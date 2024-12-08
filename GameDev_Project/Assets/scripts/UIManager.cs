using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI livesText;

    private void Awake()
    {
        // Register this UIManager instance with the HealthManager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null && gameManager.healthManager != null)
        {
            gameManager.healthManager.uiManager = this;
            Debug.Log("UIManager registered with HealthManager.");
        }
        else
        {
            Debug.LogWarning("GameManager or HealthManager instance is not found. UIManager cannot register.");
        }
    }

    private void Start()
    {
        UpdateLivesDisplay();
    }

    public void UpdateLivesDisplay()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null && gameManager.healthManager != null)
        {
            livesText.text = "Lives: " + gameManager.healthManager.GetCurrentLives();
        }
        else
        {
            Debug.LogWarning("HealthManager instance is null.");
        }
    }
}
