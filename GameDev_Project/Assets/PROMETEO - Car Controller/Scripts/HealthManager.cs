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

        lives = 3;
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Lives", lives);
    }

    public void LoseLife()
    {
        lives--;
        if (lives < 0) lives = 0;
        UpdateLivesDisplay();
    }

    public void GainLife()
    {
        lives++;
        if (lives > 5) lives = 5;
        UpdateLivesDisplay();
    }

    public int GetCurrentLives()
    {
        return lives;
    }

    public void ResetLives()
    {
        lives = 3;
        UpdateLivesDisplay();
    }

    private void UpdateLivesDisplay()
    {
        if (uiManager != null)
        {
            Debug.Log("Lives are:" + lives);
            uiManager.UpdateLivesDisplay();
        }
        else
        {
            Debug.LogWarning("UIManager instance is not set in HealthManager.");
        }
    }
}