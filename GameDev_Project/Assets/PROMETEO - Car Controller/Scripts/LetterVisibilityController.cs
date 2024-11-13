using UnityEngine;
using TMPro;

public class LettersVisibilityController : MonoBehaviour
{
    [SerializeField] private GameObject car;
    [SerializeField] private GameObject[] letters;
    [SerializeField] private float detectionRange = 5.0f;
    
    [Header("Finish Line")]
    [SerializeField] private TextMeshProUGUI finishLineText;
    [SerializeField] private float finishLineShowDistance = 10f;
    [SerializeField] private ParticleSystem finishLineParticles;
    [SerializeField] private Material finishLineMaterial;
    [SerializeField] private float pulseSpeed = 1f;
    [SerializeField] private Color glowColor = Color.yellow;

    private void Start()
    {
        SetLettersActive(false);
        if (finishLineText != null)
        {
            finishLineText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        float distance = Vector3.Distance(car.transform.position, transform.position);

        // Handle regular letters visibility
        if (distance <= detectionRange)
        {
            SetLettersActive(true);
        }
        else
        {
            SetLettersActive(false);
        }

        // Handle finish line effects
        if (distance <= finishLineShowDistance)
        {
            ShowFinishLineEffects();
            UpdateFinishLineAnimation();
        }
        else
        {
            HideFinishLineEffects();
        }
    }

    private void ShowFinishLineEffects()
    {
        if (finishLineText != null)
        {
            finishLineText.gameObject.SetActive(true);
        }
        if (finishLineParticles != null && !finishLineParticles.isPlaying)
        {
            finishLineParticles.Play();
        }
    }

    private void HideFinishLineEffects()
    {
        if (finishLineText != null)
        {
            finishLineText.gameObject.SetActive(false);
        }
        if (finishLineParticles != null)
        {
            finishLineParticles.Stop();
        }
    }

    private void UpdateFinishLineAnimation()
    {
        if (finishLineText != null)
        {
            // Create pulsing effect
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            finishLineText.color = Color.Lerp(Color.white, glowColor, pulse);
            finishLineText.transform.localScale = Vector3.one * (1f + pulse * 0.1f);
        }
    }

    private void SetLettersActive(bool isActive)
    {
        foreach (GameObject letter in letters)
        {
            letter.SetActive(isActive);
        }
    }
}
