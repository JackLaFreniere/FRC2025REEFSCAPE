using UnityEngine;

namespace FRC2025
{
    public class ToggleCamera : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Vector3 _cameraOffset = Vector3.zero;
        [SerializeField] private string _driverStationCameraName = "DriverStationCamera";

        [Header("Robot Settings")]
        [SerializeField] private string _robotDriveBaseName = "Drive_Base";

        public static bool IsRobotCamera { get; private set; } = false;

        private Camera _driverStationCamera;
        private Camera _robotCamera;

        private AudioListener _driverStationAudioListener;
        private AudioListener _robotAudioListener;

        private BaseRobot _baseRobot;

        private Transform _robot;

        /// <summary>
        /// Initializes and configures the camera and audio components for the driver station and robot,  ensuring all
        /// required dependencies are present and properly set up.
        /// </summary>
        /// <remarks>This method locates and validates the driver station camera and robot camera, along
        /// with their  associated audio listeners. It also ensures that the robot's transform is correctly identified 
        /// in the scene. If any required component is missing or invalid, the method disables the script  to prevent
        /// further execution and logs an appropriate error message.</remarks>
        private void Start()
        {
            IsRobotCamera = false;

            // Find and cache the driver station camera
            var driverStationObj = GameObject.Find(_driverStationCameraName);
            if (driverStationObj == null || !driverStationObj.TryGetComponent(out _driverStationCamera))
            {
                Debug.LogError($"{_driverStationCamera} not found or missing Camera component.");
                enabled = false;
                return;
            }

            // Get robot camera from BaseRobot
            _robotCamera = _baseRobot.GetRobotCamera();
            if (_robotCamera == null)
            {
                Debug.LogError("Robot camera not found.");
                enabled = false;
                return;
            }

            // Cache audio listeners
            if (!_driverStationCamera.TryGetComponent(out _driverStationAudioListener))
            {
                Debug.LogError($"{_driverStationCameraName} missing AudioListener.");
                enabled = false;
                return;
            }

            if (!_robotCamera.TryGetComponent(out _robotAudioListener))
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

            var driveBase = baseRobot.transform.Find(_robotDriveBaseName);
            if (driveBase == null)
            {
                Debug.LogError($"{_robotDriveBaseName} not found under BaseRobot.");
                enabled = false;
                return;
            }

            _robot = driveBase;
        }

        /// <summary>
        /// Updates the state of the system by handling camera toggling and refreshing the driver station camera feed.
        /// </summary>
        /// <remarks>This method performs multiple operations to ensure the camera system remains in sync,
        /// including toggling the active camera and updating the driver station's camera display.  It should be called
        /// periodically to maintain the correct camera state.</remarks>
        private void Update()
        {
            HandleCameraToggle();
            UpdateDriverStationCamera();
        }

        /// <summary>
        /// Toggles the active camera and audio listener between the robot and the driver station.
        /// </summary>
        /// <remarks>This method is triggered when the spacebar key is pressed. It switches the active
        /// camera  and audio listener to either the robot or the driver station, depending on the current
        /// state.</remarks>
        private void HandleCameraToggle()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                IsRobotCamera = !IsRobotCamera;

                _driverStationAudioListener.enabled = !IsRobotCamera;
                _driverStationCamera.enabled = !IsRobotCamera;

                _robotAudioListener.enabled = IsRobotCamera;
                _robotCamera.enabled = IsRobotCamera;
            }
        }

        /// <summary>
        /// Updates the orientation of the driver station camera to focus on the robot's position.
        /// </summary>
        /// <remarks>This method adjusts the driver station camera's orientation only if the camera is not
        /// designated  as the robot camera. The camera is rotated to look at the robot's position, offset by a
        /// predefined value.</remarks>
        private void UpdateDriverStationCamera()
        {
            if (!IsRobotCamera)
            {
                Vector3 targetPosition = _robot.position + _cameraOffset;
                _driverStationCamera.transform.LookAt(targetPosition);
            }
        }

        /// <summary>
        /// Sets the active robot to the specified instance.
        /// </summary>
        /// <param name="baseRobot">The robot instance to set as the active robot. Cannot be null.</param>
        public void SetActiveRobot(BaseRobot baseRobot)
        {
            _baseRobot = baseRobot;
        }
    }
}