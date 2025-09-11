using System.Collections.Generic;
using UnityEngine;

namespace FRC2025
{
    public class BaseRobot : MonoBehaviour
    {
        [Header("Robot Settings")]
        public AllianceColor AllianceColor;

        private readonly Dictionary<string, Subsystem> _subsystems = new();
        private IRobotInputHandler _robotActions;

        protected void Awake()
        {
            foreach (var subsystem in GetComponentsInChildren<Subsystem>())
                _subsystems[subsystem.SubsystemName] = subsystem;

            _robotActions = this.GetComponent<IRobotInputHandler>();
            _robotActions.SetBaseRobot(this);
            _robotActions.InputAwake();
        }

        public void ApplyPose(RobotPose pose)
        {
            foreach (var target in pose.targets)
            {
                if (_subsystems.TryGetValue(target.subsystemName, out var subsystem))
                    subsystem.ApplyTarget(target.targetValue);
            }
        }

        public bool AllAtTarget()
        {
            foreach (var s in _subsystems.Values)
                if (!s.IsAtTarget()) return false;
            return true;
        }

        public T GetSubsystem<T>() where T : Subsystem
        {
            foreach (var s in _subsystems.Values)
                if (s is T t) return t;
            return null;
        }

        private void OnEnable() => _robotActions.InputOnEnable();
        private void OnDisable() => _robotActions.InputOnDisable();
    }
}