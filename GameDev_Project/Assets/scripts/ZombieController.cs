using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private GameObject car;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private float activationDistance = 100f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private PrometeoCarController carController;
    [SerializeField] private TimerController timerController; // Reference to TimerController
    [SerializeField] private string lostSceneName;

    private Renderer zombieRenderer;
    private bool isNearCar = false;
    private Transform parentTransform;
    private bool hasCollided = false;

    private GameManager gameManager;

    void Start()
    {
        parentTransform = transform.parent;
        if (parentTransform == null)
        {
            Debug.LogError("This script must be attached to a child object with a parent!");
            enabled = false;
            return;
        }

        zombieRenderer = GetComponentInChildren<Renderer>();
        questionPanel.SetActive(false);

        if (zombieRenderer != null)
        {
            zombieRenderer.enabled = false;
        }
        else
        {
            Debug.LogWarning("Renderer not found on Zombie.");
        }

        // Subscribe to timer event
        if (timerController != null)
        {
            timerController.OnTimerEnd += HandleTimerEnd;
        }
        else
        {
            Debug.LogWarning("TimerController reference is not set.");
        }

        // Find GameManager
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
        else
        {
            Debug.Log($"GameManager found: {gameManager.gameObject.name}");
            if (gameManager.audioManager != null)
            {
                Debug.Log("AudioManager is available in GameManager.");
            }
            else
            {
                Debug.LogWarning("AudioManager is not assigned in GameManager.");
            }
        }
    }

    void Update()
    {
        if (!hasCollided)
        {
            float squaredDistance = (car.transform.position - parentTransform.position).sqrMagnitude;

            if (squaredDistance <= activationDistance)
            {
                if (!isNearCar)
                {
                    isNearCar = true;
                    zombieRenderer.enabled = true;
                    Debug.Log("Zombie showed");

                    if (gameManager != null && gameManager.audioManager != null)
                    {
                        gameManager.audioManager.PlayFourthAudio();
                    }
                    else
                    {
                        if (gameManager == null)
                        {
                            Debug.LogError("GameManager instance is null.");
                        }
                        else if (gameManager.audioManager == null)
                        {
                            Debug.LogError("AudioManager instance is null in GameManager.");
                        }
                    }
                }
                MoveTowardsCar();
            }
            else
            {
                if (isNearCar)
                {
                    isNearCar = false;
                    zombieRenderer.enabled = false;
                    Debug.Log("Zombie Gone");
                }
            }
        }
    }

    private void MoveTowardsCar()
    {
        if (!hasCollided)
        {
            Vector3 direction = (car.transform.position - parentTransform.position).normalized;
            parentTransform.position += direction * moveSpeed * Time.deltaTime;
            parentTransform.LookAt(car.transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasCollided)
        {
            if (gameManager != null && gameManager.audioManager != null)
            {
                gameManager.audioManager.PlayFirstAudio();
            }
            else
            {
                if (gameManager == null)
                {
                    Debug.LogError("GameManager instance is null.");
                }
                else if (gameManager.audioManager == null)
                {
                    Debug.LogError("AudioManager instance is null in GameManager.");
                }
            }

            hasCollided = true;
            questionPanel.SetActive(true);
            carController.SetCarControlsEnabled(false);
            Debug.Log("Question Shown");
            parentTransform.LookAt(car.transform);

            // Start the timer
            if (timerController != null)
            {
                timerController.ResetTimer(10f);
                Debug.Log("Timer started for 10 seconds.");
            }
            else
            {
                Debug.LogWarning("TimerController reference is not set.");
            }
        }
    }

    public void OnCorrectAnswer()
    {
        if (gameManager != null && gameManager.audioManager != null)
        {
            gameManager.audioManager.PlaySecondAudio();
        }
        else
        {
            if (gameManager == null)
            {
                Debug.LogError("GameManager instance is null.");
            }
            else if (gameManager.audioManager == null)
            {
                Debug.LogError("AudioManager instance is null in GameManager.");
            }
        }

        // Stop the timer only for correct answers
        if (timerController != null)
        {
            timerController.StopTimer();
            Debug.Log("Timer stopped due to correct answer.");
        }
        else
        {
            Debug.LogWarning("TimerController reference is not set.");
        }

        questionPanel.SetActive(false);
        carController.SetCarControlsEnabled(true);
        Destroy(parentTransform.gameObject);
        Debug.Log("Correct answer! Zombie defeated.");
    }

    public void OnIncorrectAnswer()
    {
        if (gameManager != null && gameManager.healthManager != null)
        {
            gameManager.healthManager.LoseLife();
            Debug.Log("Life lost due to incorrect answer.");

            if (gameManager.healthManager.GetCurrentLives() <= 0)
            {
                if (gameManager.levelManager != null)
                {
                    Debug.Log("No lives left. Loading lost scene.");
                    gameManager.levelManager.LoadScene(lostSceneName);
                }
                else
                {
                    Debug.LogError("LevelManager instance is null in GameManager.");
                }
            }
            else
            {
                Debug.Log("Incorrect answer. Lives left: " + gameManager.healthManager.GetCurrentLives());
            }
        }
        else
        {
            if (gameManager == null)
            {
                Debug.LogError("GameManager instance is null.");
            }
            else if (gameManager.healthManager == null)
            {
                Debug.LogError("HealthManager instance is null in GameManager.");
            }
        }
    }

    private void HandleTimerEnd()
    {
        if (hasCollided)
        {
            Debug.Log("Time's up! Level lost.");
            if (gameManager != null && gameManager.levelManager != null)
            {
                gameManager.levelManager.LoadScene(lostSceneName);
            }
            else
            {
                if (gameManager == null)
                {
                    Debug.LogError("GameManager instance is null.");
                }
                else if (gameManager.levelManager == null)
                {
                    Debug.LogError("LevelManager instance is null in GameManager.");
                }
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from timer event
        if (timerController != null)
        {
            timerController.OnTimerEnd -= HandleTimerEnd;
            Debug.Log("Unsubscribed from TimerController's OnTimerEnd event.");
        }
    }
}
