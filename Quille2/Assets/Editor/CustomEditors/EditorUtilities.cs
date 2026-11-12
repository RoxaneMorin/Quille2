using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class EditorUtilities
{
    // VARIABLES
    public static readonly int[] rectSegmentIndices = { 0, 1, 1, 3, 3, 2, 2, 0 };
    public static Vector3[] rectPoints = new Vector3[4];



    // METHODS
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

    public static void DrawBorderedRect(Rect rect, int borderThickness, Color borderColor, Color backgroundColor)
    {
        EditorGUI.DrawRect(rect, borderColor);

        Rect innerRect = new Rect(rect.x + borderThickness, rect.y + borderThickness, rect.width - (borderThickness * 2), rect.height - (borderThickness * 2));
        EditorGUI.DrawRect(innerRect, backgroundColor);
    }


    // OnSceneGUI
    public static void DrawSquare(Vector3 bottomLeftPoint, Vector2 dimensions)
    {
        rectPoints[0] = bottomLeftPoint;
        rectPoints[1] = bottomLeftPoint + new Vector3(dimensions.x, 0.0f, 0.0f);
        rectPoints[2] = bottomLeftPoint + new Vector3(0.0f, dimensions.y, 0.0f);
        rectPoints[3] = bottomLeftPoint + new Vector3(dimensions.x, dimensions.y, 0.0f);

        Handles.DrawLines(rectPoints, rectSegmentIndices);
    }

    public static void DrawDottedRect(Vector3 bottomLeftPoint, Vector2 dimensions, float screenSpaceDottingSize)
    {
        rectPoints[0] = bottomLeftPoint;
        rectPoints[1] = bottomLeftPoint + new Vector3(dimensions.x, 0.0f, 0.0f);
        rectPoints[2] = bottomLeftPoint + new Vector3(0.0f, dimensions.y, 0.0f);
        rectPoints[3] = bottomLeftPoint + new Vector3(dimensions.x, dimensions.y, 0.0f);

        Handles.DrawDottedLines(rectPoints, rectSegmentIndices, screenSpaceDottingSize);
    }
    public static void DrawTranslucentRect(Vector3 bottomLeftPoint, Vector2 dimensions)
    {
        rectPoints[0] = bottomLeftPoint + new Vector3(0.0f, dimensions.y, 0.0f);
        rectPoints[1] = bottomLeftPoint;
        rectPoints[2] = bottomLeftPoint + new Vector3(dimensions.x, 0.0f, 0.0f);
        rectPoints[3] = bottomLeftPoint + new Vector3(dimensions.x, dimensions.y, 0.0f);

        Handles.DrawSolidRectangleWithOutline(rectPoints, new Color32(255, 255, 255, 20), new Color32(255, 255, 255, 75));
    }






    public static Type[] FindValidSubtypes(Type baseType, bool includeSelfIfValid = true)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
                                       .SelectMany(assembly =>
                                       {
                                           try
                                           { return assembly.GetTypes(); }
                                           catch (ReflectionTypeLoadException e)
                                           {
                                               return e.Types.Where(type => type != null);
                                           }
                                       }).Where(type => baseType.IsAssignableFrom(type) && (includeSelfIfValid || type != baseType) && !type.IsAbstract && !type.ContainsGenericParameters).ToArray();
    }

    public static string[] GetTypeNames(IEnumerable<Type> types)
    {
        return types.Select(type => type.Name).ToArray();
    }
}
