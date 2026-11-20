using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Quille.ThoughtData)), CanEditMultipleObjects]
public class ThoughtDataEditor : ItemDataEditor
{
    // VARIABLES
    private SerializedProperty itemColour;
    private SerializedProperty validityChecks;
    private SerializedProperty isTemporary;
    private SerializedProperty defaultLifespan;
    private SerializedProperty lifespanModifiers;



    // METHODS
    private void OnEnable()
    {
        base.OnEnable();

        itemColour = serializedObject.FindProperty("itemColour");
        validityChecks = serializedObject.FindProperty("validityChecks");
        isTemporary = serializedObject.FindProperty("isTemporary");
        defaultLifespan = serializedObject.FindProperty("defaultLifespan");
        lifespanModifiers = serializedObject.FindProperty("lifespanModifiers");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Inherited properties.
        DrawItemDataProperties(true);
        EditorGUILayout.PropertyField(itemColour, new GUIContent("Colour"));

        GUILayout.Space(halfLine);

        EditorGUILayout.PropertyField(validityChecks);
        if (validityChecks.arraySize == 0)
        {
            EditorGUILayout.HelpBox("This thought will always be viable.", MessageType.Info);
            GUILayout.Space(halfLine);
        }

        EditorGUILayout.PropertyField(isTemporary);
        using (new EditorGUI.DisabledScope(!isTemporary.boolValue))
        {
            EditorGUILayout.PropertyField(defaultLifespan);
            EditorGUILayout.PropertyField(lifespanModifiers);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
