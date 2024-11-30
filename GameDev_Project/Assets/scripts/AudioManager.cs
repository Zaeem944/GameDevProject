using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip firstAudio;
    [SerializeField] private AudioClip secondAudio;
    [SerializeField] private AudioClip thirdAudio;
    [SerializeField] private AudioClip FourthAudio;

    private AudioSource audioSource;

    private void Awake()
    {
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

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

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

    public void PlayFourthAudio()
    {
        if (secondAudio != null)
        {
            audioSource.PlayOneShot(FourthAudio);
        }
        else
        {
            Debug.LogWarning("Fourth audio clip is not assigned!");
        }
    }
}