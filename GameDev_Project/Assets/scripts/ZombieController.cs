using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieController : MonoBehaviour
{
    public GameObject car;
    public GameObject questionPanel;
    public float activationDistance = 100f;
    public MonoBehaviour carMovementScript;

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
        questionPanel.SetActive(true);
        carMovementScript.enabled = false;
        Debug.Log("Question Shown");
    }

    public void OnCorrectAnswer()
    {
        questionPanel.SetActive(false);
        carMovementScript.enabled = true;
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
