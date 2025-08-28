using UnityEngine;
using UnityEngine.InputSystem;

namespace FRC2025
{
    public class PlayerController : MonoBehaviour
    {
        public RobotInfo robotInfo;
        public ToggleCamera driverStationCamera;
        private IRobotInputHandler robotActions;
        private GameObject robotInstance;

        private void Awake()
        {
            robotInstance = Instantiate(robotInfo.robotPrefab, robotInfo.spawnPosition, Quaternion.Euler(robotInfo.spawnEuler));

            driverStationCamera.SetActiveRobot(robotInstance.GetComponent<BaseRobot>());
            BaseRobot baseRobotScript = robotInstance.GetComponent<BaseRobot>();
            baseRobotScript.SetRobotInfo(robotInfo);

            robotActions = robotInstance.GetComponent<IRobotInputHandler>();
            this.GetComponent<PlayerInput>().actions = robotInfo.playerInput;

            robotActions.SetBaseRobot(robotInstance.GetComponent<BaseRobot>());
            robotActions.InputAwake();
        }

        private void OnEnable()
        {
            robotActions.InputOnEnable();
        }

        private void OnDisable()
        {
            robotActions.InputOnDisable();
        }
    }
}