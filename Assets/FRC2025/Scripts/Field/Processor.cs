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
                algae.Score();

                ScoreManager.AddScore(_netScore, _allianceColor);
                AllianceColor oppositeAlliance = _allianceColor == AllianceColor.Blue ? AllianceColor.Red : AllianceColor.Blue;
                ScoreManager.AddScore(_processorScore, oppositeAlliance);
            }
        }

#if UNITY_EDITOR
        protected override void CacheCollider()
        {
            CacheCollider(ref _boxCollider);
        }

        protected override void UpdateColliderSettings()
        {
            if (_boxCollider == null) return;

            _boxCollider.isTrigger = true;
            _boxCollider.center = BoxColliderCenter;
            _boxCollider.size = BoxColliderSize;
        }

        protected override void FixEditor()
        {
            HandleEditorComponent(this);
            HandleEditorComponent(_boxCollider, false);
            ComponentUtility.MoveComponentDown(_boxCollider);
        }
#endif
    }
}