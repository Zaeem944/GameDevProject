using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    public string winSceneName = "LevelWon"; // Name of the winning scene to load

    private void OnTriggerEnter(Collider other)
    {
         SceneManager.LoadScene(winSceneName);
         Debug.Log("Level won! Loading win scene.");
    }
}
