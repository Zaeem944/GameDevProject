using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class CarEngineSound : MonoBehaviour
{
    [Header("Engine Sound Settings")]
    [Tooltip("At zero speed, engine pitch will be at this minimum value.")]
    [SerializeField] private float minPitch = 0.8f;

    [Tooltip("At maximum speed (maxSpeed), engine pitch will be at this maximum value.")]
    [SerializeField] private float maxPitch = 2.0f;

    [Tooltip("At zero speed, engine volume will be at this minimum value.")]
    [SerializeField] private float minVolume = 0.5f;

    [Tooltip("At maximum speed (maxSpeed), engine volume will be at this maximum value.")]
    [SerializeField] private float maxVolume = 1.0f;

    [Header("Speed Settings")]
    [Tooltip("Maximum speed at which engine sound will reach its max pitch/volume.")]
    [SerializeField] private float maxSpeed = 100f;

    private AudioSource engineAudio;
    private Rigidbody carRigidbody;

    private void Start()
    {
        engineAudio = GetComponent<AudioSource>();
        carRigidbody = GetComponent<Rigidbody>();

        engineAudio.loop = true;

        if (!engineAudio.isPlaying)
        {
            engineAudio.Play();
        }
    }

    private void Update()
    {
        float speed = carRigidbody.velocity.magnitude;

        float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed);

        engineAudio.pitch = Mathf.Lerp(minPitch, maxPitch, normalizedSpeed);
        engineAudio.volume = Mathf.Lerp(minVolume, maxVolume, normalizedSpeed);
    }
}
