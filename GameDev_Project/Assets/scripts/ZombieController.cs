using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private GameObject car;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private float activationDistance = 100f;
    [SerializeField] private float moveSpeed = 2f;  // Speed at which the zombie moves toward the car
    [SerializeField] private PrometeoCarController carController;

    private Renderer zombieRenderer;
    private bool isNearCar = false;

    void Start()
    {
        zombieRenderer = GetComponentInChildren<Renderer>();
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

        if (squaredDistance <= activationDistance)
        {
            if (!isNearCar)
            {
                isNearCar = true;
                zombieRenderer.enabled = true;
                Debug.Log("Zombie showed");
            }

            // Move the zombie towards the car when it's close enough
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
        // Move the zombie slowly toward the car
        Vector3 direction = (car.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
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
