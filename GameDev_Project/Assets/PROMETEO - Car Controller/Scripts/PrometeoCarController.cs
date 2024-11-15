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
    [Range(10, 45)] public int maxSteeringAngle = 27;
    [Range(0.1f, 1f)] public float steeringSpeed = 0.5f;
    [Range(100, 600)] public int brakeForce = 350;
    [Range(1, 10)] public int decelerationMultiplier = 2;
    [Range(1, 10)] public int handbrakeDriftMultiplier = 5;
    public Vector3 bodyMassCenter;

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

    // CAR DATA
    [HideInInspector] public float carSpeed;
    [HideInInspector] public bool isDrifting;
    [HideInInspector] public bool isTractionLocked;

    // PRIVATE VARIABLES
    private Rigidbody carRigidbody;
    private float steeringAxis;
    private float throttleAxis;
    private float localVelocityZ;
    private float localVelocityX;
    private bool deceleratingCar;
    private bool touchControlsSetup = false;
    private float driftingAxis;

    // Control state
    private bool isControlEnabled = true; // New variable to track control state
    private float throttleInput = 0;      // To track throttle input
    private float steeringInput = 0;      // To track steering input
    private float handbrakeInput = 0;     // To track handbrake input
    private float currentSpeed = 0;       // To track current speed
    // Wheel friction curves
    private WheelFrictionCurve FLwheelFriction;
    private float FLWextremumSlip;
    private WheelFrictionCurve FRwheelFriction;
    private float FRWextremumSlip;
    private WheelFrictionCurve RLwheelFriction;
    private float RLWextremumSlip;
    private WheelFrictionCurve RRwheelFriction;
    private float RRWextremumSlip;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;

        InitializeWheelFriction();

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
        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        localVelocityX = transform.InverseTransformDirection(carRigidbody.velocity).x;
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;

        HandleInput();
        AnimateWheelMeshes();
    }

    private void InitializeWheelFriction()
    {
        FLwheelFriction = frontLeftCollider.sidewaysFriction;
        FLWextremumSlip = FLwheelFriction.extremumSlip;

        FRwheelFriction = frontRightCollider.sidewaysFriction;
        FRWextremumSlip = FRwheelFriction.extremumSlip;

        RLwheelFriction = rearLeftCollider.sidewaysFriction;
        RLWextremumSlip = RLwheelFriction.extremumSlip;

        RRwheelFriction = rearRightCollider.sidewaysFriction;
        RRWextremumSlip = RRwheelFriction.extremumSlip;
    }

    private void HandleInput()
    {
        if (!isControlEnabled) return;

        if (useTouchControls && touchControlsSetup)
        {
            if (throttlePTI.buttonPressed)
            {
                CancelInvoke(nameof(DecelerateCar));
                deceleratingCar = false;
                GoForward();
            }
            if (reversePTI.buttonPressed)
            {
                CancelInvoke(nameof(DecelerateCar));
                deceleratingCar = false;
                GoReverse();
            }
            if (turnLeftPTI.buttonPressed)
                TurnLeft();
            if (turnRightPTI.buttonPressed)
                TurnRight();
            if (!turnLeftPTI.buttonPressed && !turnRightPTI.buttonPressed && steeringAxis != 0f)
                ResetSteeringAngle();
            if (handbrakePTI.buttonPressed)
            {
                CancelInvoke(nameof(DecelerateCar));
                deceleratingCar = false;
                Handbrake();
            }
            else
                RecoverTraction();
            if (!throttlePTI.buttonPressed && !reversePTI.buttonPressed)
                ThrottleOff();
            if (!reversePTI.buttonPressed && !throttlePTI.buttonPressed && !handbrakePTI.buttonPressed && !deceleratingCar)
            {
                InvokeRepeating(nameof(DecelerateCar), 0f, 0.1f);
                deceleratingCar = true;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                CancelInvoke(nameof(DecelerateCar));
                deceleratingCar = false;
                GoForward();
            }
            if (Input.GetKey(KeyCode.S))
            {
                CancelInvoke(nameof(DecelerateCar));
                deceleratingCar = false;
                GoReverse();
            }
            if (Input.GetKey(KeyCode.A))
                TurnLeft();
            if (Input.GetKey(KeyCode.D))
                TurnRight();
            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f)
                ResetSteeringAngle();
            if (Input.GetKey(KeyCode.Space))
            {
                CancelInvoke(nameof(DecelerateCar));
                deceleratingCar = false;
                Handbrake();
            }
            if (Input.GetKeyUp(KeyCode.Space))
                RecoverTraction();
            if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
                ThrottleOff();
            if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.Space) && !deceleratingCar)
            {
                InvokeRepeating(nameof(DecelerateCar), 0f, 0.1f);
                deceleratingCar = true;
            }
        }
    }

    private void UpdateCarSpeedUI()
    {
        if (useUI && carSpeedText != null)
            carSpeedText.text = Mathf.RoundToInt(Mathf.Abs(carSpeed)).ToString();
    }

    private void UpdateCarSounds()
    {
        if (useSounds)
        {
            if (carEngineSound != null)
                carEngineSound.pitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.velocity.magnitude) / 25f);
            if ((isDrifting || (isTractionLocked && Mathf.Abs(carSpeed) > 12f)) && tireScreechSound != null)
            {
                if (!tireScreechSound.isPlaying)
                    tireScreechSound.Play();
            }
            else if ((!isDrifting && !isTractionLocked || Mathf.Abs(carSpeed) < 12f) && tireScreechSound != null)
                tireScreechSound.Stop();
        }
        else
        {
            carEngineSound?.Stop();
            tireScreechSound?.Stop();
        }
    }

    // Steering Methods
    public void TurnLeft()
    {
        steeringAxis -= Time.deltaTime * 10f * steeringSpeed;
        steeringAxis = Mathf.Clamp(steeringAxis, -1f, 1f);
        float steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    public void TurnRight()
    {
        steeringAxis += Time.deltaTime * 10f * steeringSpeed;
        steeringAxis = Mathf.Clamp(steeringAxis, -1f, 1f);
        float steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    public void ResetSteeringAngle()
    {
        if (steeringAxis < 0f)
            steeringAxis += Time.deltaTime * 10f * steeringSpeed;
        else if (steeringAxis > 0f)
            steeringAxis -= Time.deltaTime * 10f * steeringSpeed;

        if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
            steeringAxis = 0f;

        float steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
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

    // Engine and Braking Methods
    public void GoForward()
    {
        isDrifting = Mathf.Abs(localVelocityX) > 2.5f;
        DriftCarPS();
        throttleAxis += Time.deltaTime * 3f;
        throttleAxis = Mathf.Clamp(throttleAxis, 0f, 1f);

        if (localVelocityZ < -1f)
            ApplyBrakes();
        else
        {
            if (Mathf.RoundToInt(carSpeed) < maxSpeed)
            {
                ReleaseBrakes();
                float motorTorque = accelerationMultiplier * 50f * throttleAxis;
                ApplyMotorTorque(motorTorque);
            }
            else
                ApplyMotorTorque(0f);
        }
    }

    public void GoReverse()
    {
        isDrifting = Mathf.Abs(localVelocityX) > 2.5f;
        DriftCarPS();
        throttleAxis -= Time.deltaTime * 3f;
        throttleAxis = Mathf.Clamp(throttleAxis, -1f, 0f);

        if (localVelocityZ > 1f)
            ApplyBrakes();
        else
        {
            if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
            {
                ReleaseBrakes();
                float motorTorque = accelerationMultiplier * 50f * throttleAxis;
                ApplyMotorTorque(motorTorque);
            }
            else
                ApplyMotorTorque(0f);
        }
    }

    public void ThrottleOff()
    {
        ApplyMotorTorque(0f);
    }

    public void DecelerateCar()
    {
        isDrifting = Mathf.Abs(localVelocityX) > 2.5f;
        DriftCarPS();

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

    public void Handbrake()
    {
        CancelInvoke(nameof(RecoverTraction));
        driftingAxis += Time.deltaTime;
        driftingAxis = Mathf.Clamp(driftingAxis, 0f, 1f);

        float driftSlip = Mathf.Lerp(FLWextremumSlip, FLWextremumSlip * handbrakeDriftMultiplier, driftingAxis);
        AdjustFriction(driftSlip);

        isDrifting = Mathf.Abs(localVelocityX) > 2.5f;
        isTractionLocked = true;
        DriftCarPS();
    }

    private void AdjustFriction(float slip)
    {
        SetWheelFriction(frontLeftCollider, slip);
        SetWheelFriction(frontRightCollider, slip);
        SetWheelFriction(rearLeftCollider, slip);
        SetWheelFriction(rearRightCollider, slip);
    }

    private void SetWheelFriction(WheelCollider wheel, float extremumSlip)
    {
        WheelFrictionCurve friction = wheel.sidewaysFriction;
        friction.extremumSlip = extremumSlip;
        wheel.sidewaysFriction = friction;
    }

    public void RecoverTraction()
    {
        isTractionLocked = false;
        driftingAxis -= Time.deltaTime / 1.5f;
        driftingAxis = Mathf.Clamp(driftingAxis, 0f, 1f);

        float driftSlip = Mathf.Lerp(FLWextremumSlip * handbrakeDriftMultiplier, FLWextremumSlip, 1f - driftingAxis);
        AdjustFriction(driftSlip);

        if (driftingAxis > 0f)
            Invoke(nameof(RecoverTraction), Time.deltaTime);
    }

    public void DriftCarPS()
    {
        if (useEffects)
        {
            if (isDrifting)
            {
                RLWParticleSystem?.Play();
                RRWParticleSystem?.Play();
            }
            else
            {
                RLWParticleSystem?.Stop();
                RRWParticleSystem?.Stop();
            }

            bool emitSkid = (isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f;
            if (RLWTireSkid != null) RLWTireSkid.emitting = emitSkid;
            if (RRWTireSkid != null) RRWTireSkid.emitting = emitSkid;
        }
        else
        {
            RLWParticleSystem?.Stop();
            RRWParticleSystem?.Stop();
            if (RLWTireSkid != null) RLWTireSkid.emitting = false;
            if (RRWTireSkid != null) RRWTireSkid.emitting = false;
        }
    }

    public void SetCarControlsEnabled(bool enabled)
    {
        isControlEnabled = enabled;

        if (!enabled)
        {
            // Reset all movement inputs
            throttleInput = 0;
            steeringInput = 0;
            handbrakeInput = 1; // Apply handbrake
            currentSpeed = 0;

            // Stop rotation
            carRigidbody.angularVelocity = Vector3.zero;

            // Apply brake force to all wheels
            frontLeftCollider.brakeTorque = brakeForce;
            frontRightCollider.brakeTorque = brakeForce;
            rearLeftCollider.brakeTorque = brakeForce;
            rearRightCollider.brakeTorque = brakeForce;
        }
        else
        {
            // zpombie following and no white screen for options then  timer or health bar and zombie doing damage to car
            frontLeftCollider.brakeTorque = 0;
            frontRightCollider.brakeTorque = 0;
            rearLeftCollider.brakeTorque = 0;
            rearRightCollider.brakeTorque = 0;
            handbrakeInput = 0;
        }
    }

}