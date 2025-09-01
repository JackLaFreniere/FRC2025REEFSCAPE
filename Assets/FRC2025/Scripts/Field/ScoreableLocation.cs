using UnityEngine;

namespace FRC2025
{
    public abstract class ScoreableLocation : MonoBehaviour
    {
        [Header("Location Settings")]
        [SerializeField] protected AllianceColor _allianceColor;
        [SerializeField] protected GameObject _scoringElement;

        protected virtual bool IsValidScoringObject(Collider other) =>
            other.CompareTag(_scoringElement.tag) && other.GetComponent<ScoringElement>().InRobot;

        protected abstract void OnScored(Collider other);

#if UNITY_EDITOR

        private void Reset() => EditorSetup();
        private void OnValidate() => EditorSetup();

        private void EditorSetup()
        {
            CacheCollider();
            UpdateColliderSettings();
            FixEditor();
        }

        // Override if the derived class has a collider
        protected virtual void CacheCollider() { }
        protected virtual void UpdateColliderSettings() { }
        protected virtual void FixEditor() { }
        
        protected void HandleEditorComponent(Component component, bool expand = true)
        {
            if (component == null) return;
            UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(component, expand);
            UnityEditor.EditorUtility.SetDirty(component);
        }

        protected T CacheCollider<T>(ref T colliderField) where T : Collider
        {
            if (colliderField == null)
                colliderField = GetComponent<T>();
            return colliderField;
        }

#endif

    }
}