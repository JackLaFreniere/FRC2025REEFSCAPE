using UnityEngine;

public class ReefPole : MonoBehaviour
{
    [Header("Coral Scoring Settings")]
    [SerializeField] private CoralReefLocation coralReefLocation;

    [Header("Movement Speeds")]
    private const float moveSpeed = 10f;
    private const float rotateSpeed = 10f;

    private Transform scoredCoral = null;
    private bool scoredOn = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider is a coral and if it hasn't been scored yet
        if (!other.CompareTag("Coral") || scoredOn) return;

        // Makes sure the Coral is currently being held by a robot. Cannot fall and score itself
        if (other.transform.root == other.transform) return;

        scoredOn = true; // Lets the pole know it has scored a 
        BaseRobot.RemoveCoral(); // Removes the coral from the robot's manipulator

        // Stores the coral locally and parents it to the pole
        scoredCoral = other.transform;
        scoredCoral.SetParent(transform, worldPositionStays: true);

        ScoreManager.AddScore(coralReefLocation.score);
    }

    private void FixedUpdate()
    {
        if (!scoredOn) return; // If the pole has not scored a coral, do nothing

        // Get the transform of the scored coral
        Transform transform = scoredCoral.transform;

        // Smoothly move and rotate the coral onto the pole
        Vector3 targetPosition = Vector3.Lerp(transform.localPosition, coralReefLocation.localPosition, moveSpeed * Time.fixedDeltaTime);
        Quaternion targetRotation = Quaternion.Lerp(transform.localRotation, coralReefLocation.localRotation, rotateSpeed * Time.fixedDeltaTime);
        scoredCoral.transform.SetLocalPositionAndRotation(targetPosition, targetRotation);
    }
}
