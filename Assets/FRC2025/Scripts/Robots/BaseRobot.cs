using System.Collections.Generic;
using UnityEngine;

namespace FRC2025
{
    public class BaseRobot : MonoBehaviour
    {
        [Header("Robot Settings")]
        public AllianceColor AllianceColor;

        // Map from GameObject name -> list of Subsystem instances that currently have that name.
        // Using a list avoids silent collisions when multiple objects share the same name.
        private readonly Dictionary<string, List<Subsystem>> _subsystems = new();

        private IRobotInputHandler _robotActions;

        protected void Awake()
        {
            RebuildSubsystemMap();

            _robotActions = this.GetComponent<IRobotInputHandler>();
            if (_robotActions != null)
            {
                _robotActions.SetBaseRobot(this);
                _robotActions.InputAwake();
            }
        }

        // Call to rebuild the name -> subsystem list map (safe and cheap compared to doing string lookups repeatedly).
        private void RebuildSubsystemMap()
        {
            _subsystems.Clear();

            foreach (Subsystem subsystem in GetComponentsInChildren<Subsystem>())
            {
                string key = subsystem.gameObject.name ?? string.Empty;

                if (!_subsystems.TryGetValue(key, out var list))
                {
                    list = new List<Subsystem>();
                    _subsystems[key] = list;
                }

                list.Add(subsystem);
                //Debug.Log($"Registered subsystem key='{key}' (type={subsystem.GetType().Name}, go='{subsystem.gameObject.name}').");
            }
        }

        public void ApplyPose(RobotPose pose)
        {
            foreach (var target in pose.targets)
            {
                Debug.Log("Target: " + target.subsystemName + " -> " + target.targetValue);
                foreach (Subsystem subsystem in GetComponentsInChildren<Subsystem>())
                {
                    if (subsystem.gameObject.name != target.subsystemName) continue;
                    //Debug.Log();
                    Debug.Log(subsystem);
                    subsystem.ApplyTarget(target.targetValue);
                }
            }


            //foreach (var target in pose.targets)
            //{
            //    foreach(Subsystem subsystem in GetComponentsInChildren<Subsystem>())
            //    {
            //        if (subsystem.gameObject.name == target.subsystemName)
            //        {
            //            Debug.Log("Applying target " + target.targetValue + " to " + subsystem.gameObject.name);
            //            subsystem.ApplyTarget(target.targetValue);
            //        }
            //    }
            //if (_subsystems.TryGetValue(target.subsystemName, out var matches))
            //{
            //    // Apply the target to every subsystem with the matching GameObject name.
            //    // This is deliberate: if multiple subsystems share the same name you probably
            //    // want all of them affected, or at least to be warned about the collision.
            //    foreach (var subsystem in matches)
            //    {
            //        subsystem.ApplyTarget(target.targetValue);
            //        Debug.Log($"{target.subsystemName} target applied: {target.targetValue} -> {subsystem.gameObject.name}");
            //    }
            //}
            //else
            //{
            //    Debug.LogWarning($"No subsystem found for key '{target.subsystemName}'. Available keys: {string.Join(", ", _subsystems.Keys)}");
            //}
            //}
        }

        public bool AllAtTarget()
        {
            foreach (var list in _subsystems.Values)
            {
                foreach (var s in list)
                {
                    if (!s.IsAtTarget()) return false;
                }
            }

            return true;
        }

        public T GetSubsystem<T>() where T : Subsystem
        {
            foreach (var list in _subsystems.Values)
            {
                foreach (var s in list)
                {
                    if (s is T t) return t;
                }
            }
            return null;
        }

        private void OnEnable()
        {
            // Rebuild map in case names changed in editor before play.
            RebuildSubsystemMap();

            _robotActions?.InputOnEnable();
        }

        private void OnDisable()
        {
            _robotActions?.InputOnDisable();
        }
    }
}