using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float fullSteeringTilt = 45f;
    public float fullAccelerationTilt = 30f;
    public float steeringDeadZone = 2f;
    public float accelerationDeadZone = 5f;
    public float inputSmoothing = 10f;
    private Gyroscope gyro;
    private Quaternion neutralAttitude;
    private ScreenOrientation calibratedOrientation;
    private bool gyroCalibrated;
    private Vector2 gyroInput;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

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

    void FixedUpdate()
    {
        UpdateGyroInput();
        float vInput = gyroInput.y;
        // float turnInput = Input.GetAxis("Horizontal");
        float hInput = gyroInput.x;

        Vector3 move = transform.forward * vInput * moveSpeed * Time.fixedDeltaTime;
        if(rb.transform.position.z + move.z < -40 || rb.transform.position.z + move.z > 20)
        {
            if(rb.transform.position.z + move.z < -40)
            {
                rb.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y, -40);
            }
            if(rb.transform.position.z + move.z > 20)
            {
                rb.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y, 20);
            }
        } else
        {
            rb.MovePosition(rb.position + move);
        }
        

        Vector3 turn = transform.right * hInput * moveSpeed * Time.fixedDeltaTime;
        if (math.abs(rb.position.x + turn.x) > 9){
            if(rb.transform.position.x < 0)
            {
                rb.transform.position = new Vector3(-9, rb.transform.position.y, rb.transform.position.z);
            } else
            {
                rb.transform.position = new Vector3(9, rb.transform.position.y, rb.transform.position.z);
            }
        } else
        {
            rb.MovePosition(rb.position + turn);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Traffic")
        {
            //end game if no shield (or boost)
        }
    }


    //Helper Functions
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
    }

    private static float NormalizeTilt(float angle, float deadZone, float fullTilt)
    {
        float effectiveDeadZone = Mathf.Clamp(deadZone, 0f, Mathf.Max(1f, fullTilt) - 0.01f);
        return Mathf.Sign(angle) * Mathf.InverseLerp(effectiveDeadZone, Mathf.Max(1f, fullTilt), Mathf.Abs(angle));
    }
}