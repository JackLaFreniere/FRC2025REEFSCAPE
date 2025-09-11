using UnityEngine;

namespace FRC2025
{
    public class FRC2025_9999InputHandler : MonoBehaviour, IRobotInputHandler
    {
        private BaseRobot _robot;
        private FRC2025_9999Input _controls;

        public void SetBaseRobot(BaseRobot robot)
        {
            _robot = robot;
        }

        public void InputAwake()
        {
            _controls = new FRC2025_9999Input();
        }

        public void InputOnEnable()
        {
            Debug.Log("Enabled");
            _controls.Enable();

            _controls.Robot.Drive.performed += ctx =>
                _robot.GetSubsystem<DriveTrainSubsystem>().SetDriveInput(ctx.ReadValue<Vector2>());
            _controls.Robot.Drive.canceled += ctx =>
                _robot.GetSubsystem<DriveTrainSubsystem>().SetDriveInput(Vector2.zero);

            _controls.Robot.Rotate.performed += ctx =>
                _robot.GetSubsystem<DriveTrainSubsystem>().SetRotateInput(ctx.ReadValue<Vector2>());
            _controls.Robot.Rotate.canceled += ctx =>
                _robot.GetSubsystem<DriveTrainSubsystem>().SetRotateInput(Vector2.zero);
        }

        public void InputOnDisable() => _controls.Disable();
    }
}