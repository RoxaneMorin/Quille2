using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorUtilities
{
    public static void drawAmbidextrousProperty(SerializedProperty left, SerializedProperty right, float gapSpace = 8, bool drawLabels = true)
    {
        // Draw labels
        if (drawLabels)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(left.displayName);
            GUILayout.Space(gapSpace);
            GUILayout.Label(right.displayName);
            EditorGUILayout.EndHorizontal();
        }

        // Draw properties.
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(left, GUIContent.none);
        GUILayout.Space(gapSpace);
        EditorGUILayout.PropertyField(right, GUIContent.none);
        EditorGUILayout.EndHorizontal();

        //GUILayout.Space(EditorGUIUtility.singleLineHeight/5);
    }

    public static void drawLabelAndProperty(SerializedProperty property)
    {
        GUILayout.Label(property.displayName);
        EditorGUILayout.PropertyField(property, GUIContent.none);
    }
}
