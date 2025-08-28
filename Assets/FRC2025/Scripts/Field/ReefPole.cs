using UnityEngine;

namespace FRC2025
{
    public class ReefPole : ScoreableLocation
    {
        [Header("Coral Scoring Settings")]
        [SerializeField] private CoralReefLocation coralReefLocation;

        [Header("Movement Speeds")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float rotateSpeed = 10f;

        private BaseRobot _baseRobot;

        private Transform scoredCoral = null;
        private bool scored = false;

        private void Start()
        {
            _baseRobot = RobotHelper.GetBaseRobotScript(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsValidScoringObject(other) || scored) return;

            OnScored(other);
        }

        private void Update()
        {
            if (!scored) return;

            MoveToPosition();
        }

        /// <summary>
        /// When a coral is scored, this method is called to handle the scoring process.
        /// </summary>
        /// <param name="other">The Collider of the scored coral.</param>
        public override void OnScored(Collider other)
        {
            scored = true; //Lets the pole know it has scored a coral 
            _baseRobot.RemoveCoral(); //Removes the coral from the robot's manipulator

            //Stores the coral locally and parents it to the pole
            scoredCoral = other.transform;
            scoredCoral.SetParent(transform, worldPositionStays: true);

            ScoreManager.AddScore(GetScore(), allianceColor); //Updates the score
        }

        /// <summary>
        /// Determines if the given collider is a valid scoring object (a coral that can be scored).
        /// </summary>
        /// <param name="other">Collider that is being checked.</param>
        /// <returns>Whether the Collider is a valid scorable Coral.</returns>
        public override bool IsValidScoringObject(Collider other)
        {
            // Makes sure the Coral is currently being held by a robot.
            return other.CompareTag(scoringElementTag) && other.transform.root != other.transform;
        }

        /// <summary>
        /// Moves and rotates the Coral to a location on the pole to make it look scored.
        /// </summary>
        public void MoveToPosition()
        {
            // Get the transform of the scored coral
            Transform transform = scoredCoral.transform;

            // Smoothly move and rotate the coral onto the pole
            Vector3 targetPosition = Vector3.Lerp(transform.localPosition, coralReefLocation.localPosition, moveSpeed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.Lerp(transform.localRotation, coralReefLocation.localRotation, rotateSpeed * Time.deltaTime);
            scoredCoral.transform.SetLocalPositionAndRotation(targetPosition, targetRotation);
        }

        /// <summary>
        /// Retrieves the score for the coral reef location, including any auto bonus if applicable.
        /// </summary>
        /// <returns>The point value of scoring on that specific location accounting for any auto bonus points.</returns>
        private int GetScore()
        {
            return coralReefLocation.score + (Timer.IsAuto() ? coralReefLocation.autoBonus : 0);
        }
    }
}