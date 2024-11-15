using System;
using UnityEngine;
using UnityEngine.UI;

public class PrometeoCarController : MonoBehaviour
{
    // CAR SETUP
    [Header("CAR SETUP")]
    [Range(20, 190)] public int maxSpeed = 90;
    [Range(10, 120)] public int maxReverseSpeed = 45;
    [Range(1, 10)] public int accelerationMultiplier = 2;
    [Range(100, 600)] public int brakeForce = 350;
    [Range(1, 10)] public int decelerationMultiplier = 2;
    public Vector3 bodyMassCenter;

    // New jump settings
    [Header("SIDE JUMP SETTINGS")]
    public float jumpDistance = 3f;
    public float jumpCooldown = 0.5f;
    private float lastJumpTime = 0f;

    // WHEELS
    public GameObject frontLeftMesh;
    public WheelCollider frontLeftCollider;
    public GameObject frontRightMesh;
    public WheelCollider frontRightCollider;
    public GameObject rearLeftMesh;
    public WheelCollider rearLeftCollider;
    public GameObject rearRightMesh;
    public WheelCollider rearRightCollider;

    // EFFECTS
    public bool useEffects = false;
    public ParticleSystem RLWParticleSystem;
    public ParticleSystem RRWParticleSystem;
    public TrailRenderer RLWTireSkid;
    public TrailRenderer RRWTireSkid;

    // UI
    public bool useUI = false;
    public Text carSpeedText;

    // SOUNDS
    public bool useSounds = false;
    public AudioSource carEngineSound;
    public AudioSource tireScreechSound;
    private float initialCarEngineSoundPitch;

    // CONTROLS
    public bool useTouchControls = false;
    public GameObject throttleButton;
    public GameObject reverseButton;
    public GameObject turnRightButton;
    public GameObject turnLeftButton;
    public GameObject handbrakeButton;

    private PrometeoTouchInput throttlePTI;
    private PrometeoTouchInput reversePTI;
    private PrometeoTouchInput turnRightPTI;
    private PrometeoTouchInput turnLeftPTI;
    private PrometeoTouchInput handbrakePTI;

    // Control state
    private bool isControlEnabled = true;
    private float throttleInput = 0;
    private float currentSpeed = 0;

    // CAR DATA
    [HideInInspector] public float carSpeed;

    // PRIVATE VARIABLES
    private Rigidbody carRigidbody;
    private float throttleAxis;
    private float localVelocityZ;
    private bool deceleratingCar;
    private bool touchControlsSetup = false;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;
        carRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        if (carEngineSound != null)
            initialCarEngineSoundPitch = carEngineSound.pitch;

        if (useUI)
            InvokeRepeating(nameof(UpdateCarSpeedUI), 0f, 0.1f);
        else if (carSpeedText != null)
            carSpeedText.text = "0";

        if (useSounds)
            InvokeRepeating(nameof(UpdateCarSounds), 0f, 0.1f);
        else
        {
            carEngineSound?.Stop();
            tireScreechSound?.Stop();
        }

        if (!useEffects)
        {
            RLWParticleSystem?.Stop();
            RRWParticleSystem?.Stop();
            if (RLWTireSkid != null) RLWTireSkid.emitting = false;
            if (RRWTireSkid != null) RRWTireSkid.emitting = false;
        }

        SetupTouchControls();
    }

    private void SetupTouchControls()
    {
        if (useTouchControls)
        {
            if (throttleButton != null && reverseButton != null &&
                turnRightButton != null && turnLeftButton != null &&
                handbrakeButton != null)
            {
                throttlePTI = throttleButton.GetComponent<PrometeoTouchInput>();
                reversePTI = reverseButton.GetComponent<PrometeoTouchInput>();
                turnRightPTI = turnRightButton.GetComponent<PrometeoTouchInput>();
                turnLeftPTI = turnLeftButton.GetComponent<PrometeoTouchInput>();
                handbrakePTI = handbrakeButton.GetComponent<PrometeoTouchInput>();
                touchControlsSetup = true;
            }
            else
            {
                Debug.LogWarning("Touch controls are not completely set up.");
            }
        }
    }

    void Update()
    {
        if (!isControlEnabled) return;

        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;

        HandleInput();
        AnimateWheelMeshes();
        MoveForward();
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

    private void HandleInput()
    {
        if (!isControlEnabled) return;

        if (useTouchControls && touchControlsSetup)
        {
            if (turnLeftPTI.buttonPressed)
                TryJumpLeft();
            if (turnRightPTI.buttonPressed)
                TryJumpRight();
        }
        else
        {
            if (Input.GetKey(KeyCode.A))
                TryJumpLeft();
            if (Input.GetKey(KeyCode.D))
                TryJumpRight();
        }
    }

    private void TryJumpLeft()
    {
        if (Time.time - lastJumpTime >= jumpCooldown)
        {
            Vector3 jumpPosition = transform.position - transform.right * jumpDistance;
            transform.position = jumpPosition;
            lastJumpTime = Time.time;
        }
    }

    private void TryJumpRight()
    {
        if (Time.time - lastJumpTime >= jumpCooldown)
        {
            Vector3 jumpPosition = transform.position + transform.right * jumpDistance;
            transform.position = jumpPosition;
            lastJumpTime = Time.time;
        }
    }

    private void AnimateWheelMeshes()
    {
        UpdateWheelPose(frontLeftCollider, frontLeftMesh);
        UpdateWheelPose(frontRightCollider, frontRightMesh);
        UpdateWheelPose(rearLeftCollider, rearLeftMesh);
        UpdateWheelPose(rearRightCollider, rearRightMesh);
    }

    private void UpdateWheelPose(WheelCollider collider, GameObject mesh)
    {
        collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        mesh.transform.position = position;
        mesh.transform.rotation = rotation;
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

    private void UpdateCarSpeedUI()
    {
        if (useUI && carSpeedText != null)
            carSpeedText.text = Mathf.RoundToInt(Mathf.Abs(carSpeed)).ToString();
    }

    private void UpdateCarSounds()
    {
        if (useSounds && carEngineSound != null)
        {
            carEngineSound.pitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.velocity.magnitude) / 25f);
        }
    }

    public void SetCarControlsEnabled(bool enabled)
    {
        isControlEnabled = enabled;

        if (!enabled)
        {
            throttleInput = 0;
            throttleAxis = 0;
            currentSpeed = 0;

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