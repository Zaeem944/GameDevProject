using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieController : MonoBehaviour
{
    public GameObject car;                   // Reference to the car object
    public GameObject questionPanel;         // UI panel with the math question
    public float activationDistance = 100f;  // Distance at which zombie appears (squared distance)

    private Renderer zombieRenderer;         // Renderer component of the zombie
    private bool isNearCar = false;          // To track if the car is near the zombie

    void Start()
    {
        // Use GetComponentInChildren to find the renderer on any child objects
        zombieRenderer = GetComponentInChildren<Renderer>();
        questionPanel.SetActive(false);      // Initially hide the question panel

        if (zombieRenderer != null)
        {
            zombieRenderer.enabled = false;  // Initially hide the zombie
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
                zombieRenderer.enabled = true;  // Show the zombie
                Debug.Log("Zombie showed");
            }
        }
        else
        {
            if (isNearCar)
            {
                isNearCar = false;
                zombieRenderer.enabled = false; // Hide the zombie
                Debug.Log("Zombie Gone");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
          questionPanel.SetActive(true);   // Show the question UI when car collides with zombie
          Debug.Log("Question Shown");
    }

    // Method to call when the correct answer button is pressed
    public void OnCorrectAnswer()
    {
        questionPanel.SetActive(false);   // Hide the question panel
        Destroy(gameObject);              // Destroy the zombie
        Debug.Log("Correct answer! Zombie defeated.");
    }

    // Method to call when the incorrect answer button is pressed
    public void OnIncorrectAnswer()
    {
        SceneManager.LoadScene("LevelLost");  // Load the LevelLost scene
        Debug.Log("Incorrect answer. Game over.");
    }

    //public void CloseQuestionPanel()
    //{
    //    questionPanel.SetActive(false);      // Close the question UI after answering
    //}
}
