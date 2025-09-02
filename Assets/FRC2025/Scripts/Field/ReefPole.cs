using UnityEngine;

namespace FRC2025
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class ReefPole : ScoreableLocation
    {
        [Header("Coral Scoring Settings")]
        [SerializeField] private CoralReefLocation _coralReefLocation;

        [Header("Movement Speeds")]
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _rotateSpeed = 10f;

        private BaseRobot _baseRobot;

        private Transform _scoredCoral = null;
        private bool _scored = false;

        /// <summary>
        /// Handles the event when another collider enters the trigger collider attached to this object.
        /// </summary>
        /// <remarks>This method checks if the entering object is a valid scoring object and ensures the
        /// scoring logic  is executed only once per trigger event. If the object is valid and scoring has not already
        /// occurred,  it invokes the scoring logic.</remarks>
        /// <param name="other">The <see cref="Collider"/> of the object that entered the trigger.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!IsValidScoringObject(other) || _scored) return;

            OnScored(other);
        }

        /// <summary>
        /// Updates the state of the object by moving it to the target position if scoring has occurred.
        /// </summary>
        /// <remarks>This method performs no action if the object has not been scored. Ensure that the
        /// scoring state  is set appropriately before calling this method.</remarks>
        private void Update()
        {
            if (!_scored) return;

            MoveToPosition();
        }

        /// <summary>
        /// When a coral is scored, this method is called to handle the scoring process.
        /// </summary>
        /// <param name="other">The Collider of the scored coral.</param>
        protected override void OnScored(Collider other)
        {
            _baseRobot = RobotHelper.GetBaseRobotScript(other);

            _scored = true; //Lets the pole know it has scored a coral 
            _baseRobot.RemoveCoral(); //Removes the coral from the robot's manipulator

            //Stores the coral locally and parents it to the pole
            _scoredCoral = other.transform;
            _scoredCoral.SetParent(transform, worldPositionStays: true);

            ScoreManager.AddScore(GetScore(), _allianceColor); //Updates the score
        }

        /// <summary>
        /// Determines if the given collider is a valid scoring object (a coral that can be scored).
        /// </summary>
        /// <param name="other">Collider that is being checked.</param>
        /// <returns>Whether the Collider is a valid scorable Coral.</returns>
        protected override bool IsValidScoringObject(Collider other)
        {
            // Makes sure the Coral is currently being held by a robot.
            return other.CompareTag(_scoringElement.tag) && other.transform.root != other.transform;
        }

        /// <summary>
        /// Moves and rotates the Coral to a location on the pole to make it look scored.
        /// </summary>
        public void MoveToPosition()
        {
            // Get the transform of the scored coral
            Transform transform = _scoredCoral.transform;

            // Smoothly move and rotate the coral onto the pole
            Vector3 targetPosition = Vector3.Lerp(transform.localPosition, _coralReefLocation.localPosition, _moveSpeed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.Lerp(transform.localRotation, _coralReefLocation.localRotation, _rotateSpeed * Time.deltaTime);
            _scoredCoral.transform.SetLocalPositionAndRotation(targetPosition, targetRotation);
        }

        /// <summary>
        /// Retrieves the score for the coral reef location, including any auto bonus if applicable.
        /// </summary>
        /// <returns>The point value of scoring on that specific location accounting for any auto bonus points.</returns>
        private int GetScore()
        {
            return _coralReefLocation.score + (Timer.IsAuto() ? _coralReefLocation.autoBonus : 0);
        }
    }
}