using Quille;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Quille.Thought))]
public class ThoughtProperyDrawer : PropertyDrawer
{
    // VARIABLES
    private Thought targetThought;

    private Editor cachedEditor;
    private ThoughtEditor cachedThoughtEditor;

    // Misc
    private GUIStyle customGUIStyle;


    // METHODS
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        targetThought = (Thought)property.objectReferenceValue;

        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PropertyField(position, property, label, true);

        if (property.objectReferenceValue != null)
        {
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
        }
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            if (!cachedEditor)
            {
                Editor.CreateCachedEditor(property.objectReferenceValue, typeof(ThoughtEditor), ref cachedEditor);
                cachedThoughtEditor = cachedEditor as ThoughtEditor;
                cachedThoughtEditor.drawScriptProperty = false;
            }

            EditorGUI.BeginChangeCheck();
            if (cachedEditor)
            {
                cachedEditor.OnInspectorGUI();
            }
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }
}
