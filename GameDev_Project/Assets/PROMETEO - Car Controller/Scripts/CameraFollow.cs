using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform carTransform;           
    public float followSpeed = 2f;           
    public float turnSpeed = 5f;             

    private Vector3 offset;                  

    void Start()
    {
        offset = transform.position - carTransform.position;
    }

    void LateUpdate()
    {
        Vector3 targetPosition = carTransform.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(carTransform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
}
