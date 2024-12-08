using UnityEngine;
using System.Collections;

public class MineCollision : MonoBehaviour
{
    [Header("Mine Visual Settings")]
    [SerializeField] private float rotationSpeed = 45f;
    [SerializeField] private float verticalSpeed = 1f;
    [SerializeField] private float verticalRange = 0.5f;

    [Header("Scene Settings")]
    [SerializeField] private string lostSceneName;


    [Header("Explosion Settings")]
    [SerializeField] private GameObject explosionPrefab;

    private GameManager gameManager;

    private void Start()
    {
        StartCoroutine(MoveVertically());

        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("MineCollision: GameManager instance not found. Please ensure GameManager is present and follows the Singleton pattern.");
        }
    }

    private void Update()
    {
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
                direction *= -1f;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("MineCollision: Player collided with mine.");
        if (explosionPrefab != null)
        {
            // Store the instantiated explosion in a variable and detach it.
            GameObject explosionObj = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosionObj.transform.SetParent(null);

            Debug.Log("MineCollision: Explosion effect instantiated.");
        }
        else
        {
            Debug.LogWarning("MineCollision: Explosion prefab is not assigned.");
        }

        if (gameManager != null && gameManager.healthManager != null)
        {
            gameManager.healthManager.LoseLife();
            int currentLives = gameManager.healthManager.GetCurrentLives();
            Debug.Log($"MineCollision: Life lost! Current Lives: {currentLives}");

            if (gameManager.audioManager != null)
            {
                gameManager.audioManager.PlayFifthAudio();
                Debug.Log("MineCollision: Played fifth audio (Mine collision sound).");
            }
            else
            {
                Debug.LogError("MineCollision: AudioManager instance is null in GameManager. Cannot play collision sound.");
            }

            if (currentLives <= 0)
            {
                if (gameManager.levelManager != null)
                {
                    Debug.Log("MineCollision: No lives left. Loading lost scene.");
                    gameManager.levelManager.LoadScene(lostSceneName);
                }
                else
                {
                    Debug.LogError("MineCollision: LevelManager instance is null in GameManager. Cannot load lost scene.");
                }
            }
        }
        else
        {
            if (gameManager == null)
            {
                Debug.LogError("MineCollision: GameManager instance is null. Cannot lose life.");
            }
            else if (gameManager.healthManager == null)
            {
                Debug.LogError("MineCollision: HealthManager instance is null in GameManager. Cannot lose life.");
            }
        }

        Destroy(gameObject);
    }
}
