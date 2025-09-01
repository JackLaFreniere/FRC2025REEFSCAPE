using UnityEngine;

namespace FRC2025
{
    public class TroughHelper : MonoBehaviour
    {
        [Header("Trough")]
        [SerializeField] private Trough _trough;

        /// <summary>
        /// Handles the event when another collider enters the trigger collider attached to this object.
        /// </summary>
        /// <param name="other">The <see cref="Collider"/> that entered the trigger. Represents the interacting object.</param>
        private void OnTriggerEnter(Collider other)
        {
            _trough.OnChildTriggerEnter(other);
        }

        /// <summary>
        /// Invoked while another collider remains within the trigger collider attached to this object.
        /// </summary>
        /// <param name="other">The <see cref="Collider"/> that is currently within the trigger collider.</param>
        private void OnTriggerStay(Collider other)
        {
            _trough.OnChildTriggerStay(other);
        }

        /// <summary>
        /// Handles the event when a collider exits the trigger area.
        /// </summary>
        /// <param name="other">The <see cref="Collider"/> that exited the trigger area.</param>
        private void OnTriggerExit(Collider other)
        {
            _trough.OnChildTriggerExit(other);
        }
    }
}