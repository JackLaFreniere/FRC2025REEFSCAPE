using UnityEngine;

public class Wrist : MonoBehaviour
{
    private Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);
    [SerializeField] private float rotationSpeed = 10f;

    private void FixedUpdate()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
    }

    public void SetTargetRotation(float rotation)
    {
        targetRotation = Quaternion.Euler(0f, rotation, 0f);
    }
}
