using UnityEngine;

namespace FRC2025
{
    public class Generator<S> : MonoBehaviour where S : Component
    {
        [Header("Parent GameObject Settings")]
        [SerializeField] private GameObject _parentObject;

        protected string _name;
        protected string _layerName = "Robot";
        protected bool _isInitialized = false;

        private Vector3 _localPosition;
        private Quaternion _localRotation;
        private bool _hasComputedLocalTransform = false;

        protected void Start()
        {
            if (AttemptRemoveSelf(this)) return;

            ComputeLocalTransform();
        }

        protected virtual void Update()
        {
            if (_parentObject != null)
            {
                UpdateTransformFromParent();
            }

            SetLayerRecursively(this.gameObject, LayerMask.NameToLayer(_layerName));
        }

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

        private bool AttemptRemoveSelf(Component script)
        {
            if (transform.parent == null)
            {
                DestroyImmediate(script);
                return true;
            }

            return false;
        }

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