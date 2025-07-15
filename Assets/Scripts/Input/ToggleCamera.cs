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
        // Get instances of the cameras and audio listeners
        driverStationCamera = GameObject.Find("DriverStationCamera").GetComponent<Camera>();
        robotCamera = BaseRobot.GetRobotCamera();
        robotCamera.enabled = false; // The Camera has to start enabled to get an instance before being disabled

        driverStationAudioListener = driverStationCamera.GetComponent<AudioListener>();
        robotAudioListener = robotCamera.GetComponent<AudioListener>();

        // Verifie that only the driver station camera is active at the start
        driverStationCamera.gameObject.SetActive(true);
        driverStationAudioListener.enabled = true;
        robotCamera.gameObject.SetActive(false);
        robotAudioListener.enabled = false;

        // Get the robot's Transform component
        robot = robotCamera.transform.GetComponentInParent<Transform>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // Toggle the camera's view between the driver station and the robot
            IsRobotCamera = !IsRobotCamera;

            driverStationCamera.gameObject.SetActive(!IsRobotCamera);
            driverStationAudioListener.enabled = !IsRobotCamera;
            driverStationCamera.enabled = !IsRobotCamera;

            robotCamera.gameObject.SetActive(IsRobotCamera);
            robotAudioListener.enabled = IsRobotCamera;
            robotCamera.enabled = IsRobotCamera;
        }

        if (!IsRobotCamera)
        {
            // Update the driver station camera to track the position of the robot
            Vector3 targetPosition = robot.position + cameraOffset;
            driverStationCamera.transform.LookAt(targetPosition);
        }
    }
}
