using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private GameObject car;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private float activationDistance = 100f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private PrometeoCarController carController;

    private Renderer zombieRenderer;
    private bool isNearCar = false;
    private Transform parentTransform;

    void Start()
    {
        // Get the parent transform (Zombie 1)
        parentTransform = transform.parent;
        if (parentTransform == null)
        {
            Debug.LogError("This script must be attached to a child object with a parent!");
            enabled = false;
            return;
        }

        // Get the renderer from the appropriate object in the hierarchy
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
        // Use parent's position for distance calculation
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

    private void MoveTowardsCar()
    {
        // Move the parent object instead of this object
        Vector3 direction = (car.transform.position - parentTransform.position).normalized;
        parentTransform.position += direction * moveSpeed * Time.deltaTime;

        // Optional: Make the zombie face the car
        parentTransform.LookAt(car.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        questionPanel.SetActive(true);
        carController.SetCarControlsEnabled(false);
        Debug.Log("Question Shown");
    }

    public void OnCorrectAnswer()
    {
        questionPanel.SetActive(false);
        carController.SetCarControlsEnabled(true);
        // Destroy the parent object instead of just this object
        Destroy(parentTransform.gameObject);
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