using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FRC2025
{
    [RequireComponent(typeof(BoxCollider))]
    public class Processor : ScoreableLocation
    {
        [Header("Box Collider Information")]
        [SerializeField] protected Vector3 BoxColliderCenter = Vector3.zero;
        [SerializeField] protected Vector3 BoxColliderSize = Vector3.one;

        private BoxCollider _boxCollider;
        private readonly int _processorScore = 6;
        private readonly int _netScore = 4;

        /// <summary>
        /// Handles the event when another collider enters the trigger collider attached to this object.
        /// </summary>
        /// <remarks>This method processes the collider if it represents a valid scoring object. If the object is
        /// valid,  it triggers the scoring logic and destroys the object.</remarks>
        /// <param name="other">The <see cref="Collider"/> of the object that entered the trigger.</param>
        protected override void OnTriggerEnter(Collider other)
        {
            if (!IsValidScoringObject(other)) return;

            OnScored(other);

            Destroy(other.gameObject);
        }

        /// <summary>
        /// Handles scoring logic when a collision with a specific object occurs.
        /// </summary>
        /// <remarks>This method updates the score for the current alliance and the opposing alliance based on the
        /// type of object collided with. The scoring logic is triggered only if the collider is associated with an <see
        /// cref="Algae"/> component.</remarks>
        /// <param name="other">The <see cref="Collider"/> involved in the collision. If the collider belongs to an <see cref="Algae"/> object,
        /// scoring actions are performed.</param>
        protected override void OnScored(Collider other)
        {
            if (other.TryGetComponent<Algae>(out var algae))
            {
                algae.Score();

                ScoreManager.AddScore(_processorScore, allianceColor);
                AllianceColor oppositeAlliance = allianceColor == AllianceColor.Blue ? AllianceColor.Red : AllianceColor.Blue;
                ScoreManager.AddScore(_netScore, oppositeAlliance);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Resets the editor to its initial state.
        /// </summary>
        /// <remarks>This method reinitializes the editor setup, restoring any default configurations or
        /// settings.</remarks>
        private void Reset()
        {
            EditorSetupt();
        }

        /// <summary>
        /// Called by the Unity Editor to validate the object's state when changes are made in the Inspector.
        /// </summary>
        /// <remarks>This method is invoked automatically by Unity when the object is modified in the Inspector. 
        /// Use this method to ensure that the object's state remains consistent and to perform any necessary  setup or
        /// validation logic.</remarks>
        private void OnValidate()
        {
            EditorSetupt();
        }

        /// <summary>
        /// Prepares the editor environment by configuring collider settings and applying necessary adjustments.
        /// </summary>
        /// <remarks>This method ensures that the editor is properly set up by caching the collider, updating its
        /// settings,  and performing additional fixes specific to the editor environment. It is intended to be used during 
        /// editor setup workflows.</remarks>
        private void EditorSetupt()
        {
            CacheCollider();
            UpdateColliderSettings();
            FixEditor();
        }

        /// <summary>
        /// Ensures that the <see cref="BoxCollider"/> component is cached for future use.
        /// </summary>
        /// <remarks>If the <see cref="BoxCollider"/> component is not already cached, this method retrieves it 
        /// from the current GameObject and stores it for subsequent operations. This method is typically  called internally
        /// to optimize access to the collider.</remarks>
        protected override void CacheCollider()
        {
            if (_boxCollider == null)
            {
                _boxCollider = GetComponent<BoxCollider>();
            }
        }

        /// <summary>
        /// Updates the settings of the box collider to ensure it matches the configured center, size, and trigger state.
        /// </summary>
        /// <remarks>This method sets the box collider to be a trigger and updates its center and size based on
        /// the current configuration. It has no effect if the box collider is not initialized.</remarks>
        protected override void UpdateColliderSettings()
        {
            if (_boxCollider == null) return;

            _boxCollider.isTrigger = true;
            _boxCollider.center = BoxColliderCenter;
            _boxCollider.size = BoxColliderSize;
        }

        /// <summary>
        /// Ensures the editor state of the current object and its associated components is properly configured.
        /// </summary>
        /// <remarks>This method adjusts the inspector expansion state for the current object and its associated 
        /// <see cref="_boxCollider"/> component. It marks the <see cref="_boxCollider"/> as dirty to ensure  changes are
        /// saved and moves the component down in the component hierarchy.</remarks>
        protected override void FixEditor()
        {
            InternalEditorUtility.SetIsInspectorExpanded(this, true);
            InternalEditorUtility.SetIsInspectorExpanded(_boxCollider, false);

            EditorUtility.SetDirty(_boxCollider);
            ComponentUtility.MoveComponentDown(_boxCollider);
        }
#endif
    }
}