using UnityEngine;

public class ToggleCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    private readonly Vector3 cameraOffset = Vector3.zero;
    private const string driverStationCameraName = "DriverStationCamera";

    [Header("Robot Settings")]
    private const string robotDriveBaseName = "Drive_Base";

    public static bool IsRobotCamera { get; private set; } = false;

    private Camera driverStationCamera;
    private Camera robotCamera;

    private AudioListener driverStationAudioListener;
    private AudioListener robotAudioListener;

    private Transform robot;

    private void Start()
    {
        IsRobotCamera = false;

        // Find and cache the driver station camera
        var driverStationObj = GameObject.Find(driverStationCameraName);
        if (driverStationObj == null || !driverStationObj.TryGetComponent(out driverStationCamera))
        {
            Debug.LogError($"{driverStationCamera} not found or missing Camera component.");
            enabled = false;
            return;
        }

        // Get robot camera from BaseRobot
        robotCamera = BaseRobot.GetRobotCamera();
        if (robotCamera == null)
        {
            Debug.LogError("Robot camera not found.");
            enabled = false;
            return;
        }

        // Cache audio listeners
        if (!driverStationCamera.TryGetComponent(out driverStationAudioListener))
        {
            Debug.LogError($"{driverStationCameraName} missing AudioListener.");
            enabled = false;
            return;
        }

        if (!robotCamera.TryGetComponent(out robotAudioListener))
        {
            Debug.LogError("Robot camera missing AudioListener.");
            enabled = false;
            return;
        }

        // Find robot's transform (Drive_Base)
        var baseRobot = FindAnyObjectByType<BaseRobot>();
        if (baseRobot == null)
        {
            Debug.LogError("BaseRobot not found in scene.");
            enabled = false;
            return;
        }

        var driveBase = baseRobot.transform.Find(robotDriveBaseName);
        if (driveBase == null)
        {
            Debug.LogError($"{robotDriveBaseName} not found under BaseRobot.");
            enabled = false;
            return;
        }
        robot = driveBase;
    }

    private void Update()
    {
        HandleCameraToggle();
        UpdateDriverStationCamera();
    }

    /// <summary>
    /// Toggles between the robot camera and the driver station camera when the spacebar is pressed.
    /// </summary>
    private void HandleCameraToggle()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IsRobotCamera = !IsRobotCamera;

            driverStationAudioListener.enabled = !IsRobotCamera;
            driverStationCamera.enabled = !IsRobotCamera;

            robotAudioListener.enabled = IsRobotCamera;
            robotCamera.enabled = IsRobotCamera;
        }
    }

    /// <summary>
    /// Updates the driver station camera to focus on the robot's position if the camera is not set as the robot camera.
    /// </summary>
    private void UpdateDriverStationCamera()
    {
        if (!IsRobotCamera)
        {
            Vector3 targetPosition = robot.position + cameraOffset;
            driverStationCamera.transform.LookAt(targetPosition);
        }
    }
}
