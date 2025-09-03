using UnityEngine;

#if UNITY_EDITOR
using UnityEditorInternal;
#endif

namespace FRC2025
{
    [RequireComponent(typeof(BoxCollider))]
    public class Processor : ScoreableLocation
    {
        [Header("Box Collider Information")]
        [SerializeField] private Vector3 _boxColliderCenter = Vector3.zero;
        [SerializeField] private Vector3 _boxColliderSize = Vector3.one;

        private readonly int _processorScore = 6;
        private readonly int _netScore = 4;

#if UNITY_EDITOR

        private BoxCollider _boxCollider;

#endif

        /// <summary>
        /// Handles the event when another collider enters the trigger collider attached to this object.
        /// </summary>
        /// <remarks>This method processes the collider if it represents a valid scoring object. If the object is
        /// valid, it triggers the scoring logic and destroys the object.</remarks>
        /// <param name="other">The <see cref="Collider"/> of the object that entered the trigger.</param>
        private void OnTriggerEnter(Collider other)
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
                algae.SetScored();

                ScoreManager.AddScore(_netScore, _allianceColor);
                AllianceColor oppositeAlliance = _allianceColor == AllianceColor.Blue ? AllianceColor.Red : AllianceColor.Blue;
                ScoreManager.AddScore(_processorScore, oppositeAlliance);
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Caches the reference to the BoxCollider component for use in the editor.
        /// </summary>
        /// <remarks>This method is only available in the Unity Editor and is used to ensure the <see
        /// cref="BoxCollider"/> reference is properly cached. It overrides the base implementation to specifically
        /// cache the <see cref="_boxCollider"/> field.</remarks>
        protected override void CacheCollider()
        {
            CacheCollider(ref _boxCollider);
        }

        /// <summary>
        /// Updates the settings of the associated box collider.
        /// </summary>
        /// <remarks>This method ensures that the box collider is configured as a trigger and updates its
        /// center and size based on the current values of <see cref="_boxColliderCenter"/> and <see
        /// cref="_boxColliderSize"/>. If the box collider is not assigned, the method exits without making any
        /// changes.</remarks>
        protected override void UpdateColliderSettings()
        {
            if (_boxCollider == null) return;

            _boxCollider.isTrigger = true;
            _boxCollider.center = _boxColliderCenter;
            _boxCollider.size = _boxColliderSize;
        }

        /// <summary>
        /// Ensures that the editor components are properly configured and arranged.
        /// </summary>
        /// <remarks>This method adjusts the state and order of editor-related components to ensure they
        /// are set up correctly. It operates on the current object and its associated components, such as the box
        /// collider.</remarks>
        protected override void FixEditor()
        {
            HandleEditorComponent(this);
            HandleEditorComponent(_boxCollider, false);
            ComponentUtility.MoveComponentDown(_boxCollider);
        }

#endif

    }
}