using UnityEngine;

namespace FRC2025
{
    public class Generator<S> : MonoBehaviour where S : Component
    {
#if UNITY_EDITOR
        protected string _name;
        protected bool _isInitialized = false;

        /// <summary>
        /// Initiates the process of removing the current instance from its context.
        /// </summary>
        /// <remarks>This method is intended to be called to trigger the removal of the current instance. 
        /// The specific behavior depends on the implementation of the <c>AttemptRemoveSelf</c> method.</remarks>
        protected void Start()
        {
            AttemptRemoveSelf(this);
        }

        /// <summary>
        /// Attempts to remove the specified component if the current object has no parent.
        /// </summary>
        /// <remarks>This method immediately destroys the specified component if the current object's 
        /// <see cref="Transform.parent"/> is null. Use with caution, as <see
        /// cref="UnityEngine.Object.DestroyImmediate"/> can have unintended side effects if called during certain
        /// Unity lifecycle events.</remarks>
        /// <param name="script">The component to be removed. Must not be null.</param>
        private void AttemptRemoveSelf(Component script)
        {
            if (transform.parent == null)
            {
                DestroyImmediate(script);
            }
        }

        /// <summary>
        /// Resets the current object by reinitializing its state and ensuring required components are present.
        /// </summary>
        /// <remarks>If the object has no parent, a new child object is created, initialized, and attached
        /// to it. Otherwise, all child objects are destroyed, and a required component of type <see cref="S"/> is
        /// the corresponding subsystem that is added to the current object if it does not already exist.</remarks>
        private void Reset()
        {
            if (transform.parent == null)
            {
                GameObject child = new(_name);
                child.transform.SetParent(transform, false);

                child.AddComponent(this.GetType());
                _isInitialized = true;

                return;
            }

            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }

            if (gameObject.GetComponent<S>() == null)
            {
                gameObject.AddComponent<S>();
            }
        }

        /// <summary>
        /// Ensures that the specified directory exists as a child of the current transform.
        /// </summary>
        /// <remarks>If the directory already exists, no changes are made. If a new directory is created,
        /// it is added as a child of the current transform, positioned at the origin with no rotation.</remarks>
        /// <param name="directory">A reference to the <see cref="GameObject"/> representing the directory. If the reference is null, the
        /// method will attempt to find a child object with the specified name. If no such child exists, a new <see
        /// cref="GameObject"/> is created and assigned to this reference.</param>
        /// <param name="name">The name of the directory to validate or create. This name is used to search for an existing child object
        /// or to assign to the newly created <see cref="GameObject"/>.</param>
        protected void ValidateDirectory(ref GameObject directory, string name, GameObject parent = null)
        {
            if (directory != null) return;

            if (parent == null ? transform.Find(name) != null : parent.transform.Find(name) != null)
            {
                directory = transform.Find(name).gameObject;
                return;
            }

            directory = new(name);
            directory.transform.SetParent(parent == null ? transform : parent.transform);
            directory.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
#endif
    }
}