using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int lives = 3;
    public UIManager uiManager; // Ensure this is assigned via Inspector

    private void Awake()
    {
        // Register with GameManager Singleton
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterHealthManager(this);
            Debug.Log($"HealthManager: Registered with GameManager: {GameManager.Instance.gameObject.name}");
        }
        else
        {
            Debug.LogError("HealthManager: GameManager Singleton instance not found.");
        }

        UpdateLivesDisplay();
    }

    public void LoseLife()
    {
        lives--;
        if (lives < 0) lives = 0;
        UpdateLivesDisplay();
        Debug.Log($"HealthManager: Life lost. Current lives: {lives}");
    }

    public void GainLife()
    {
        lives++;
        if (lives > 5) lives = 5;
        UpdateLivesDisplay();
        Debug.Log($"HealthManager: Life gained. Current lives: {lives}");
    }

    public int GetCurrentLives()
    {
        return lives;
    }

    public void ResetLives()
    {
        lives = 3;
        UpdateLivesDisplay();
        Debug.Log("HealthManager: Lives reset to 3.");
    }

    private void UpdateLivesDisplay()
    {
        if (uiManager != null)
        {
            uiManager.UpdateLivesDisplay();
        }
        else
        {
            Debug.LogWarning("HealthManager: UIManager instance is not set.");
        }
    }
}
