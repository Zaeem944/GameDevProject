using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI livesText;

    private void Awake()
    {
        // Register this UIManager instance with the HealthManager
        if (HealthManager.Instance != null)
        {
            HealthManager.Instance.uiManager = this;
        }
        else
        {
            Debug.LogWarning("HealthManager instance is not found.");
        }
    }

    private void Start()
    {
        UpdateLivesDisplay();
    }

    public void UpdateLivesDisplay()
    {
        if (HealthManager.Instance != null)
        {
            livesText.text = "Lives: " + HealthManager.Instance.GetCurrentLives();
        }
        else
        {
            Debug.LogWarning("HealthManager instance is null.");
        }
    }
}
