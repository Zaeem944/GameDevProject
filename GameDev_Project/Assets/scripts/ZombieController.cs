using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private GameObject car;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private float activationDistance = 100f;
    [SerializeField] private float moveSpeed = 2f;  // Speed at which the zombie moves toward the car
    [SerializeField] private PrometeoCarController carController;
    [SerializeField] private Animator zombieAnimator; // Reference to the animator
    [SerializeField] private float rotationSpeed = 5f; // Speed at which zombie rotates to face car

    private Renderer zombieRenderer;
    private bool isNearCar = false;

    void Start()
    {
        // Assuming the Renderer is on the parent object (Zombie 1)
        zombieRenderer = GetComponent<Renderer>();
        ]
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
        float squaredDistance = (car.transform.position - transform.position).sqrMagnitude;

        if (squaredDistance <= activationDistance)
        {
            if (!isNearCar)
            {
                isNearCar = true;
                zombieRenderer.enabled = true;
                Debug.Log("Zombie showed");
            }

            // Move the root zombie object (Zombie 1) towards the car
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

    private void MoveTowardsCar()
    {
        // Calculate direction to car
        Vector3 direction = (car.transform.position - transform.position).normalized;
        
        // Rotate zombie to face the car
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
        // Move zombie forward
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        
        // Set animation parameter if we have an animator
        if (zombieAnimator != null)
        {
            zombieAnimator.SetBool("IsWalking", true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject == car)
        //{
        questionPanel.SetActive(true);
        carController.SetCarControlsEnabled(false);
        Debug.Log("Question Shown");
        //}
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
