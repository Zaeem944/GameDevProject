using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieController : MonoBehaviour
{
    [Header("Core References")]
    [SerializeField] private GameObject car;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private PrometeoCarController carController;
    [SerializeField] private Animator zombieAnimator;
    
    [Header("Movement Settings")]
    [SerializeField] private float activationDistance = 100f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float stoppingDistance = 2f;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private Material dissolveMaterial;
    [SerializeField] private float dissolveTime = 2f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip attackSound;
    
    [Header("Difficulty Settings")]
    [SerializeField] private float healthPoints = 100f;
    [SerializeField] private float damageAmount = 20f;
    [SerializeField] private float speedIncreasePerLevel = 0.5f;
    
    private Renderer zombieRenderer;
    private bool isNearCar = false;
    private ZombieState currentState = ZombieState.Idle;
    private float dissolveAmount = 0f;
    private static int zombiesDefeated = 0;

    private enum ZombieState
    {
        Idle,
        Pursuing,
        Attacking,
        Stunned,
        Dying
    }

    void Start()
    {
        InitializeZombie();
        AdjustDifficultyBasedOnProgress();
    }

    private void InitializeZombie()
    {
        zombieRenderer = GetComponent<Renderer>();
        questionPanel.SetActive(false);
        zombieRenderer.enabled = false;
        
        if (audioSource && spawnSound)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }

    private void AdjustDifficultyBasedOnProgress()
    {
        moveSpeed += speedIncreasePerLevel * (zombiesDefeated / 5);
        healthPoints += (zombiesDefeated / 3) * 20f;
    }

    void Update()
    {
        UpdateZombieState();
        UpdateVisualEffects();
    }

    private void UpdateZombieState()
    {
        float distanceToCar = Vector3.Distance(car.transform.position, transform.position);

        switch (currentState)
        {
            case ZombieState.Idle:
                if (distanceToCar <= activationDistance)
                {
                    TransitionToState(ZombieState.Pursuing);
                }
                break;

            case ZombieState.Pursuing:
                if (distanceToCar <= stoppingDistance)
                {
                    TransitionToState(ZombieState.Attacking);
                }
                else
                {
                    MoveTowardsCar();
                }
                break;

            case ZombieState.Attacking:
                if (distanceToCar > stoppingDistance)
                {
                    TransitionToState(ZombieState.Pursuing);
                }
                PerformAttack();
                break;
        }
    }

    private void TransitionToState(ZombieState newState)
    {
        currentState = newState;
        
        switch (newState)
        {
            case ZombieState.Pursuing:
                zombieAnimator?.SetBool("IsWalking", true);
                break;
            case ZombieState.Attacking:
                zombieAnimator?.SetTrigger("Attack");
                if (audioSource && attackSound)
                {
                    audioSource.PlayOneShot(attackSound);
                }
                break;
            case ZombieState.Dying:
                StartDeath();
                break;
        }
    }

    private void StartDeath()
    {
        if (deathEffect)
        {
            deathEffect.Play();
        }
        if (audioSource && deathSound)
        {
            audioSource.PlayOneShot(deathSound);
        }
        zombieAnimator?.SetTrigger("Death");
        zombiesDefeated++;
        Invoke("OnDeathComplete", dissolveTime);
    }

    private void OnDeathComplete()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        healthPoints -= damage;
        if (hitEffect)
        {
            hitEffect.Play();
        }
        
        if (healthPoints <= 0)
        {
            TransitionToState(ZombieState.Dying);
        }
        else
        {
            StartCoroutine(FlashDamage());
        }
    }

    private System.Collections.IEnumerator FlashDamage()
    {
        if (zombieRenderer.material.HasProperty("_FlashAmount"))
        {
            zombieRenderer.material.SetFloat("_FlashAmount", 1f);
            yield return new WaitForSeconds(0.1f);
            zombieRenderer.material.SetFloat("_FlashAmount", 0f);
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
        questionPanel.SetActive(true);
        carController.SetCarControlsEnabled(false);
        Debug.Log("Question Shown");
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
