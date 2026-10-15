using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using NUnit.Framework;

public class CarControl : MonoBehaviour
{
    [Header("Car Properties")]
    public float motorTorque = 2000f;
    public float brakeTorque = 2000f;
    public float maxSpeed = 20f;
    public float steeringRange = 30f;
    public float steeringRangeAtMaxSpeed = 10f;
    public float centreOfGravityOffset = -1f;
    public int crashThreshold = 100;
    public AudioSource crashSource;

    private WheelControl[] wheels;
    private Rigidbody rigidBody;
    private VoiceInput voiceInput;
    private WheelFrictionCurve sideFric;
    private bool handbraking = false;
    private float prevSpeed;
    
    [Header("Gyro Controls")]
    [Min(1f)] public float fullSteeringTilt = 45f;
    [Min(1f)] public float fullAccelerationTilt = 30f;
    [Min(0f)] public float steeringDeadZone = 2f;
    [Min(0f)] public float accelerationDeadZone = 5f;
    [Min(0f)] public float inputSmoothing = 10f;
    [Tooltip("Throttle change per second at full tilt. Neutral tilt holds the current throttle.")]
    [Min(0f)] public float throttleChangeRate = 0.5f;
    public bool invertSteering = false;
    public bool invertAcceleration = false;

    private Gyroscope gyro;
    private Quaternion neutralAttitude;
    private ScreenOrientation calibratedOrientation;
    private bool gyroCalibrated;
    private Vector2 gyroInput;
    private float throttle;

    [Header("OtherObjects")]
    // public TMP_Text speedText;
    public TMP_Text brakeText;
    public GameObject startPos;
    [Tooltip("UI Image object with Image Type set to Filled. If unassigned, finds the active object tagged ThrottleBar. Displays gyro throttle from 0 to 1.")]
    public GameObject throttleBar;
    [Tooltip("Shield object whose active state follows VoiceInput.shieldActive.")]
    public GameObject shield;

    private UnityEngine.UI.Image throttleBarImage;

    private void OnEnable()
    {
        throttle = 0f;
        gyroInput = Vector2.zero;
        gyroCalibrated = false;
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            gyro.updateInterval = 1f / 60f;
        }
        else
        {
            Debug.LogWarning("Gyroscope unavailable. Car movement input will remain neutral.", this);
        }
    }

    private void OnDisable()
    {
        throttle = 0f;
        gyroCalibrated = false;
        gyroInput = Vector2.zero;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            gyroCalibrated = false;
            gyroInput = Vector2.zero;
        }
    }

    public void CalibrateGyro()
    {
        if (gyro == null || !gyro.enabled || gyro.gravity.sqrMagnitude < 0.25f)
        {
            return;
        }

        neutralAttitude = GetScreenAttitude();
        calibratedOrientation = Screen.orientation;
        gyroCalibrated = true;
        gyroInput = Vector2.zero;
    }

    private Quaternion GetScreenAttitude()
    {
        Quaternion attitude = gyro.attitude;
        Quaternion unityAttitude = new Quaternion(attitude.x, attitude.y, -attitude.z, -attitude.w);
        if (Input.compensateSensors)
        {
            return unityAttitude;
        }

        float screenRotation = 0f;
        switch (Screen.orientation)
        {
            case ScreenOrientation.LandscapeLeft:
                screenRotation = -90f;
                break;
            case ScreenOrientation.LandscapeRight:
                screenRotation = 90f;
                break;
            case ScreenOrientation.PortraitUpsideDown:
                screenRotation = 180f;
                break;
        }

        return unityAttitude * Quaternion.AngleAxis(screenRotation, Vector3.forward);
    }

    private static float NormalizeTilt(float angle, float deadZone, float fullTilt)
    {
        float effectiveDeadZone = Mathf.Clamp(deadZone, 0f, Mathf.Max(1f, fullTilt) - 0.01f);
        return Mathf.Sign(angle) * Mathf.InverseLerp(effectiveDeadZone, Mathf.Max(1f, fullTilt), Mathf.Abs(angle));
    }

    private void UpdateGyroInput()
    {
        if (gyro == null || !gyro.enabled)
        {
            gyroInput = Vector2.zero;
            return;
        }

        if (!gyroCalibrated || calibratedOrientation != Screen.orientation)
        {
            CalibrateGyro();
            return;
        }

        Quaternion relativeAttitude = Quaternion.Inverse(neutralAttitude) * GetScreenAttitude();
        Vector3 deviceUp = relativeAttitude * Vector3.up;
        Vector3 deviceForward = relativeAttitude * Vector3.forward;
        float steeringAngle = Mathf.Atan2(deviceUp.x, deviceUp.y) * Mathf.Rad2Deg;
        float accelerationAngle = Mathf.Atan2(-deviceForward.y,
            Mathf.Sqrt(deviceForward.x * deviceForward.x + deviceForward.z * deviceForward.z)) * Mathf.Rad2Deg;
        if (invertSteering) steeringAngle = -steeringAngle;
        if (invertAcceleration) accelerationAngle = -accelerationAngle;

        Vector2 targetInput = new Vector2(
            NormalizeTilt(steeringAngle, steeringDeadZone, fullSteeringTilt),
            NormalizeTilt(accelerationAngle, accelerationDeadZone, fullAccelerationTilt));
        float smoothing = inputSmoothing > 0f ? 1f - Mathf.Exp(-inputSmoothing * Time.deltaTime) : 1f;
        gyroInput = Vector2.Lerp(gyroInput, targetInput, smoothing);
        // Stop adjustment immediately in the neutral dead zone, without smoothing drift.
        if (targetInput.y == 0f)
        {
            gyroInput.y = 0f;
        }
        throttle = Mathf.Clamp01(throttle + gyroInput.y * throttleChangeRate * Time.deltaTime);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject listener = GameObject.FindGameObjectWithTag("Listener");
        if (listener != null)
        {
            voiceInput = listener.GetComponent<VoiceInput>();
            if (voiceInput == null)
            {
                Debug.LogError("Object tagged Listener must have a VoiceInput component. Boost and shield will be unavailable.", listener);
            }
        }
        else
        {
            Debug.LogWarning("No active object tagged Listener found. Boost and shield will be unavailable.", this);
        }

        if (shield == null)
        {
            Debug.LogWarning("No shield object assigned. Shield display will not update.", this);
        }

        if (throttleBar == null)
        {
            throttleBar = GameObject.FindGameObjectWithTag("ThrottleBar");
        }

        if (throttleBar != null)
        {
            throttleBarImage = throttleBar.GetComponent<UnityEngine.UI.Image>();
            if (throttleBarImage == null)
            {
                Debug.LogError("Throttle bar must have a UnityEngine.UI.Image component.", throttleBar);
            }
            else
            {
                throttleBarImage.fillAmount = 0f;
            }
        }
        else
        {
            Debug.LogWarning("No active object tagged ThrottleBar found. Throttle display will not update.", this);
        }

        rigidBody = GetComponent<Rigidbody>();

        // Adjust center of mass to improve stability and prevent rolling
        Vector3 centerOfMass = rigidBody.centerOfMass;
        centerOfMass.y += centreOfGravityOffset;
        rigidBody.centerOfMass = centerOfMass;

        // Get all wheel components attached to the car
        wheels = GetComponentsInChildren<WheelControl>();

        if(brakeText == null)
        {
            brakeText = GameObject.Find("Handbrake Status").GetComponent<TMP_Text>();
        }
    }

    void Update()
    {
        if (shield != null)
        {
            shield.SetActive(voiceInput != null && voiceInput.shieldActive);
        }

        UpdateGyroInput();
        if (throttleBarImage != null)
        {
            throttleBarImage.fillAmount = throttle;
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            rigidBody.transform.position = startPos.transform.position;
            rigidBody.transform.rotation = startPos.transform.rotation;
            rigidBody.linearVelocity = new Vector3(0,0,0);
            // Debug.Log("reset car pos");
        }

        //reduce friction of wheels to simulate handbraking
        if (Input.GetKeyDown(KeyCode.Space))
        {
            handbraking = true;
            brakeText.text = "Handbrake: " + handbraking;
            print("Handbraking: " + handbraking);
            foreach(var wheel in wheels)
            {
                if (!wheel.steerable)
                {
                    sideFric = wheel.WheelCollider.sidewaysFriction;
                    if (handbraking)
                    {
                        sideFric.stiffness = 0.4f;
                    } else
                    {
                        sideFric.stiffness = 1f;
                    }
                    wheel.WheelCollider.sidewaysFriction = sideFric;
                }
            }

            
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            handbraking = false;
            brakeText.text = "Handbrake: " + handbraking;
            print("Handbraking: " + handbraking);
            foreach(var wheel in wheels)
            {
                if (!wheel.steerable)
                {
                    sideFric = wheel.WheelCollider.sidewaysFriction;
                    if (handbraking)
                    {
                        sideFric.stiffness = 0f;
                    } else
                    {
                        sideFric.stiffness = 1f;
                    }
                    wheel.WheelCollider.sidewaysFriction = sideFric;
                }
            }
        }

        prevSpeed = rigidBody.linearVelocity.magnitude;
    }

    // FixedUpdate is called at a fixed time interval 
    void FixedUpdate()
    {
        // Get player input for acceleration and steering
        float vInput = throttle; // Stored forward throttle
        float hInput = gyroInput.x; // Steering input

        bool boostActive = voiceInput != null && voiceInput.boostActive;
        float currentMaxSpeed = maxSpeed * (boostActive ? 1.25f : 1f);

        // Calculate current speed along the car's forward axis
        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.linearVelocity);
        float speedFactor = Mathf.InverseLerp(0, currentMaxSpeed, Mathf.Abs(forwardSpeed)); // Normalized speed factor

        //incentivise drifting
        if (handbraking)
        {
            speedFactor = Mathf.InverseLerp(0, currentMaxSpeed * 1.5f, Mathf.Abs(forwardSpeed));
        }

        // Reduce motor torque and steering at high speeds for better handling
        float boostMultiplier = boostActive ? 1.5f : 1f;
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor) * boostMultiplier;
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        // Determine if the player is accelerating or trying to reverse
        bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);


        foreach (var wheel in wheels)
        {
            // Apply steering to wheels that support steering
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
            }

            if (isAccelerating)
            {
                // Apply torque to motorized wheels
                if (wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = vInput * currentMotorTorque;
                }
                // Release brakes when accelerating
                wheel.WheelCollider.brakeTorque = 0f;
            }
            else
            {
                // Apply brakes when reversing direction
                if (wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = 0f;
                    wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
                }
            }
        }

        // speedText.text = "Speed: " + rigidBody.linearVelocity.magnitude;
    }

    void OnCollisionEnter(Collision collision)
    {
        print("prevSpeed "+prevSpeed);
        if(prevSpeed > crashThreshold && rigidBody.linearVelocity.magnitude < 0.7 * prevSpeed)
        {
            print("crashed");
            crashSource.time = 1.8f;
            crashSource.Play();
        }
    }

}
