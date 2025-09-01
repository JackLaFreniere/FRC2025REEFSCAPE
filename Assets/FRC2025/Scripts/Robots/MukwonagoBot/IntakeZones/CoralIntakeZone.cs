using UnityEngine;

namespace FRC2025
{
    public class CoralIntakeZone : MonoBehaviour
    {
        [Header("Intake Settings")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float rotateSpeed = 100f;

        [Header("Eject Settings")]
        [SerializeField] private float ejectForce = 100f;

        private BaseRobot _baseRobot;

        private Vector3 ejectDirection = Vector3.up;

        private void Start()
        {
            _baseRobot = RobotHelper.GetBaseRobotScript(gameObject);
        }

        private void FixedUpdate()
        {
            Collider coralCollider = _baseRobot.coral;

            if (coralCollider == null) return;

            Transform transform = coralCollider.transform;

            // Smoothly move and rotate the coral towards the wrist's intake zone
            Vector3 targetPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, moveSpeed * Time.fixedDeltaTime);
            Quaternion targetRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, rotateSpeed * Time.fixedDeltaTime);
            coralCollider.transform.SetLocalPositionAndRotation(targetPosition, targetRotation);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Ensures that only a coral is accepted when the intake zone is empty
            if (!other.CompareTag("Coral") || _baseRobot.coral != null) return;

            // Makes sure that you can only intake Coral when the robot is in the Coral Intake State
            if (BaseRobot.stateMachine.CurrentState is not MukwonagoBotCoralIntakeState) return;

            // If the coral is already scored, do not intake it
            if (other.GetComponent<Coral>().ScoredInAuto) return;

            _baseRobot.hasCoral = true;

            // Parents the coral under the wrist's intakezone
            Transform coral = other.transform;
            coral.SetParent(transform, worldPositionStays: true);

            // Disables the coral's physics to animate it smoothly
            Rigidbody rb = coral.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            // Stores the coral in the intake zone
            _baseRobot.coral = other;

            _baseRobot.coral.GetComponent<Coral>().SetInRobot(true);
        }

        public void EjectCoral()
        {
            if (_baseRobot.coral == null) return;

            _baseRobot.coral.GetComponent<Coral>().SetInRobot(false);
            _baseRobot.coral.transform.parent = null;

            Rigidbody coralRB = _baseRobot.coral.GetComponent<Rigidbody>();
            coralRB.useGravity = true;
            coralRB.constraints = RigidbodyConstraints.None;
            coralRB.AddRelativeForce(ejectDirection * ejectForce, ForceMode.Impulse);

            _baseRobot.RemoveCoral();
        }
    }
}