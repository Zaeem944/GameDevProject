using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }
    private int lives = 3;

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
        if (lives > 0)
        {
            lives--;
            Debug.Log("Lifes left: "+lives);
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
