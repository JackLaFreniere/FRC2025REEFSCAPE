using System.Collections.Generic;
using UnityEngine;

namespace FRC2025
{
    public abstract class ZoneBase : MonoBehaviour
    {
        [Header("Alliance Color")]
        [SerializeField] protected AllianceColor _allianceColor;
        [SerializeField] protected int _points;

        protected Dictionary<int, int> _robotsInside = new();

        /// <summary>
        /// Subscribes to timer-related events when the object is enabled.
        /// </summary>
        /// <remarks>This method is called automatically when the object is enabled. It attaches event
        /// handlers  to the timer's events, such as <see cref="Timer.MatchStart"/>, <see cref="Timer.AutoEnd"/>,  <see
        /// cref="Timer.MatchOver"/>, and <see cref="Timer.EndgameStart"/>. Override this method in a derived class to
        /// customize the event subscription behavior,  ensuring the base implementation is called to maintain existing
        /// subscriptions.</remarks>
        protected virtual void OnEnable()
        {
            Timer.MatchStart += OnMatchStart;
            Timer.AutoEnd += OnAutoEnd;
            Timer.MatchOver += OnMatchOver;
            Timer.EndgameStart += OnEndgameStart;
        }

        /// <summary>
        /// Called when the object is disabled. Unsubscribes from timer-related events to prevent memory leaks and
        /// unintended behavior.
        /// </summary>
        /// <remarks>This method is invoked automatically by Unity when the object is disabled. It
        /// detaches event handlers from the timer to ensure that the object no longer responds to timer events while
        /// disabled. Derived classes can override this method to include additional cleanup logic, but should call the
        /// base implementation to maintain proper event unsubscription.</remarks>
        protected virtual void OnDisable()
        {
            Timer.MatchStart -= OnMatchStart;
            Timer.AutoEnd -= OnAutoEnd;
            Timer.MatchOver -= OnMatchOver;
            Timer.EndgameStart -= OnEndgameStart;
        }

        /// <summary>
        /// Handles the event when another collider enters the trigger collider attached to this object.
        /// </summary>
        /// <remarks>If the entering object is identified as a robot, it is added to the internal tracking
        /// collection, and additional processing is triggered.</remarks>
        /// <param name="other">The <see cref="Collider"/> of the object that entered the trigger. This parameter is used to identify and
        /// process the entering object.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (RobotHelper.IsRobot(other) && RobotHelper.IsRobotOnAlliance(other, _allianceColor))
            {
                int instanceId = other.GetInstanceID();
                _robotsInside.TryAdd(instanceId, -1);

                OnEnter(other);
            }
        }

        /// <summary>
        /// Handles the event when a collider exits the trigger area.
        /// </summary>
        /// <remarks>If the collider belongs to a robot, its instance ID is added to the tracking
        /// collection  with a placeholder value, and the <c>OnLeave</c> method is invoked.</remarks>
        /// <param name="other">The <see cref="Collider"/> that exited the trigger area.</param>
        private void OnTriggerExit(Collider other)
        {
            if (RobotHelper.IsRobot(other) && RobotHelper.IsRobotOnAlliance(other, _allianceColor))
            {
                int instanceId = other.GetInstanceID();
                _robotsInside.TryAdd(instanceId, -1);

                OnLeave(other);
            }
        }

        // Event handler methods to be overridden in derived classes
        protected virtual void OnMatchStart() { }
        protected virtual void OnAutoEnd() { }
        protected virtual void OnEndgameStart() { }
        protected virtual void OnMatchOver() { }
        protected virtual void OnEnter(Collider other) { }
        protected virtual void OnLeave(Collider other) { }
    }
}