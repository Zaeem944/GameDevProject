using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f; // Time in seconds before destruction

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // If using animation events, you can add this method
    public void DestroyNow()
    {
        Destroy(gameObject);
    }
}
