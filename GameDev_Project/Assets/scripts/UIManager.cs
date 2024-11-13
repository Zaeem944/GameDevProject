using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI livesText;  

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
