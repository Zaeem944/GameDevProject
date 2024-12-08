using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = GameManager.Instance?.audioManager;

        if (audioManager != null)
        {
            audioManager.PlaySixthAudio();
        }
        else
        {
            Debug.LogError("MenuMusic: AudioManager instance not found in GameManager.");
        }
    }

    private void OnDestroy()
    {
        if (audioManager != null)
        {
            audioManager.StopSixthAudio();
        }
    }
}
