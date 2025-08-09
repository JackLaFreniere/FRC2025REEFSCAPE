using UnityEngine;

public class CoralIntakeZone : MonoBehaviour
{
    [Header("Intake Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotateSpeed = 100f;

    [Header("Eject Settings")]
    [SerializeField] private float ejectForce = 100f;

    private Vector3 ejectDirection = Vector3.up;

    private void FixedUpdate()
    {
        Collider coralCollider = BaseRobot.coral;

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
        if (!other.CompareTag("Coral") || BaseRobot.coral != null) return;

        // Makes sure that you can only intake Coral when the robot is in the Coral Intake State
        if (BaseRobot.stateMachine.CurrentState is not MukwonagoBotCoralIntakeState) return;

        // If the coral is already scored, do not intake it
        if (other.GetComponent<Coral>().GetIsScored()) return;

        BaseRobot.hasCoral = true;

        // Parents the coral under the wrist's intakezone
        Transform coral = other.transform;
        coral.SetParent(transform, worldPositionStays: true);

        // Disables the coral's physics to animate it smoothly
        Rigidbody rb = coral.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Stores the coral in the intake zone
        BaseRobot.coral = other;
    }

    public void EjectCoral()
    {
        if (BaseRobot.coral == null) return;

        BaseRobot.coral.transform.parent = null;

        Rigidbody coralRB = BaseRobot.coral.GetComponent<Rigidbody>();
        coralRB.useGravity = true;
        coralRB.constraints = RigidbodyConstraints.None;
        coralRB.AddRelativeForce(ejectDirection * ejectForce, ForceMode.Impulse);

        BaseRobot.RemoveCoral();
    }
}
