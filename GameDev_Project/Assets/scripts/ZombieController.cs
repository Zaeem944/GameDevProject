using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private GameObject car;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private float activationDistance = 100f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private PrometeoCarController carController;
    [SerializeField] private int zombieGroanSoundIndex = 0;


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
            Debug.LogWarning("Renderer not found on Zombie 1.");
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
            // Use PlayFirstAudio instead of PlaySoundEffect
            audioManager.Instance.PlayFirstAudio();

            hasCollided = true;
            questionPanel.SetActive(true);
            carController.SetCarControlsEnabled(false);
            Debug.Log("Question Shown");
            parentTransform.LookAt(car.transform);
        }
    }

    public void OnCorrectAnswer()
    {
        // Use PlaySecondAudio instead of PlaySoundEffect
        audioManager.Instance.PlaySecondAudio();

        questionPanel.SetActive(false);
        carController.SetCarControlsEnabled(true);
        Destroy(parentTransform.gameObject);
        Debug.Log("Correct answer! Zombie defeated.");
    }

    public void OnIncorrectAnswer()
    {
        audioManager.Instance.PlayThirdAudio();
        HealthManager.Instance.LoseLife();
        if (HealthManager.Instance.GetCurrentLives() <= 0)
        {
            SceneManager.LoadScene("LevelLost");
        }
        else
        {
            Debug.Log("Incorrect answer. Lives left: " + HealthManager.Instance.GetCurrentLives());
        }
    }
}