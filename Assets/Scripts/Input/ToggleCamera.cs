using UnityEngine;

public class ToggleCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Vector3 cameraOffset = Vector3.zero;

    public static bool IsRobotCamera = false;

    private Camera driverStationCamera;
    private Camera robotCamera;

    private AudioListener driverStationAudioListener;
    private AudioListener robotAudioListener;

    private Transform robot;

    private void Start()
    {
        IsRobotCamera = false;

        // Get instances of the cameras and audio listeners
        driverStationCamera = GameObject.Find("DriverStationCamera").GetComponent<Camera>();
        robotCamera = BaseRobot.GetRobotCamera();

        driverStationAudioListener = driverStationCamera.GetComponent<AudioListener>();
        robotAudioListener = robotCamera.GetComponent<AudioListener>();

        // Get the robot's Transform component
        robot = GameObject.FindAnyObjectByType<BaseRobot>().transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Toggle the camera's view between the driver station and the robot
            IsRobotCamera = !IsRobotCamera;

            driverStationAudioListener.enabled = !IsRobotCamera;
            driverStationCamera.enabled = !IsRobotCamera;

            robotAudioListener.enabled = IsRobotCamera;
            robotCamera.enabled = IsRobotCamera;
        }

        if (!IsRobotCamera)
        {
            // Update the driver station camera to track the position of the robot
            Vector3 targetPosition = robot.position;// robot.TransformPoint(cameraOffset);
            driverStationCamera.transform.LookAt(targetPosition);
        }
    }
}
