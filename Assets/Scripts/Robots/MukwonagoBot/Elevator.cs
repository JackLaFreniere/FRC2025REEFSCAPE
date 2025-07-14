using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Elevator : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private BoxCollider boxCollider;
    private LayerMask fieldLayer;

    private Vector3 targetPosition;
    private Vector3 elevatorOffset;

    private const float collisionMargin = 0.01f; // 1 cm

    private void Awake()
    {
        elevatorOffset = transform.localPosition;
    }

    private void Start()
    {
        // Gets the Field physics layer
        fieldLayer = LayerMask.GetMask("Field");

        // Gets the elevator's BoxCollider component
        boxCollider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        // Uses linear interpolation to determine the next y location for the elevator
        Vector3 nextLocalPos = Vector3.Lerp(transform.localPosition, targetPosition, Time.fixedDeltaTime * speed);

        // If the elevator is moving downwards, we don't need to worry about colliding with the field above it
        if (nextLocalPos.y <= transform.localPosition.y)
        {
            UpdatePosition(nextLocalPos);
        }
        else
        {
            CalculateFieldCollision(nextLocalPos);
        }
    }

    /// <summary>
    /// Updates the local position of the object to the specified value.
    /// </summary>
    /// <param name="newPosition">The new position to set, represented as a <see cref="Vector3"/>.</param>
    private void UpdatePosition(Vector3 newPosition)
    {
        transform.localPosition = newPosition;
    }

    /// <summary>
    /// Checks if the elevator's next position would collide with objects in the Field layer.
    /// layer.
    /// </summary>
    /// <param name="nextLocalPos">The local position to which the elevator is attempting to move.</param>
    private void CalculateFieldCollision(Vector3 nextLocalPos)
    {
        // Calculate world position of collider center at next position
        Vector3 colliderCenter = transform.parent.TransformPoint(nextLocalPos + boxCollider.center + new Vector3(0f, collisionMargin, 0f));
        Vector3 colliderSize = Vector3.Scale(boxCollider.size, transform.parent.lossyScale);

        // The elevator should only stop for static field objects, so we check how many objects it collides with that are in the fieldLayer
        Collider[] hits = Physics.OverlapBox(colliderCenter, colliderSize * 0.5f, Quaternion.identity, fieldLayer);

        // If no collisions are detected, move the elevator to the next position
        if (hits.Length == 0) UpdatePosition(nextLocalPos);
    }

    /// <summary>
    /// Sets the target position of the elevator based on the specified height.
    /// </summary>
    /// <param name="height">The desired height in inches. Must be a positive value.</param>
    public void SetTargetPosition(float height)
    {
        targetPosition = new Vector3(elevatorOffset.x, InchesToMeters(height) + elevatorOffset.y, elevatorOffset.z);
    }

    /// <summary>
    /// Converts a measurement in inches to meters.
    /// </summary>
    /// <param name="inches">The measurement in inches to be converted. Must be a non-negative value.</param>
    /// <returns>The equivalent measurement in meters.</returns>
    private float InchesToMeters(float inches)
    {
        return inches * 0.0254f;
    }
}