using UnityEngine;
using System.Collections;

public class PizzaCollision : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 45f;
    [SerializeField] private float verticalSpeed = 1f;
    [SerializeField] private float verticalRange = 0.5f;

    private GameManager gameManager;

    private void Start()
    {
        // Start the vertical movement coroutine
        StartCoroutine(MoveVertically());

        // Find and assign the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene. Please ensure a GameManager exists.");
        }
    }

    private void Update()
    {
        // Rotate the pizza around the Y-axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private IEnumerator MoveVertically()
    {
        float initialY = transform.position.y;
        float currentY = initialY;
        float targetY;
        float direction = 1f;

        while (true)
        {
            targetY = initialY + direction * verticalRange;
            currentY = Mathf.Lerp(currentY, targetY, verticalSpeed * Time.deltaTime);
            transform.position = new Vector3(
                transform.position.x,
                currentY,
                transform.position.z
            );

            if (Mathf.Abs(currentY - targetY) < 0.01f)
            {
                direction *= -1;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ensure that the collider is the player or relevant object
        if (other.CompareTag("Player"))
        {
            if (gameManager != null && gameManager.healthManager != null)
            {
                int currentLives = gameManager.healthManager.GetCurrentLives();
                if (currentLives < 5)
                {
                    gameManager.healthManager.GainLife();
                    Debug.Log($"Gained a life! Current Lives: {gameManager.healthManager.GetCurrentLives()}");
                }
                else
                {
                    Debug.Log("Maximum lives reached. No additional lives granted.");
                }

                // Destroy the pizza object after collision
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("GameManager or HealthManager is not assigned. Cannot gain life.");
                // Optionally, handle the case where managers are not found
                Destroy(gameObject);
            }
        }
    }
}
