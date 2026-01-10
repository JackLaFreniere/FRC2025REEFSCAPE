using UnityEngine;

namespace FRC2025
{
    public class ArmSubsystem : Subsystem
    {
        private GameObject _arm;
        private AxisDirection _axis;
        public override void ApplyTarget(float value)
        {
            if (_arm == null) return;

            Quaternion targetRotation = Quaternion.Euler(
                _axis == AxisDirection.X ? value : 0,
                _axis == AxisDirection.Y ? value : 0,
                _axis == AxisDirection.Z ? value : 0
            );

            _arm.GetComponent<ConfigurableJoint>().targetRotation = targetRotation;
        }

        public override bool IsAtTarget()
        {
            throw new System.NotImplementedException();
        }

        public void SetArmGameObjects(GameObject arm, AxisDirection axis)
        {
            _arm = arm;
            _axis = axis;
        }
    }
}