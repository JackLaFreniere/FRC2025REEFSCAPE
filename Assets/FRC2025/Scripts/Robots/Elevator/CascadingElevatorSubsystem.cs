using UnityEngine;

namespace FRC2025
{
    public class CascadingElevatorSubsystem : Subsystem
    {
        private GameObject[] _elevatorStages;
        public override string SubsystemName => "Cascading Elevator";

        public override void ApplyTarget(float value)
        {
            if (_elevatorStages == null) return;

            float targetHeight = value/ (_elevatorStages.Length - 1);

            for (int i = 1; i < _elevatorStages.Length; i++)
            {
                _elevatorStages[i].GetOrAddComponent<ConfigurableJoint>().targetPosition = new Vector3(0, targetHeight, 0);
            }
        }

        public override bool IsAtTarget()
        {
            throw new System.NotImplementedException();
        }

        public void SetCascadingElevatorGeneratorAndStages(GameObject[] stages)
        {
            _elevatorStages = stages;
        }
    }
}