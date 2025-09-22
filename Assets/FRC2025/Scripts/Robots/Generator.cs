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
        /// Ensures that the specified directory exists by validating or creating it.
        /// </summary>
        /// <remarks>If the specified directory does not exist, this method attempts to find a child
        /// object with the specified name under the given parent. If no such child is found, a new <see
        /// cref="GameObject"/> is created with the specified name and parented to the provided parent or the current
        /// object's transform. The new directory's position and rotation are reset to local defaults.</remarks>
        /// <param name="directory">A reference to the <see cref="GameObject"/> representing the directory. If the directory is <see
        /// langword="null"/>, it will be initialized to an existing child object with the specified name, or a new
        /// <see cref="GameObject"/> will be created if no such child exists.</param>
        /// <param name="name">The name of the directory to validate or create. This is used to locate an existing child object or to name
        /// the newly created <see cref="GameObject"/>.</param>
        /// <param name="parent">An optional parent <see cref="GameObject"/> under which the directory will be searched for or created. If
        /// <see langword="null"/>, the current object's transform is used as the parent.</param>
        protected void ValidateDirectory(ref GameObject directory, string name, GameObject parent = null)
        {
            if (directory != null) return;

            Transform target = parent == null ? transform.Find(name): parent.transform.Find(name);
            if (target!= null)
            {
                directory = target.gameObject;
                return;
            }

            directory = new(name);
            directory.transform.SetParent(parent == null ? transform : parent.transform);
            directory.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
#endif
    }
}