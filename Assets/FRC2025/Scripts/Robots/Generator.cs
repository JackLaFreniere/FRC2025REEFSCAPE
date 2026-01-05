using UnityEngine;

namespace FRC2025
{
    public class Generator<S> : MonoBehaviour where S : Component
    {
        [Header("Parent GameObject Settings")]
        [SerializeField] protected GameObject _parentObject;

        protected string _name;
        protected string _layerName = "Robot";
        protected bool _isInitialized = false;

        private Vector3 _localPosition;
        private Quaternion _localRotation;
        private bool _hasComputedLocalTransform = false;

        /// <summary>
        /// Initializes the component and updates its local transformation state.
        /// </summary>
        /// <remarks>This method should be called to ensure the component is properly initialized before
        /// use. If the component has already been removed, the method returns without performing any further
        /// actions.</remarks>
        protected void Start()
        {
            if (AttemptRemoveSelf(this)) return;

            ComputeLocalTransform();
        }

        /// <summary>
        /// Updates the object's transform and layer assignment based on its parent and configured layer name.
        /// </summary>
        /// <remarks>This method synchronizes the object's transform with its parent, if one exists, and
        /// sets the object's layer recursively according to the specified layer name. Override this method in a derived
        /// class to customize update behavior.</remarks>
        protected virtual void Update()
        {
            if (_parentObject != null)
            {
                UpdateTransformFromParent();
            }

            SetLayerRecursively(this.gameObject, LayerMask.NameToLayer(_layerName));
        }

        /// <summary>
        /// Calculates and updates the local position and rotation of the object relative to its parent or to the world
        /// if no parent exists.
        /// </summary>
        /// <remarks>This method should be called whenever the object's transform or its parent's
        /// transform changes to ensure that the local position and rotation remain accurate. It sets internal state
        /// used by other components that depend on the object's local transform.</remarks>
        private void ComputeLocalTransform()
        {
            if (_parentObject != null)
            {
                _localPosition = _parentObject.transform.InverseTransformPoint(transform.position);
                _localRotation = Quaternion.Inverse(_parentObject.transform.rotation) * transform.rotation;
            }
            else
            {
                _localPosition = transform.localPosition;
                _localRotation = transform.localRotation;
            }

            _hasComputedLocalTransform = true;
        }

        /// <summary>
        /// Updates this object's world position and rotation to match its local transform relative to the parent
        /// object.
        /// </summary>
        /// <remarks>This method recalculates the object's world transform based on its local position and
        /// rotation with respect to its parent. It should be called after changes to the local transform or parent
        /// object to ensure the world transform remains accurate.</remarks>
        private void UpdateTransformFromParent()
        {
            if (!_hasComputedLocalTransform)
            {
                ComputeLocalTransform();
            }

            Vector3 targetWorldPosition = _parentObject.transform.TransformPoint(_localPosition);
            Quaternion targetWorldRotation = _parentObject.transform.rotation * _localRotation;

            transform.SetPositionAndRotation(targetWorldPosition, targetWorldRotation);
        }

        /// <summary>
        /// Attempts to remove the specified component from the GameObject if it is not attached to a parent transform.
        /// </summary>
        /// <remarks>Use this method to conditionally remove a component only when the GameObject is not
        /// part of a parent-child hierarchy. The method performs immediate destruction of the component, which cannot
        /// be undone.</remarks>
        /// <param name="script">The component to remove from the GameObject.</param>
        /// <returns>true if the component was removed; otherwise, false.</returns>
        private bool AttemptRemoveSelf(Component script)
        {
            if (transform.parent == null)
            {
                DestroyImmediate(script);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Resets the state of the component by reinitializing its child objects and ensuring required components are
        /// present.
        /// </summary>
        /// <remarks>This method removes all existing child objects from the current transform and adds a
        /// required component of type S if it is not already attached. If the transform has no parent, it creates a new
        /// child GameObject and attaches a component of the current type. This method is intended for internal use and
        /// may modify the GameObject hierarchy and component composition.</remarks>
        private void Reset()
        {
            if (transform.parent == null)
            {
                GameObject child = new(_name);
                child.transform.SetParent(transform, false);
                child.AddComponent(GetType());
                _isInitialized = true;

                return;
            }

            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            if (gameObject.GetComponent<S>() == null)
            {
                gameObject.AddComponent<S>();
            }
        }

        /// <summary>
        /// Ensures that the specified directory GameObject exists as a child of the given parent, creating it if
        /// necessary.
        /// </summary>
        /// <remarks>If the directory GameObject does not exist, this method searches for a child with the
        /// specified name under the given parent. If not found, it creates a new GameObject with the specified name and
        /// assigns it as a child of the parent. The reference parameter is updated to point to the resulting
        /// directory.</remarks>
        /// <param name="directory">A reference to the GameObject representing the directory. If null, this parameter will be assigned to the
        /// found or newly created directory.</param>
        /// <param name="name">The name of the directory GameObject to validate or create.</param>
        /// <param name="parent">The parent GameObject under which to search for or create the directory. If null, a default parts directory
        /// is used as the parent.</param>
        protected void ValidateDirectory(ref GameObject directory, string name, GameObject parent = null)
        {
            if (directory != null) return;

            // If no parent provided, use parts directory
            if (parent == null)
            {
                parent = ValidatePartsDirectory();
            }

            // Try to find existing directory
            Transform target = parent.transform.Find(name);
            if (target != null)
            {
                directory = target.gameObject;
                return;
            }

            // Create new directory
            directory = new(name);
            directory.transform.SetParent(parent == null ? transform : parent.transform);
            directory.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Ensures that a child GameObject named "[this.name] Parts" exists as a direct child of the current transform
        /// and returns it.
        /// </summary>
        /// <remarks>This method is typically used to organize related parts under a dedicated child
        /// GameObject. The returned GameObject will always be a direct child of the current object's
        /// transform.</remarks>
        /// <returns>The GameObject representing the "[this.name] Parts" child. If it does not exist, a new GameObject is
        /// created, parented to the current transform, and returned.</returns>
        private GameObject ValidatePartsDirectory()
        {
            string partsName = this.name + " Parts";
            Transform parts = transform.Find(partsName);

            if (parts == null)
            {
                GameObject parent = new(this.name + " Parts");
                parent.transform.SetParent(transform);
                parent.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                return parent;
            }
            else
            {
                return parts.gameObject;
            }
        }

        /// <summary>
        /// Sets the layer of the specified GameObject and all its child GameObjects recursively to the given layer.
        /// </summary>
        /// <remarks>This method updates the layer property for the entire hierarchy rooted at the
        /// specified GameObject. If the provided GameObject is null, the method performs no action.</remarks>
        /// <param name="obj">The root GameObject whose layer and the layers of all its descendants will be set. Cannot be null.</param>
        /// <param name="newLayer">The layer to assign to the GameObject and all its children.</param>
        protected void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (obj == null) return;

            obj.layer = newLayer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }
}