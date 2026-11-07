using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorUtilities
{
    public static void DrawAmbidextrousProperty(SerializedProperty left, SerializedProperty right, float gapSpace = 8, bool drawLabels = true)
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

    public static void DrawShortLabelAndProperty(SerializedProperty property)
    {
        GUILayout.Label(property.displayName);
        EditorGUILayout.PropertyField(property, GUIContent.none);
    }


    // OnSceneGUI
    public static void DrawDottedRect(Vector3 topLeftPoint, Vector2 dimensions, float screenSpaceDottingSize, Vector3[] rectPoints, int[] rectSegmentIndices)
    {
        rectPoints[0] = topLeftPoint;
        rectPoints[1] = topLeftPoint + new Vector3(dimensions.x, 0.0f, 0.0f);
        rectPoints[2] = topLeftPoint + new Vector3(0.0f, -dimensions.y, 0.0f);
        rectPoints[3] = topLeftPoint + new Vector3(dimensions.x, -dimensions.y, 0.0f);

        Handles.DrawDottedLines(rectPoints, rectSegmentIndices, screenSpaceDottingSize);
    }
    public static void DrawTranslucentRect(Vector3 topLeftPoint, Vector2 dimensions, Vector3[] rectPoints)
    {
        rectPoints[0] = topLeftPoint + new Vector3(0.0f, -dimensions.y, 0.0f);
        rectPoints[1] = topLeftPoint;
        rectPoints[2] = topLeftPoint + new Vector3(dimensions.x, 0.0f, 0.0f);
        rectPoints[3] = topLeftPoint + new Vector3(dimensions.x, -dimensions.y, 0.0f);

        Handles.DrawSolidRectangleWithOutline(rectPoints, new Color32(255, 255, 255, 20), new Color32(255, 255, 255, 75));
    }
}
