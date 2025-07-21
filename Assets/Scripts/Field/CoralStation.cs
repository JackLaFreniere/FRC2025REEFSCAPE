using UnityEngine;

public class CoralStation : MonoBehaviour
{
    [SerializeField] private GameObject coralPrefab;
    [SerializeField] private Vector3 coralTransform;
    [SerializeField] private Vector3 coralEuler;

    private bool hasDroppedCoral = false;

    private void OnTriggerEnter(Collider other)
    {
        TryDropCoral(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDropCoral(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Robot")) return;
        hasDroppedCoral = false;
    }

    private void TryDropCoral(Collider other)
    {
        // Only drops Coral for robots
        if (!other.CompareTag("Robot")) return;

        if (BaseRobot.stateMachine.CurrentState is MukwonagoBotCoralIntakeState)
        {
            DropCoral(other);
        }
        else if (BaseRobot.stateMachine.CurrentState is Stow)
        {
            hasDroppedCoral = false;
        }
    }

    private void DropCoral(Collider other)
    {
        if (hasDroppedCoral || BaseRobot.hasCoral) return;

        Instantiate(coralPrefab, coralTransform, Quaternion.Euler(coralEuler));
        hasDroppedCoral = true;
    }
}