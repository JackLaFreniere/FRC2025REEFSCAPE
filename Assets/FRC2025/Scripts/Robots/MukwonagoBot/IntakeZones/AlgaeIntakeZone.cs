using UnityEngine;

namespace FRC2025
{
    public class AlgaeIntakeZone : MonoBehaviour
    {
        //[Header("Intake Settings")]
        //[SerializeField] private float moveSpeed = 3f;
        //[SerializeField] private float rotateSpeed = 100f;

        //[Header("Eject Settings")]
        //[SerializeField] private float ejectForce = 100f;

        //private BaseRobot _baseRobot;

        //private Vector3 ejectDirection = Vector3.up;

        //private void Start()
        //{
        //    _baseRobot = RobotHelper.GetBaseRobotScript(gameObject);
        //}

        //private void FixedUpdate()
        //{
        //    Collider algaeCollider = _baseRobot.algae;

        //    if (algaeCollider == null) return;

        //    Transform transform = algaeCollider.transform;

        //    // Smoothly move and rotate the algae towards the Algae EE's intake zone
        //    Vector3 targetPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, moveSpeed * Time.fixedDeltaTime);
        //    Quaternion targetRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, rotateSpeed * Time.fixedDeltaTime);
        //    algaeCollider.transform.SetLocalPositionAndRotation(targetPosition, targetRotation);
        //}

        //private void OnTriggerEnter(Collider other)
        //{
        //    // Ensures that only a algae is accepted when the intake zone is empty
        //    if (!other.CompareTag("Algae") || _baseRobot.algae != null) return;

        //    // Makes sure that you can only intake Algae when the robot is in the Algae Intake State
        //    if (BaseRobot.stateMachine.CurrentState is not MukwonagoBotAlgaeIntakeState) return;

        //    // If the algae is already scored, do not intake it
        //    if (other.GetComponent<Algae>().ScoredInAuto) return;

        //    _baseRobot.hasAlgae = true;

        //    // Parents the algae under the wrist's intakezone
        //    Transform algae = other.transform;
        //    algae.SetParent(transform, worldPositionStays: true);

        //    // Disables the algae's physics to animate it smoothly
        //    Rigidbody rb = algae.GetComponent<Rigidbody>();
        //    rb.useGravity = false;
        //    rb.constraints = RigidbodyConstraints.FreezeAll;

        //    // Stores the algae in the intake zone
        //    _baseRobot.algae = other;
        //}

        //public void EjectAlgae()
        //{
        //    if (_baseRobot.algae == null) return;

        //    _baseRobot.algae.transform.parent = null;

        //    Rigidbody algaeRB = _baseRobot.algae.GetComponent<Rigidbody>();
        //    algaeRB.useGravity = true;
        //    algaeRB.constraints = RigidbodyConstraints.None;
        //    algaeRB.AddRelativeForce(ejectDirection * ejectForce, ForceMode.Impulse);

        //    _baseRobot.RemoveAlgae();
        //}
    }
}