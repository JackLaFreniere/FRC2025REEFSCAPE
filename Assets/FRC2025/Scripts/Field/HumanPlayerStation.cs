#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;

namespace FRC2025
{
    [RequireComponent(typeof(BoxCollider))]
    [ExecuteAlways]
    public abstract class HumanPlayerStation : MonoBehaviour
    {
        [Header("Scoring Element Information")]
        [SerializeField] protected GameObject _scoringElementPrefab;

        [Header("Box Collider Information")]
        [SerializeField] protected Vector3 BoxColliderCenter = Vector3.zero;
        [SerializeField] protected Vector3 BoxColliderSize = Vector3.one;

        [Header("Scoring Element Transform Information")]
        [SerializeField] protected Vector3 ScoringElementPosition = Vector3.zero;
        [SerializeField] protected Vector3 ScoringElementRotation = Vector3.zero;

        protected BoxCollider _boxCollider;

#if UNITY_EDITOR
        private GameObject _previewInstance;
        private bool _previewDirty;
#endif

        public abstract void DropScoringElement();
        protected Vector3 GetScoringElementPosition() => transform.TransformPoint(ScoringElementPosition);
        protected Quaternion GetScoringElementRotation() => transform.rotation * Quaternion.Euler(ScoringElementRotation);

        //Only run the following code in the editor so you can see live previews of the scoring element and box collider
#if UNITY_EDITOR
        /// <summary>
        /// Ensures that no stray preview instances exist when the application is not playing.
        /// </summary>
        /// <remarks>This method is intended to be used in the Unity Editor only. It checks if the application is
        /// in play mode, and if not, it destroys any existing preview instances to maintain a clean editor
        /// state.</remarks>
        private void Start()
        {
            if (!Application.isPlaying)
                DestroyPreviewInstance(); // ensure no stray previews at start
        }

        /// <summary>
        /// Called when the object becomes disabled or inactive.
        /// </summary>
        /// <remarks>This method ensures that any preview instance created by the object is properly destroyed to
        /// release resources and prevent memory leaks.</remarks>
        private void OnDisable()
        {
            DestroyPreviewInstance();
        }

        /// <summary>
        /// Resets the state of the object by reinitializing its internal components.
        /// </summary>
        /// <remarks>This method ensures that the object's internal state is restored to its default
        /// configuration. It is typically used to reapply settings or refresh the object after changes.</remarks>
        private void Reset()
        {
            CacheCollider();
            FixEditor();
        }

        /// <summary>
        /// Called by the Unity Editor to validate and update the state of the object when changes are made in the
        /// Inspector.
        /// </summary>
        /// <remarks>This method ensures that the object's collider and related settings are updated to reflect
        /// any modifications made in the Inspector. It is primarily used to maintain consistency and correctness of the
        /// object's state during design-time.</remarks>
        private void OnValidate()
        {
            // Collider settings
            CacheCollider();
            UpdateColliderSettings();

            MarkPreviewDirty(); // Preview scoring element
            FixEditor(); // Editor state
        }

        /// <summary>
        /// Marks the preview as dirty, scheduling an update to refresh the preview.
        /// </summary>
        /// <remarks>This method ensures that the preview is updated only once, even if called multiple times, by
        /// setting an internal flag. The update is scheduled to occur on the next editor update cycle.</remarks>
        private void MarkPreviewDirty()
        {
            if (!_previewDirty)
            {
                /// Delays the method call to <see cref="SafeUpdatePreview"/> to the next editor update cycle
                _previewDirty = true;
                EditorApplication.delayCall += SafeUpdatePreview;
            }
        }

        /// <summary>
        /// Safely updates the preview instance if the object and its associated game object are still valid.
        /// </summary>
        /// <remarks>This method ensures that the preview update logic is only executed if the object has not been
        /// destroyed and its associated game object is still valid. It resets the internal preview state before performing
        /// the update.</remarks>
        private void SafeUpdatePreview()
        {
            _previewDirty = false;
            // Checks if object or GameObject has been destroyed before updating preview
            if (this == null || !gameObject) return;
            UpdatePreviewInstance();
        }

        /// <summary>
        /// Updates the preview instance of the scoring element in the editor.
        /// </summary>
        /// <remarks>This method destroys the existing preview instance, if any, and creates a new one based on
        /// the specified scoring element prefab. The preview instance is only created when a prefab is assigned and the
        /// application is not in play mode. The new instance is positioned and rotated according to the specified scoring
        /// element position and rotation, and is marked as not savable to ensure it does not persist in the
        /// scene.</remarks>
        private void UpdatePreviewInstance()
        {
            DestroyPreviewInstance();

            if (_scoringElementPrefab != null && !Application.isPlaying)
            {
                _previewInstance = Instantiate(_scoringElementPrefab, transform);
                _previewInstance.name = _scoringElementPrefab.name + " (Preview)";
                _previewInstance.hideFlags = HideFlags.DontSave;
                _previewInstance.transform.SetLocalPositionAndRotation(
                    ScoringElementPosition,
                    Quaternion.Euler(ScoringElementRotation)
                );
            }
        }

        /// <summary>
        /// Destroys the current preview instance if it exists.
        /// </summary>
        /// <remarks>This method releases the resources associated with the preview instance by immediately
        /// destroying it and setting the reference to <see langword="null" />. Ensure that this method is called only when
        /// the preview instance is no longer needed.</remarks>
        private void DestroyPreviewInstance()
        {
            if (_previewInstance != null)
            {
                // Use DestroyImmediate to ensure immediate cleanup in the editor
                DestroyImmediate(_previewInstance);
                _previewInstance = null;
            }
        }

        /// <summary>
        /// Adjusts the editor state for the current object and its associated BoxCollider component.
        /// </summary>
        /// <remarks>This method ensures that the inspector for the current object is expanded, while collapsing
        /// the inspector for the associated BoxCollider component. It also marks the BoxCollider component as dirty and
        /// moves it down in the component hierarchy.</remarks>
        private void FixEditor()
        {
            // Makes sure this script is always expanded and the BoxCollider is collapsed in the inspector
            InternalEditorUtility.SetIsInspectorExpanded(this, true);
            InternalEditorUtility.SetIsInspectorExpanded(_boxCollider, false);

            // Makes sure that the BoxCollider is marked as dirty and moved down in the component list
            EditorUtility.SetDirty(_boxCollider);
            ComponentUtility.MoveComponentDown(_boxCollider);
        }

        /// <summary>
        /// Ensures that the <see cref="BoxCollider"/> component is cached for future use.
        /// </summary>
        /// <remarks>If the <see cref="BoxCollider"/> component is not already cached, this method retrieves it 
        /// from the current GameObject and stores it for subsequent operations. This avoids repeated calls to <see
        /// cref="GetComponent{T}"/> for the same component.</remarks>
        private void CacheCollider()
        {
            if (_boxCollider == null)
                _boxCollider = GetComponent<BoxCollider>();
        }

        /// <summary>
        /// Updates the settings of the associated box collider.
        /// </summary>
        /// <remarks>This method ensures that the box collider is configured as a trigger and updates its center
        /// and size based on the current values of <see cref="BoxColliderCenter"/> and <see
        /// cref="BoxColliderSize"/>.</remarks>
        private void UpdateColliderSettings()
        {
            if (_boxCollider == null) return;

            _boxCollider.isTrigger = true;
            _boxCollider.center = BoxColliderCenter;
            _boxCollider.size = BoxColliderSize;
        }
#endif
    }
}