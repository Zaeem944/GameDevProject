using UnityEngine;

public class LettersVisibilityController : MonoBehaviour
{
    [SerializeField] private GameObject car; // Reference to the car object
    [SerializeField] private GameObject[] letters; // Array of letter objects to control visibility
    [SerializeField] private float detectionRange = 5.0f; // Range within which letters will appear

    private void Start()
    {
        // Hide all letters initially
        SetLettersActive(false);
    }

    private void Update()
    {
        // Calculate distance between the car and each letter object
        float distance = Vector3.Distance(car.transform.position, transform.position);

        // If the car is within range, show the letters; otherwise, hide them
        if (distance <= detectionRange)
        {
            SetLettersActive(true);
        }
        else
        {
            SetLettersActive(false);
        }
    }

    // Method to set all letters active or inactive
    private void SetLettersActive(bool isActive)
    {
        foreach (GameObject letter in letters)
        {
            letter.SetActive(isActive);
        }
    }
}
