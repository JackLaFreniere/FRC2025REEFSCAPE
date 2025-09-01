using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FRC2025
{
    [RequireComponent(typeof(BoxCollider))]
    public class BargeZone : ZoneBase
    {

#if UNITY_EDITOR

        [Header("Box Collider Information")]
        [SerializeField] private Vector3 _boxColliderCenter = Vector3.zero;
        [SerializeField] private Vector3 _boxColliderSize = Vector3.one;
        private BoxCollider _boxCollider;

#endif

        private readonly HashSet<Collider> _robotsPreEndgame = new();

        /// <summary>
        /// Handles the event when a collider enters the trigger zone.
        /// </summary>
        /// <remarks>This method checks whether the game is in the endgame phase and processes the
        /// entering object accordingly. If the game is not in the endgame phase, the collider is added to a pre-endgame
        /// tracking list. If the game is in the endgame phase, the method updates the score based on the object's
        /// instance ID and its scoring state.</remarks>
        /// <param name="other">The <see cref="Collider"/> representing the object that entered the trigger zone.</param>
        protected override void OnEnter(Collider other)
        {
            if (!Timer.IsEndgame())
            {
                _robotsPreEndgame.Add(other);
                return;
            }

            int instanceId = other.GetInstanceID();
            int awardedPoints = _robotsInside[instanceId];

            if (awardedPoints == -1)
            {
                ScoreManager.AddScore(_points, _allianceColor);
                _robotsInside[instanceId] = _points;
            }
            else if (awardedPoints == 0)
            {
                ScoreManager.AddScore(_points, _allianceColor);
                _robotsInside[instanceId] = _points;
            }
        }

        /// <summary>
        /// Handles the logic when a collider exits the trigger area.
        /// </summary>
        /// <remarks>If the game is not in the endgame phase, the collider is removed from the pre-endgame
        /// tracking list. During the endgame phase, if the collider corresponds to a robot that was awarded points
        /// equal to the configured value, the points are deducted from the score, and the robot's score is
        /// reset.</remarks>
        /// <param name="other">The <see cref="Collider"/> that exited the trigger area.</param>
        protected override void OnLeave(Collider other)
        {
            if (!Timer.IsEndgame())
            {
                _robotsPreEndgame.Remove(other);
                return;
            }

            int instanceId = other.GetInstanceID();
            int awardedPoints = _robotsInside[instanceId];

            if (awardedPoints == _points)
            {
                ScoreManager.AddScore(-_points, _allianceColor);
                _robotsInside[instanceId] = 0;
            }
        }

        /// <summary>
        /// Executes logic when the endgame phase begins.
        /// </summary>
        /// <remarks>This method is called to handle the transition into the endgame phase. It processes
        /// all colliders in the pre-endgame state by invoking the <see cref="OnEnter"/> method for each
        /// collider.</remarks>
        protected override void OnEndgameStart()
        {
            foreach (Collider other in _robotsPreEndgame)
            {
                OnEnter(other);
            }
        }

        /// <summary>
        /// Handles the automatic end of the object's lifecycle.
        /// </summary>
        /// <remarks>This method is called to perform cleanup when the object's lifecycle ends
        /// automatically. It destroys the current instance to release resources and remove it from the
        /// application.</remarks>
        protected override void OnMatchOver()
        {
            Destroy(this);
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
        /// internal settings when changes are made to the object's properties. It ensures that the collider and other
        /// related settings are properly updated to reflect the current state.</remarks>
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
        /// stored for future use, avoiding repeated calls to <see cref="GetComponent{T}"/>. If the <see
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
        /// <remarks>This method modifies the collider's <see cref="BoxCollider.isTrigger"/>,
        /// <see cref="BoxCollider.center"/>, and <see cref="BoxCollider.size"/> properties. If
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