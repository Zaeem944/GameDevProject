using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip firstAudio;
    [SerializeField] private AudioClip secondAudio;
    [SerializeField] private AudioClip thirdAudio;
    [SerializeField] private AudioClip fourthAudio;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("AudioManager: AudioSource component added.");
        }

        // Register with GameManager Singleton
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterAudioManager(this);
            Debug.Log($"AudioManager: Registered with GameManager: {GameManager.Instance.gameObject.name}");
        }
        else
        {
            Debug.LogError("AudioManager: GameManager Singleton instance not found.");
        }
    }

    public void PlayFirstAudio()
    {
        if (firstAudio != null)
        {
            audioSource.PlayOneShot(firstAudio);
            Debug.Log("AudioManager: Playing first audio.");
        }
        else
        {
            Debug.LogWarning("AudioManager: First audio clip is not assigned!");
        }
    }

    public void PlaySecondAudio()
    {
        if (secondAudio != null)
        {
            audioSource.PlayOneShot(secondAudio);
            Debug.Log("AudioManager: Playing second audio.");
        }
        else
        {
            Debug.LogWarning("AudioManager: Second audio clip is not assigned!");
        }
    }

    public void PlayThirdAudio()
    {
        if (thirdAudio != null)
        {
            audioSource.PlayOneShot(thirdAudio);
            Debug.Log("AudioManager: Playing third audio.");
        }
        else
        {
            Debug.LogWarning("AudioManager: Third audio clip is not assigned!");
        }
    }

    public void PlayFourthAudio()
    {
        if (fourthAudio != null)
        {
            audioSource.PlayOneShot(fourthAudio);
            Debug.Log("AudioManager: Playing fourth audio.");
        }
        else
        {
            Debug.LogWarning("AudioManager: Fourth audio clip is not assigned!");
        }
    }
}
