using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip firstAudio;
    [SerializeField] private AudioClip secondAudio;
    [SerializeField] private AudioClip thirdAudio;

    private AudioSource audioSource;

    private void Awake()
    {
        // Simple singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Play the first audio clip
    public void PlayFirstAudio()
    {
        if (firstAudio != null)
        {
            audioSource.PlayOneShot(firstAudio);
        }
        else
        {
            Debug.LogWarning("First audio clip is not assigned!");
        }
    }

    // Play the second audio clip
    public void PlaySecondAudio()
    {
        if (secondAudio != null)
        {
            audioSource.PlayOneShot(secondAudio);
        }
        else
        {
            Debug.LogWarning("Second audio clip is not assigned!");
        }
    }
    public void PlayThirdAudio()
    {
        if (secondAudio != null)
        {
            audioSource.PlayOneShot(thirdAudio);
        }
        else
        {
            Debug.LogWarning("Third audio clip is not assigned!");
        }
    }
}