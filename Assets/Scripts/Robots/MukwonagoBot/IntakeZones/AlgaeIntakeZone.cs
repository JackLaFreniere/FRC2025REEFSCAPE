using UnityEngine;

public class AlgaeIntakeZone : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotateSpeed = 100f;

    private void FixedUpdate()
    {
        Collider algaeCollider = BaseRobot.algae;

        if (algaeCollider == null) return;

        Transform transform = algaeCollider.transform;

        // Smoothly move and rotate the algae towards the Algae EE's intake zone
        Vector3 targetPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, moveSpeed * Time.fixedDeltaTime);
        Quaternion targetRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, rotateSpeed * Time.fixedDeltaTime);
        algaeCollider.transform.SetLocalPositionAndRotation(targetPosition, targetRotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ensures that only a algae is accepted when the intake zone is empty
        if (!other.CompareTag("Algae") || BaseRobot.algae != null) return;

        // If the algae is already scored, do not intake it
        if (other.GetComponent<Algae>().GetIsScored()) return;

        BaseRobot.hasAlgae = true;

        // Parents the algae under the wrist's intakezone
        Transform algae = other.transform;
        algae.SetParent(transform, worldPositionStays: true);

        // Disables the algae's physics to animate it smoothly
        Rigidbody rb = algae.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Stores the algae in the intake zone
        BaseRobot.algae = other;
    }

    public void EjectAlgae()
    {
        if (BaseRobot.algae == null) return;

        BaseRobot.algae.transform.parent = null;
        BaseRobot.algae.GetComponent<Rigidbody>().useGravity = true;
        BaseRobot.algae.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        BaseRobot.RemoveAlgae();
    }
}
