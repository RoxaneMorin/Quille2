using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(QuilleUI.ButtonExtended), true)]
[CanEditMultipleObjects]
public class ButtonExtendedEditor : UnityEditor.UI.ButtonEditor
{
    SerializedProperty m_OnRightClickProperty;
    SerializedProperty m_OnMiddleClickProperty;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_OnRightClickProperty = serializedObject.FindProperty("m_OnRightClick");
        m_OnMiddleClickProperty = serializedObject.FindProperty("m_OnMiddleClick");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        serializedObject.Update();
        EditorGUILayout.PropertyField(m_OnRightClickProperty);
        EditorGUILayout.PropertyField(m_OnMiddleClickProperty);
        serializedObject.ApplyModifiedProperties();
    }
}
