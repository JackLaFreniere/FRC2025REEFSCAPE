using UnityEngine;

namespace FRC2025
{
    public class DriveTrainSubsystem : Subsystem
    {
        protected WheelCollider[] _wheelColliders;
        protected Vector2 _leftJoystickInput;
        protected Vector2 _rightJoystickInput;

        public override string SubsystemName => "Drive Train";

        public override void ApplyTarget(float value)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsAtTarget()
        {
            throw new System.NotImplementedException();
        }

        public void SetDriveInput(Vector2 leftJoyStick) => _leftJoystickInput = leftJoyStick;

        public void SetRotateInput(Vector2 rightJoyStick) => _rightJoystickInput = rightJoyStick;

        public void SetWheelColliders(WheelCollider[] wheelColiders)
        {
            _wheelColliders = wheelColiders;
        }
    }
}