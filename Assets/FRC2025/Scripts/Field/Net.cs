using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace FRC2025
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class Net : ScoreableLocation
    {
        [Header("Capsule Collider Settings")]
        [SerializeField] private AxisDirection _axisDirection = AxisDirection.Y;
        [SerializeField] private Vector3 _capsuleColliderCenter = Vector3.zero;
        [SerializeField] private float _capsuleColliderRadius = 0.5f;
        [SerializeField] private float _capsuleColliderHeight = 1f;

        private readonly int _score = 4;
        private readonly HashSet<Collider> _scoredAlgae = new();

#if UNITY_EDITOR
        private CapsuleCollider _capsuleCollider;
#endif

        /// <summary>
        /// Handles the event when another collider enters the trigger collider attached to this object.
        /// </summary>
        /// <remarks>If the collider is determined to be a valid scoring object, it is added to the
        /// internal collection of scored objects,  and the scoring logic is triggered. Invalid objects are
        /// ignored.</remarks>
        /// <param name="other">The <see cref="Collider"/> that entered the trigger. This parameter represents the object being evaluated
        /// for scoring.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!IsValidScoringObject(other)) return;

            _scoredAlgae.Add(other);
            OnScored(other);
        }

        /// <summary>
        /// Called once per frame for every Collider that remains within the trigger.
        /// </summary>
        /// <remarks>This method checks if the specified <paramref name="other"/> is a valid scoring
        /// object.  If the object is valid and has not been scored yet, it is added to the scored objects  collection,
        /// and the scoring logic is triggered.</remarks>
        /// <param name="other">The <see cref="Collider"/> that is currently within the trigger.</param>
        private void OnTriggerStay(Collider other)
        {
            if (!IsValidScoringObject(other)) return;

            if (_scoredAlgae.Add(other)) OnScored(other);
        }

        /// <summary>
        /// Handles the event when a collider exits the trigger area.
        /// </summary>
        /// <remarks>This method checks if the exiting collider is a valid scoring object. If it is, the
        /// object is removed  from the scored list, and the corresponding unscoring logic is triggered.</remarks>
        /// <param name="other">The <see cref="Collider"/> that exited the trigger area.</param>
        private void OnTriggerExit(Collider other)
        {
            if (!IsValidScoringObject(other)) return;

            if (_scoredAlgae.Remove(other)) OnUnscored(other);
        }

        /// <summary>
        /// Code to run when an algae is scored.
        /// </summary>
        /// <param name="other">The Algae that is being scored.</param>
        protected override void OnScored(Collider other)
        {
            other.GetComponent<Algae>().SetScored();

            ScoreManager.AddScore(_score, _allianceColor);
        }

        /// <summary>
        /// Code to run when an algae is unscored.
        /// </summary>
        /// <param name="other">The Algae that is being unscored.</param>
        public void OnUnscored(Collider other)
        {
            other.GetComponent<Algae>().SetScored(false);

            ScoreManager.AddScore(-_score, _allianceColor);
        }

#if UNITY_EDITOR

        /// <summary>
        /// Caches the reference to the capsule collider used by this component.
        /// </summary>
        /// <remarks>This method ensures that the capsule collider reference is cached for efficient
        /// access. It is intended to be used in the Unity Editor environment.</remarks>
        protected override void CacheCollider()
        {
            CacheCollider(ref _capsuleCollider);
        }

        /// <summary>
        /// Ensures that the editor components are properly configured and adjusted.
        /// </summary>
        /// <remarks>This method modifies the state of the editor components associated with the current
        /// object to ensure they are correctly set up. It may adjust the order or properties of components as needed.
        /// This method is intended to be called in scenarios where editor-specific adjustments are required.</remarks>
        protected override void FixEditor()
        {
            HandleEditorComponent(this);
            HandleEditorComponent(_capsuleCollider, false);
            ComponentUtility.MoveComponentDown(_capsuleCollider);
        }

        /// <summary>
        /// Updates the settings of the capsule collider to reflect the current configuration.
        /// </summary>
        /// <remarks>This method ensures that the capsule collider is configured with the specified
        /// center, radius, height, and axis direction. The collider is also set to trigger mode. If the capsule
        /// collider is null, the method exits without making any changes.</remarks>
        protected override void UpdateColliderSettings()
        {
            if (_capsuleCollider == null) return;

            _capsuleCollider.isTrigger = true;
            _capsuleCollider.center = _capsuleColliderCenter;
            _capsuleCollider.radius = _capsuleColliderRadius;
            _capsuleCollider.height = _capsuleColliderHeight;
            _capsuleCollider.direction = (int)_axisDirection;
        }

#endif

    }
}