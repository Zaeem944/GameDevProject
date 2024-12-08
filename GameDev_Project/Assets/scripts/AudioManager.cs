using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip firstAudio;
    [SerializeField] private AudioClip secondAudio;
    [SerializeField] private AudioClip thirdAudio;
    [SerializeField] private AudioClip fourthAudio;
    [SerializeField] private AudioClip fifthAudio;
    [SerializeField] private AudioClip sixthAudio; 

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxAudioSource; 
    [SerializeField] private AudioSource bgmAudioSource; 

    private void Awake()
    {
        if (sfxAudioSource == null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("AudioManager: SFX AudioSource component added.");
        }

        if (bgmAudioSource == null)
        {
            bgmAudioSource = gameObject.AddComponent<AudioSource>();
            bgmAudioSource.loop = true; 
            Debug.Log("AudioManager: BGM AudioSource component added with looping enabled.");
        }

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
            sfxAudioSource.PlayOneShot(firstAudio);
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
            sfxAudioSource.PlayOneShot(secondAudio);
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
            sfxAudioSource.PlayOneShot(thirdAudio);
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
            sfxAudioSource.PlayOneShot(fourthAudio);
            Debug.Log("AudioManager: Playing fourth audio.");
        }
        else
        {
            Debug.LogWarning("AudioManager: Fourth audio clip is not assigned!");
        }
    }

    public void PlayFifthAudio()
    {
        if (fifthAudio != null)
        {
            sfxAudioSource.PlayOneShot(fifthAudio);
            Debug.Log("AudioManager: Playing fifth audio.");
        }
        else
        {
            Debug.LogWarning("AudioManager: Fifth audio clip is not assigned!");
        }
    }

    public void PlaySixthAudio()
    {
        if (sixthAudio != null)
        {
            if (!bgmAudioSource.isPlaying)
            {
                bgmAudioSource.clip = sixthAudio;
                bgmAudioSource.Play();
                Debug.Log("AudioManager: Playing sixth audio (Background Music).");
            }
            else
            {
                Debug.Log("AudioManager: Background music is already playing.");
            }
        }
        else
        {
            Debug.LogWarning("AudioManager: Sixth audio clip (Background Music) is not assigned!");
        }
    }

    public void StopSixthAudio()
    {
        if (bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
            Debug.Log("AudioManager: Stopped sixth audio (Background Music).");
        }
        else
        {
            Debug.Log("AudioManager: Background music is not playing.");
        }
    }

    public void ToggleSixthAudio()
    {
        if (bgmAudioSource.isPlaying)
        {
            StopSixthAudio();
        }
        else
        {
            PlaySixthAudio();
        }
    }
}
