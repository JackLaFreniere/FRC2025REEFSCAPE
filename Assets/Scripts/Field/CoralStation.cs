using UnityEngine;

public class CoralStation : MonoBehaviour
{
    [SerializeField] private GameObject coralPrefab;
    [SerializeField] private Vector3 coralTransform;
    [SerializeField] private Vector3 coralEuler;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Robot"))
        {
            Instantiate(coralPrefab, coralTransform, Quaternion.Euler(coralEuler));
        }
    }
}
