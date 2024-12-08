using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarEngineSound : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Transform of the car (with a Rigidbody).")]
    public Transform carTransform;

    [Header("Speed Settings")]
    [Tooltip("The maximum speed at which the car would reach max pitch/volume.")]
    public float maxSpeed = 100f;

    [Header("Audio Adjustments")]
    [Tooltip("Minimum pitch at zero speed.")]
    public float minPitch = 0.5f;
    [Tooltip("Maximum pitch at max speed.")]
    public float maxPitch = 2f;

    [Tooltip("Minimum volume at zero speed.")]
    public float minVolume = 0.2f;
    [Tooltip("Maximum volume at max speed.")]
    public float maxVolume = 1.0f;

    private AudioSource engineAudio;
    private Rigidbody carRigidbody;

    void Start()
    {
        // Get the AudioSource component from the current GameObject
        engineAudio = GetComponent<AudioSource>();

        // Ensure we have a valid car Transform, and attempt to get its Rigidbody
        if (carTransform != null)
        {
            carRigidbody = carTransform.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogWarning("Car Transform not assigned. Please assign the car Transform in the inspector.");
        }

        // For best results, ensure your audio clip is set to loop in the AudioSource component
    }

    void Update()
    {
        // If we have a car with a Rigidbody, measure its speed
        float speed = 0f;
        if (carRigidbody != null)
        {
            // speed in Unity units (m/s). Convert as needed if you have a different measure.
            speed = carRigidbody.velocity.magnitude;
        }

        // Normalize speed into a 0-1 range relative to maxSpeed
        float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed);

        // Adjust pitch and volume based on normalized speed
        engineAudio.pitch = Mathf.Lerp(minPitch, maxPitch, normalizedSpeed);
        engineAudio.volume = Mathf.Lerp(minVolume, maxVolume, normalizedSpeed);
    }
}
