using UnityEngine;

public class Wrist : MonoBehaviour
{
    private Quaternion targetRotation;
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
