using UnityEngine;

namespace FRC2025
{
    public class CascadingElevatorSubsystem : Subsystem
    {
        private GameObject[] _elevatorStages;
        public override void ApplyTarget(float value)
        {
            if (_elevatorStages == null || _elevatorStages.Length < 2) return;

            float targetHeight = value / (_elevatorStages.Length - 1);
            for (int i = 1; i < _elevatorStages.Length; i++)
            {
                _elevatorStages[i].GetComponent<ConfigurableJoint>().targetPosition = new Vector3(0, targetHeight, 0);
            }
        }

        public override bool IsAtTarget()
        {
            throw new System.NotImplementedException();
        }

        public void SetElevatorStages(GameObject[] stages)
        {
            _elevatorStages = stages;
        }
    }
}