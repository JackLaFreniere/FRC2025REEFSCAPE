using UnityEngine;

namespace FRC2025
{
    public class TroughHelper : MonoBehaviour
    {
        private Trough trough;

        private void Awake()
        {
            trough = GetComponentInParent<Trough>();
        }

        private void OnTriggerEnter(Collider other)
        {
            trough.OnChildTriggerEnter(other);
        }

        private void OnTriggerStay(Collider other)
        {
            trough.OnChildTriggerStay(other);
        }

        private void OnTriggerExit(Collider other)
        {
            trough.OnChildTriggerExit(other);
        }
    }
}