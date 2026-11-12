using Quille;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(Quille.Thought))]
public class ThoughtProperyDrawer : PropertyDrawer
{
    // VARIABLES
    private Thought targetThought;
    private ThoughtEditor cachedEditor;


    // METHODS
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUILayout.PropertyField(property);

        targetThought = (Thought)property.objectReferenceValue;

        cachedEditor = (ThoughtEditor)Editor.CreateEditor(targetThought);
        cachedEditor.drawScriptProperty = false;
        cachedEditor.OnInspectorGUI();
    }
}
