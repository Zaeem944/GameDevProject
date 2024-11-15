using UnityEngine;
using System.Collections;

public class PizzaCollision : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 45f;
    [SerializeField] private float verticalSpeed = 1f;
    [SerializeField] private float verticalRange = 0.5f;

    private void Start()
    {
        StartCoroutine(MoveVertically());
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private System.Collections.IEnumerator MoveVertically()
    {
        float initialY = transform.position.y;
        float currentY = initialY;
        float targetY;
        float direction = 1f;

        while (true)
        {
            targetY = initialY + direction * verticalRange;
            currentY = Mathf.Lerp(currentY, targetY, verticalSpeed * Time.deltaTime);
            transform.position = new Vector3(
                transform.position.x,
                currentY,
                transform.position.z
            );

            if (Mathf.Abs(currentY - targetY) < 0.01f)
            {
                direction *= -1;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        int currentLives = HealthManager.Instance.GetCurrentLives();
        if (currentLives < 5)
        {
            HealthManager.Instance.GainLife();
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}