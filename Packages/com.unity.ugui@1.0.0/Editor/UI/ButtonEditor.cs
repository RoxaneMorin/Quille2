using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(Button), true)]
    [CanEditMultipleObjects]
    /// <summary>
    ///   Custom Editor for the Button Component.
    ///   Extend this class to write a custom editor for a component derived from Button.
    /// </summary>
    public class ButtonEditor : SelectableEditor
    {
        SerializedProperty m_OnClickProperty;
        SerializedProperty m_OnLeftClickProperty;
        SerializedProperty m_OnRightClickProperty;
        SerializedProperty m_OnMiddleClickProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
            m_OnLeftClickProperty = serializedObject.FindProperty("m_OnLeftClick");
            m_OnRightClickProperty = serializedObject.FindProperty("m_OnRightClick");
            m_OnMiddleClickProperty = serializedObject.FindProperty("m_OnMiddleClick");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_OnClickProperty);
            EditorGUILayout.PropertyField(m_OnLeftClickProperty);
            EditorGUILayout.PropertyField(m_OnRightClickProperty);
            EditorGUILayout.PropertyField(m_OnMiddleClickProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
