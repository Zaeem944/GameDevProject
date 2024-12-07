using System;
using UnityEngine;
using UnityEngine.UI;

public class PrometeoCarController : MonoBehaviour
{
    [Range(20, 190)] public int maxSpeed = 190;
    [Range(10, 120)] public int maxReverseSpeed = 45;
    [Range(1, 10)] public int accelerationMultiplier = 5;
    [Range(100, 600)] public int brakeForce = 350;
    [Range(1, 10)] public int decelerationMultiplier = 5;
    public Vector3 bodyMassCenter;

    public float jumpDistance = 8.77f;
    public float jumpCooldown = 0.5f;

    private float lastJumpTime = 0f;
    private bool isJumping = false;
    private Vector3 jumpTarget;
    private Vector3 jumpStartPos;
    private Quaternion jumpStartRot;
    private float jumpTimeElapsed = 0f;

    // Slightly increase duration for smoother transition
    [SerializeField] private float jumpDuration = 0.3f;

    // Increase turn angle for a more pronounced steering look
    private float turnAngle = 20f;

    private int jumpDirection = 0; // -1 for left, +1 for right

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
    public int posState = 2; // 1=left, 2=middle, 3=right

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
        jumpTimeElapsed += Time.deltaTime;
        float t = jumpTimeElapsed / jumpDuration;
        t = Mathf.Clamp01(t);

        // Position: Add a slight forward arc to simulate a real turn
        // Sine curve: peaks at mid-transition, giving a slight bulge forward
        float arcOffset = 0.5f; // Adjust this for more or less forward bulge
        Vector3 forwardOffset = transform.forward * arcOffset * Mathf.Sin(t * Mathf.PI);

        Vector3 newPosition = Vector3.Lerp(jumpStartPos, jumpTarget, t) + forwardOffset;
        transform.position = newPosition;

        // Rotation: Use a sine curve to simulate steering into the turn:
        // At t=0 and t=1, angle=0 (car straight)
        // At t=0.5, angle=max turn angle
        float steeringCurve = Mathf.Sin(t * Mathf.PI);
        float currentAngle = steeringCurve * turnAngle * jumpDirection;

        transform.rotation = jumpStartRot * Quaternion.Euler(0f, currentAngle, 0f);

        if (t >= 1f)
        {
            // Jump complete
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
        if (Time.time - lastJumpTime >= jumpCooldown && posState != 1 && !isJumping)
        {
            jumpDirection = -1;
            jumpTarget = transform.position - transform.right * jumpDistance;
            StartJump();
            posState -= 1;
        }
    }

    private void TryJumpRight()
    {
        if (Time.time - lastJumpTime >= jumpCooldown && posState != 3 && !isJumping)
        {
            jumpDirection = 1;
            jumpTarget = transform.position + transform.right * jumpDistance;
            StartJump();
            posState += 1;
        }
    }

    private void StartJump()
    {
        isJumping = true;
        lastJumpTime = Time.time;
        jumpTimeElapsed = 0f;

        jumpStartPos = transform.position;
        jumpStartRot = transform.rotation;
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
