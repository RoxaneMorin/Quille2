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
    private SerializedProperty defaultDecayRate;
    private SerializedProperty decayRateModifiers;



    // METHODS
    private void OnEnable()
    {
        base.OnEnable();

        itemColour = serializedObject.FindProperty("itemColour");
        validityChecks = serializedObject.FindProperty("validityChecks");
        isTemporary = serializedObject.FindProperty("isTemporary");
        defaultLifespan = serializedObject.FindProperty("defaultLifespan");
        lifespanModifiers = serializedObject.FindProperty("lifespanModifiers");
        defaultDecayRate = serializedObject.FindProperty("defaultDecayRate");
        decayRateModifiers = serializedObject.FindProperty("decayRateModifiers");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Inherited properties.
        DrawItemDataProperties(true);

        EditorGUILayout.PropertyField(itemColour, new GUIContent("Colour"));

        GUILayout.Space(halfLine);

        EditorGUILayout.PropertyField(validityChecks);

        EditorGUILayout.PropertyField(isTemporary);
        EditorGUILayout.PropertyField(defaultLifespan);
        EditorGUILayout.PropertyField(lifespanModifiers);
        EditorGUILayout.PropertyField(defaultDecayRate);
        EditorGUILayout.PropertyField(decayRateModifiers);

        serializedObject.ApplyModifiedProperties();
    }
}
