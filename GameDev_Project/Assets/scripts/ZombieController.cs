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

    private Renderer zombieRenderer;
    private bool isNearCar = false;
    private Transform parentTransform;
    private bool hasCollided = false;

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
                    audioManager.Instance.PlayFourthAudio();
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
            audioManager.Instance.PlayFirstAudio();

            hasCollided = true;
            questionPanel.SetActive(true);
            carController.SetCarControlsEnabled(false);
            Debug.Log("Question Shown");
            parentTransform.LookAt(car.transform);

            // Start the timer
            if (timerController != null)
            {
                timerController.ResetTimer(10f);
            }
        }
    }

    public void OnCorrectAnswer()
    {
        audioManager.Instance.PlaySecondAudio();

        // Stop the timer only for correct answers
        if (timerController != null)
        {
            timerController.StopTimer();
        }

        questionPanel.SetActive(false);
        carController.SetCarControlsEnabled(true);
        Destroy(parentTransform.gameObject);
        Debug.Log("Correct answer! Zombie defeated.");
    }

    public void OnIncorrectAnswer()
    {
        audioManager.Instance.PlayThirdAudio();
        HealthManager.Instance.LoseLife();

        // No stopping of the timer here
        if (HealthManager.Instance.GetCurrentLives() <= 0)
        {
            SceneManager.LoadScene("LevelLost");
        }
        else
        {
            Debug.Log("Incorrect answer. Lives left: " + HealthManager.Instance.GetCurrentLives());
        }
    }

    private void HandleTimerEnd()
    {
        if (hasCollided)
        {
            Debug.Log("Time's up! Level lost.");
            SceneManager.LoadScene("LevelLost");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from timer event
        if (timerController != null)
        {
            timerController.OnTimerEnd -= HandleTimerEnd;
        }
    }
}
