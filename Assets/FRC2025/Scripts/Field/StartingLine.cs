using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FRC2025
{
    [RequireComponent(typeof(BoxCollider))]
    public class StartingLine : MonoBehaviour
    {
        [Header("Alliance Color")]
        [SerializeField] private AllianceColor _allianceColor;

        [Header("Box Collider Information")]
        [SerializeField] private Vector3 _boxColliderCenter = Vector3.zero;
        [SerializeField] private Vector3 _boxColliderSize = Vector3.one;

        private BoxCollider _boxCollider;

        private const int mobilityPoints = 3;
        private int mobilityPointsAwarded = 0;

        private bool hasAutoEnded = false;

        /// <summary>
        /// Handles the event when a collider enters the trigger zone.
        /// </summary>
        /// <remarks>This method checks if the collider belongs to a robot, whether the timer is in auto
        /// mode, and if mobility points have been awarded. If all conditions are met and the robot's alliance color
        /// matches the current alliance, mobility points are deducted from the score.</remarks>
        /// <param name="other">The <see cref="Collider"/> that entered the trigger zone.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!RobotHelper.IsRobot(other)) return;

            if (!Timer.IsAuto() || hasAutoEnded) return;

            if (mobilityPointsAwarded == 0) return;

            if (RobotHelper.GetBaseRobotScript(other).allianceColor != _allianceColor) return;

            ScoreManager.AddScore(-mobilityPoints, _allianceColor);
        }

        /// <summary>
        /// Handles the event when a collider exits the trigger area, awarding mobility points to the appropriate
        /// alliance if certain conditions are met.
        /// </summary>
        /// <remarks>This method checks if the collider belongs to a robot, whether the timer is in auto
        /// mode, and if the robot's alliance matches the current alliance. If all conditions are satisfied, mobility
        /// points are added to the score of the corresponding alliance.</remarks>
        /// <param name="other">The <see cref="Collider"/> that exited the trigger area. This is expected to represent a robot.</param>
        private void OnTriggerExit(Collider other)
        {
            if (!RobotHelper.IsRobot(other)) return;

            if (!Timer.IsAuto() || hasAutoEnded) return;

            if (RobotHelper.GetBaseRobotScript(other).allianceColor != _allianceColor) return;

            ScoreManager.AddScore(mobilityPoints, _allianceColor);
            mobilityPointsAwarded += mobilityPoints;
        }

        /// <summary>
        /// Updates the state of the object based on the current timer mode.
        /// </summary>
        /// <remarks>This method checks if the timer is not in auto mode and ensures that the auto-ended
        /// state is updated accordingly.  It is intended to be called as part of the object's internal update
        /// cycle.</remarks>
        private void Update()
        {
            if (!Timer.IsAuto() && !hasAutoEnded)
            {
                hasAutoEnded = true;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Resets the StartingLine component in the Unity Editor.
        /// Ensures the BoxCollider is cached and editor settings are applied for proper inspector display.
        /// </summary>
        private void Reset()
        {
            CacheCollider();
            FixEditor();
        }

        /// <summary>
        /// Validates the current state of the object and ensures that its configuration is consistent.
        /// </summary>
        /// <remarks>This method is typically called in the Unity Editor to update and synchronize
        /// internal settings  when changes are made to the object's properties. It ensures that the collider and other
        /// related  settings are properly updated to reflect the current state.</remarks>
        private void OnValidate()
        {
            CacheCollider();
            UpdateColliderSettings();

            FixEditor();
        }

        /// <summary>
        /// Caches the <see cref="BoxCollider"/> component attached to the current GameObject.
        /// </summary>
        /// <remarks>This method ensures that the <see cref="BoxCollider"/> component is retrieved and
        /// stored  for future use, avoiding repeated calls to <see cref="GetComponent{T}"/>. If the  <see
        /// cref="BoxCollider"/> is already cached, this method does nothing.</remarks>
        private void CacheCollider()
        {
            if (_boxCollider == null)
                _boxCollider = GetComponent<BoxCollider>();
        }

        /// <summary>
        /// Updates the settings of the box collider, ensuring it is configured as a trigger and applies the specified
        /// center and size values.
        /// </summary>
        /// <remarks>This method modifies the collider's <see cref="UnityEngine.BoxCollider.isTrigger"/>,
        /// <see cref="UnityEngine.BoxCollider.center"/>, and <see cref="UnityEngine.BoxCollider.size"/> properties. If
        /// the collider is not initialized, the method exits without making changes.</remarks>
        private void UpdateColliderSettings()
        {
            if (_boxCollider == null) return;

            _boxCollider.isTrigger = true;
            _boxCollider.center = _boxColliderCenter;
            _boxCollider.size = _boxColliderSize;
        }

        /// <summary>
        /// Ensures the script is always expanded and the associated BoxCollider component is collapsed in the Unity
        /// Inspector, while also marking the BoxCollider as modified and adjusting its position in the component list.
        /// </summary>
        /// <remarks>This method is intended for use in the Unity Editor to enforce specific Inspector
        /// behavior for the script and its associated BoxCollider component. It modifies the Inspector's expanded
        /// state, marks the BoxCollider as dirty to reflect changes, and reorders the component list.</remarks>
        private void FixEditor()
        {
            // Makes sure this script is always expanded and the BoxCollider is collapsed in the inspector
            InternalEditorUtility.SetIsInspectorExpanded(this, true);
            InternalEditorUtility.SetIsInspectorExpanded(_boxCollider, false);

            // Makes sure that the BoxCollider is marked as dirty and moved down in the component list
            EditorUtility.SetDirty(_boxCollider);
            ComponentUtility.MoveComponentDown(_boxCollider);
        }
#endif
    }
}