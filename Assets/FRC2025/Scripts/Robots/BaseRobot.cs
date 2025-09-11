using System.Collections.Generic;
using UnityEngine;

namespace FRC2025
{
    public class BaseRobot : MonoBehaviour
    {
        [Header("Robot Settings")]
        public AllianceColor AllianceColor;

        private Dictionary<string, Subsystem> subsystems = new();

        private void Awake()
        {

            foreach (var subsystem in GetComponentsInChildren<Subsystem>())
                subsystems[subsystem.SubsystemName] = subsystem;

            var robotHandler = GetComponent<IRobotInputHandler>();
            robotHandler.SetBaseRobot(this);
            robotHandler.InputAwake();
        }

        public void ApplyPose(RobotPose pose)
        {
            foreach (var target in pose.targets)
            {
                if (subsystems.TryGetValue(target.subsystemName, out var subsystem))
                    subsystem.ApplyTarget(target.targetValue);
            }
        }

        public bool AllAtTarget()
        {
            foreach (var s in subsystems.Values)
                if (!s.IsAtTarget()) return false;
            return true;
        }

        public T GetSubsystem<T>() where T : Subsystem
        {
            foreach (var s in subsystems.Values)
                if (s is T t) return t;
            return null;
        }

        private void OnEnable() => GetComponent<IRobotInputHandler>().InputOnEnable();
        private void OnDisable() => GetComponent<IRobotInputHandler>().InputOnDisable();
    }
}