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

        private readonly int score = 4;
        private readonly HashSet<Collider> scoredAlgae = new();

#if UNITY_EDITOR
        private CapsuleCollider _capsuleCollider;
#endif

        private void OnTriggerEnter(Collider other)
        {
            if (!IsValidScoringObject(other)) return;

            scoredAlgae.Add(other);
            OnScored(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!IsValidScoringObject(other)) return;

            if (scoredAlgae.Add(other)) OnScored(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsValidScoringObject(other)) return;

            if (scoredAlgae.Remove(other)) OnUnscored(other);
        }

        /// <summary>
        /// Code to run when an algae is scored.
        /// </summary>
        /// <param name="other">The Algae that is being scored.</param>
        protected override void OnScored(Collider other)
        {
            other.GetComponent<Algae>().Score();

            ScoreManager.AddScore(score, _allianceColor);
        }

        /// <summary>
        /// Code to run when an algae is unscored.
        /// </summary>
        /// <param name="other">The Algae that is being unscored.</param>
        public void OnUnscored(Collider other)
        {
            other.GetComponent<Algae>().Unscore();

            ScoreManager.AddScore(-score, _allianceColor);
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