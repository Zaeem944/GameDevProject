using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }
    private int lives = 3;
    public UIManager uiManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void LoseLife()
    {
        lives--;
        if (lives < 0) lives = 0;

        if (uiManager != null)
        {
            uiManager.UpdateLivesDisplay(); 
        }
        else
        {
            Debug.LogWarning("UIManager instance is not set in HealthManager.");
        }
    }

    public void GainLife()
    {
        lives++;
        if (lives > 5) lives = 5;

        if (uiManager != null)
        {
            uiManager.UpdateLivesDisplay();
        }
        else
        {
            Debug.LogWarning("UIManager instance is not set in HealthManager.");
        }
    }

    public int GetCurrentLives()
    {
        return lives;
    }

    public void ResetLives()
    {
        lives = 3;
    }
}
