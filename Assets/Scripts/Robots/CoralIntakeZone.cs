using UnityEngine;

public class CoralIntakeZone : MonoBehaviour
{
    [SerializeField] private Transform intakeTarget;
    [SerializeField] private float intakeForce = 10f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Coral"))
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                Vector3 direction = (intakeTarget.position - other.transform.position).normalized;
                rb.AddForce(direction * intakeForce, ForceMode.Force);
            }
        }
    }
}
