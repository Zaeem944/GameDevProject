using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private GameObject car;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private float activationDistance = 100f;
    [SerializeField] private float moveSpeed = 2f; // Adjust the speed as needed
    [SerializeField] private PrometeoCarController carController;

    private Renderer zombieRenderer;
    private Animator animator;
    private bool isNearCar = false;

    void Start()
    {
        zombieRenderer = GetComponentInChildren<Renderer>();
        animator = GetComponentInChildren<Animator>();

        questionPanel.SetActive(false);

        if (zombieRenderer != null)
        {
            zombieRenderer.enabled = false;
        }
        else
        {
            Debug.LogWarning("Renderer not found on zombie or its children.");
        }
    }

    void Update()
    {
        float squaredDistance = (car.transform.position - transform.position).sqrMagnitude;

        if (squaredDistance <= activationDistance * activationDistance)
        {
            if (!isNearCar)
            {
                isNearCar = true;
                zombieRenderer.enabled = true;
                Debug.Log("Zombie showed");
            }

            // Move towards the car
            Vector3 direction = (car.transform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.LookAt(car.transform.position);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == car)
        {
            // Start hitting animation
            animator.SetTrigger("Hit");

            // Optionally, disable car controls and show question panel
            questionPanel.SetActive(true);
            carController.SetCarControlsEnabled(false);
            Debug.Log("Zombie is hitting the car!");
        }
    }

    public void OnCorrectAnswer()
    {
        questionPanel.SetActive(false);
        carController.SetCarControlsEnabled(true);
        Destroy(gameObject);
        Debug.Log("Correct answer! Zombie defeated.");
    }

    public void OnIncorrectAnswer()
    {
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
