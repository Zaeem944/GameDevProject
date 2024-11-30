using System;
using UnityEngine;
using UnityEngine.UI;

public class PrometeoCarController : MonoBehaviour
{
    [Range(20, 190)] public int maxSpeed = 90;
    [Range(10, 120)] public int maxReverseSpeed = 45;
    [Range(1, 10)] public int accelerationMultiplier = 2;
    [Range(100, 600)] public int brakeForce = 350;
    [Range(1, 10)] public int decelerationMultiplier = 2;
    public Vector3 bodyMassCenter;

    public float jumpDistance = 3f;
    public float jumpCooldown = 0.5f;
    private float lastJumpTime = 0f;
    private bool isJumping = false;
    private Vector3 jumpTarget;

    public GameObject frontLeftMesh;
    public WheelCollider frontLeftCollider;
    public GameObject frontRightMesh;
    public WheelCollider frontRightCollider;
    public GameObject rearLeftMesh;
    public WheelCollider rearLeftCollider;
    public GameObject rearRightMesh;
    public WheelCollider rearRightCollider;


 
    public bool useSounds = false;
    public AudioSource carEngineSound;
    public AudioSource tireScreechSound;
    private float initialCarEngineSoundPitch;

    public bool useTouchControls = false;
    public GameObject throttleButton;
    public GameObject reverseButton;
    public GameObject turnRightButton;
    public GameObject turnLeftButton;
    public GameObject handbrakeButton;

    private bool isControlEnabled = true;

    [HideInInspector] public float carSpeed;

    private Rigidbody carRigidbody;
    private float throttleAxis;
    private float localVelocityZ;
    private bool deceleratingCar;
    public bool IsLeft = true;
    public bool IsRight = false;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;
        carRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (!isControlEnabled) return;

        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;

        HandleInput();
        MoveForward();

        if (isJumping)
        {
            SmoothJump();
        }
    }
    public void MoveForward()
    {
        if (!isControlEnabled) return;

        float targetSpeed = maxSpeed;
        float motorTorque = accelerationMultiplier * 50f;

        frontLeftCollider.motorTorque = motorTorque;
        frontRightCollider.motorTorque = motorTorque;
        rearLeftCollider.motorTorque = motorTorque;
        rearRightCollider.motorTorque = motorTorque;
    }

    private void SmoothJump()
    {
        transform.position = Vector3.MoveTowards(transform.position, jumpTarget, Time.deltaTime * jumpDistance * 5f);
        if (Vector3.Distance(transform.position, jumpTarget) < 0.01f)
        {
            transform.position = jumpTarget; 
            isJumping = false; 
        }
    }

    private void HandleInput()
    {
        if (!isControlEnabled) return;

        if (Input.GetKey(KeyCode.A))
            TryJumpLeft();
        if (Input.GetKey(KeyCode.D))
            TryJumpRight();
    }

    private void TryJumpLeft()
    {
        if (Time.time - lastJumpTime >= jumpCooldown && !IsLeft && !isJumping)
        {
            jumpTarget = transform.position - transform.right * jumpDistance;
            isJumping = true; // Start the jump
            lastJumpTime = Time.time;
            IsLeft = true;
            IsRight = false;
        }
    }

    private void TryJumpRight()
    {
        if (Time.time - lastJumpTime >= jumpCooldown && !IsRight && !isJumping)
        {
            jumpTarget = transform.position + transform.right * jumpDistance;
            isJumping = true; // Start the jump
            lastJumpTime = Time.time;
            IsRight = true;
            IsLeft = false;
        }
    }



    public void ThrottleOff()
    {
        ApplyMotorTorque(0f);
    }

    public void DecelerateCar()
    {
        if (throttleAxis != 0f)
            throttleAxis = Mathf.MoveTowards(throttleAxis, 0f, Time.deltaTime * 10f);

        carRigidbody.velocity *= (1f / (1f + (0.025f * decelerationMultiplier)));
        ApplyMotorTorque(0f);

        if (carRigidbody.velocity.magnitude < 0.25f)
        {
            carRigidbody.velocity = Vector3.zero;
            CancelInvoke(nameof(DecelerateCar));
        }
    }

    private void ApplyBrakes()
    {
        frontLeftCollider.brakeTorque = brakeForce;
        frontRightCollider.brakeTorque = brakeForce;
        rearLeftCollider.brakeTorque = brakeForce;
        rearRightCollider.brakeTorque = brakeForce;
    }

    private void ReleaseBrakes()
    {
        frontLeftCollider.brakeTorque = 0f;
        frontRightCollider.brakeTorque = 0f;
        rearLeftCollider.brakeTorque = 0f;
        rearRightCollider.brakeTorque = 0f;
    }

    private void ApplyMotorTorque(float torque)
    {
        frontLeftCollider.motorTorque = torque;
        frontRightCollider.motorTorque = torque;
        rearLeftCollider.motorTorque = torque;
        rearRightCollider.motorTorque = torque;
    }

    public void SetCarControlsEnabled(bool enabled)
    {
        isControlEnabled = enabled;

        if (!enabled)
        {
            throttleAxis = 0;

            carRigidbody.velocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;

            ApplyBrakes();

            CancelInvoke(nameof(DecelerateCar));

            if (useSounds && carEngineSound != null)
            {
                carEngineSound.pitch = initialCarEngineSoundPitch;
            }
        }
        else
        {
            ReleaseBrakes();
        }
    }
}