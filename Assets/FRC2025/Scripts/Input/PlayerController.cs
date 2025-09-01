using UnityEngine;
using UnityEngine.InputSystem;

namespace FRC2025
{
    public class PlayerController : MonoBehaviour
    {
        public RobotInfo RobotInfo;
        public ToggleCamera DriverStationCamera;
        private IRobotInputHandler _robotActions;
        private GameObject _robotInstance;

        /// <summary>
        /// Initializes the robot instance and its associated components, setting up the necessary input handling and
        /// camera tracking.
        /// </summary>
        /// <remarks>This method is called during the MonoBehaviour's lifecycle to instantiate the robot,
        /// configure its input handling, and link it to the driver station camera. It ensures that the robot is
        /// properly initialized with the provided robot information and input actions.</remarks>
        private void Awake()
        {
            _robotInstance = Instantiate(RobotInfo.robotPrefab, RobotInfo.spawnPosition, Quaternion.Euler(RobotInfo.spawnEuler));

            DriverStationCamera.SetActiveRobot(_robotInstance.GetComponent<BaseRobot>());
            BaseRobot baseRobotScript = _robotInstance.GetComponent<BaseRobot>();
            baseRobotScript.SetRobotInfo(RobotInfo);

            _robotActions = _robotInstance.GetComponent<IRobotInputHandler>();
            this.GetComponent<PlayerInput>().actions = RobotInfo.playerInput;

            _robotActions.SetBaseRobot(_robotInstance.GetComponent<BaseRobot>());
            _robotActions.InputAwake();
        }

        /// <summary>
        /// Enables the input actions for the robot. 
        /// </summary>
        /// <remarks>This method is typically called when the component is enabled to ensure that the
        /// robot's input actions are active and ready for use.</remarks>
        private void OnEnable()
        {
            _robotActions.InputOnEnable();
        }

        /// <summary>
        /// Invoked when the object is disabled. Disables input handling for the associated robot actions.
        /// </summary>
        /// <remarks>This method ensures that input handling is properly deactivated when the object is no
        /// longer active. It is typically called automatically by the Unity engine when the object is
        /// disabled.</remarks>
        private void OnDisable()
        {
            _robotActions.InputOnDisable();
        }
    }
}