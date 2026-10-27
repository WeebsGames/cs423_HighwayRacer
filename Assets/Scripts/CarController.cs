using UnityEngine;

public class CarController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;

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
        float moveInput = Input.GetAxis("Vertical");
        // float turnInput = Input.GetAxis("Horizontal");
        float hInput = gyroInput.x;

        Vector3 move = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        // rb.MovePosition(rb.position + move);

        Vector3 turn = transform.right * hInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + turn);
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
}