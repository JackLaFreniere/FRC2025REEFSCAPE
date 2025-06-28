using UnityEngine;

public class Climber : MonoBehaviour
{
    private Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);
    [SerializeField] private float rotationSpeed = 2f;
    private readonly float climberOffset = 22f;

    private void FixedUpdate()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
    }

    public void SetTargetRotation(float targetAngle)
    {
        targetRotation = Quaternion.Euler(-targetAngle - climberOffset, 0f, 0f);
    }
}
