using UnityEngine;

public class DynamicFogController : MonoBehaviour
{
    public Transform player; // Reference to the player transform
    public float maxDistance = 100f; // Maximum distance to start full fog
    public float minDistance = 10f;  // Minimum distance for no fog
    public Color fogColor = new Color(0.7f, 0.7f, 0.7f, 1f); // Light grey fog

    void Start()
    {
        // Initial fog settings
        RenderSettings.fog = true;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = FogMode.Linear; // You can also try Exponential or ExponentialSquared
        RenderSettings.fogStartDistance = 0f;    // Start very close to player
        RenderSettings.fogEndDistance = maxDistance; // Full fog at maximum distance
    }

    void Update()
    {
        // Adjust fog dynamically based on player's position or other conditions
        float distance = Vector3.Distance(player.position, Vector3.zero); // Calculate distance from a point (e.g., origin)

        // Clamp the distance between minDistance and maxDistance
        float fogFactor = Mathf.Clamp(distance, minDistance, maxDistance);

        // Set fog parameters dynamically
        RenderSettings.fogStartDistance = Mathf.Lerp(0f, minDistance, fogFactor / maxDistance);
        RenderSettings.fogEndDistance = Mathf.Lerp(minDistance, maxDistance, fogFactor / maxDistance);
    }
}
