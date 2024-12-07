using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    [SerializeField] private string winSceneName = "LevelWon"; // Name of the winning scene to load

    private void OnTriggerEnter(Collider other)
    {
            // Call LevelManager's method to load the win scene
            LevelManager.Instance.LoadWinScene(winSceneName);
            Debug.Log("Level won! Requested LevelManager to load win scene.");
    }
}
