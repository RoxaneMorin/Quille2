using QuilleUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(QuilleUI.UI_ReflowableTextbox))]
public class UI_ReflowableTextboxEditor : Editor
{
    // VARIABLES
    // Object References
    private UI_ReflowableTextbox targetReflowableTextbox;
    private TMP_Text targetTextMain;
    private TMP_Text targetTextOverflow;
    private Image targetIcon;

    private RectTransform targetReflowableTextboxRect;
    private RectTransform targetTextMainRect;
    private RectTransform targetTextOverflowRect;
    private RectTransform targetIconRect;

    // Serialized Properties
    private SerializedProperty scriptProperty;

    private SerializedProperty myTextMain;
    private SerializedProperty myTextOverflow;
    private SerializedProperty myIcon;

    private SerializedProperty myRect;
    private SerializedProperty myTextMainsRect;
    private SerializedProperty myTextOverflowsRect;
    private SerializedProperty myIconsRect;

    private SerializedProperty hasIcon;
    private SerializedProperty interiorPadding;
    private SerializedProperty dimensionRatio;

    private SerializedProperty basePreferredDims;
    private SerializedProperty handCalculatedDims;

    private SerializedProperty lineHeight;
    private SerializedProperty lineCount;
    private SerializedProperty summedTextLenght;




    // Local and cached variables 
    private Sprite cachedIconSprite;
    private string cachedTextContent;

    private bool cachedHasIcon;
    private Vector2 cachedDimensionRatio;
    private float cachedInteriorPadding;
    private Vector2 cachedIconSize;
    private float cachedFontSize;


    Vector3[] rectPoints;
    private int[] rectSegmentIndices;
    GUIStyle labelStyle;


    // METHODS
    private void OnEnable()
    {
        // Fetch the target ReflowableTextbox proper.
        targetReflowableTextbox = serializedObject.targetObject as UI_ReflowableTextbox;

        // Fetch serialized properties
        scriptProperty = serializedObject.FindProperty("m_Script");

        myTextMain = serializedObject.FindProperty("myTextMain");
        myTextOverflow = serializedObject.FindProperty("myTextOverflow");
        myIcon = serializedObject.FindProperty("myIcon");

        myRect = serializedObject.FindProperty("myRect");
        myTextMainsRect = serializedObject.FindProperty("myTextMainsRect");
        myTextOverflowsRect = serializedObject.FindProperty("myTextOverflowsRect");
        myIconsRect = serializedObject.FindProperty("myIconsRect");

        hasIcon = serializedObject.FindProperty("hasIcon");
        interiorPadding = serializedObject.FindProperty("interiorPadding");
        dimensionRatio = serializedObject.FindProperty("dimensionRatio");

        basePreferredDims = serializedObject.FindProperty("basePreferredDims");
        handCalculatedDims = serializedObject.FindProperty("handCalculatedDims");

        lineHeight = serializedObject.FindProperty("lineHeight");
        lineCount = serializedObject.FindProperty("lineCount");
        summedTextLenght = serializedObject.FindProperty("summedTextLenght");


        // Fetch rects from serialized properties
        targetTextMain = (TMP_Text)myTextMain.objectReferenceValue;
        targetTextOverflow = (TMP_Text)myTextOverflow.objectReferenceValue;
        targetIcon = (Image)myIcon.objectReferenceValue;

        targetReflowableTextboxRect = (RectTransform)myRect.objectReferenceValue;
        targetTextMainRect = (RectTransform)myTextMainsRect.objectReferenceValue;
        targetTextOverflowRect = (RectTransform)myTextOverflowsRect.objectReferenceValue;
        targetIconRect = (RectTransform)myIconsRect.objectReferenceValue;


        // Generate misc values.
        rectPoints = new Vector3[4];
        rectSegmentIndices = new int[] { 0, 1, 2, 3, 0, 2, 1, 3 };
        labelStyle = new GUIStyle() { alignment = TextAnchor.LowerLeft, fontSize = 10 };
    }





    public override void OnInspectorGUI()
    {
        if (targetReflowableTextbox != null)
        {
            serializedObject.Update();

            // Draw script field.
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(scriptProperty);
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            // Draw properties
            GUILayout.Label("References", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(myRect);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(myIcon.displayName, GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(myIcon, GUIContent.none);
            EditorGUILayout.PropertyField(myIconsRect, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(myTextMain.displayName, GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(myTextMain, GUIContent.none);
            EditorGUILayout.PropertyField(myTextMainsRect, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(myTextOverflow.displayName, GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(myTextOverflow, GUIContent.none);
            EditorGUILayout.PropertyField(myTextOverflowsRect, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);


            GUILayout.Label("Contents", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Icon"), GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUI.BeginChangeCheck();
            cachedIconSprite = (Sprite)EditorGUILayout.ObjectField(targetIcon.sprite, typeof(Sprite), false);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetReflowableTextbox, "Change ReflowableTextbox Icon Sprite");
                targetReflowableTextbox.IconSprite = cachedIconSprite;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Text"), GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUI.BeginChangeCheck();
            cachedTextContent = GUILayout.TextArea(targetTextMain.text);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetReflowableTextbox, "Change ReflowableTextbox Text Content");
                targetReflowableTextbox.TextContent = cachedTextContent;

                targetReflowableTextbox.UpdateTextboxDimensions(false);
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            GUILayout.Label("Layout", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            cachedDimensionRatio = EditorGUILayout.Vector2Field(dimensionRatio.displayName, targetReflowableTextbox.DimensionRatio);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetReflowableTextbox, "Change ReflowableTextbox Dimension Ratio");
                targetReflowableTextbox.DimensionRatio = cachedDimensionRatio;

                targetReflowableTextbox.UpdateTextboxDimensions();
            }

            EditorGUI.BeginChangeCheck();
            cachedInteriorPadding = EditorGUILayout.Slider(interiorPadding.displayName, targetReflowableTextbox.InteriorPadding, 0.0f, 30.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetReflowableTextbox, "Change ReflowableTextbox Interior Padding");
                targetReflowableTextbox.InteriorPadding = cachedInteriorPadding;

                targetReflowableTextbox.UpdateTextboxDimensions();
            }

            EditorGUI.BeginChangeCheck();
            cachedHasIcon = EditorGUILayout.Toggle(hasIcon.displayName, targetReflowableTextbox.HasIcon);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetReflowableTextbox, "Toggle ReflowableTextbox Icon");
                targetReflowableTextbox.HasIcon = cachedHasIcon;

                targetReflowableTextbox.UpdateTextboxDimensions();
            }
            EditorGUI.BeginChangeCheck();
            cachedIconSize = EditorGUILayout.Vector2Field(new GUIContent("Icon Size"), targetIconRect.sizeDelta);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetReflowableTextbox, "Change ReflowableTextbox Icon Size");
                targetIconRect.sizeDelta = cachedIconSize;

                targetReflowableTextbox.UpdateTextboxDimensions();
            }

            EditorGUI.BeginChangeCheck();
            cachedFontSize = EditorGUILayout.Slider(new GUIContent("Font Size"), targetTextMain.fontSize, 5.0f, 50.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetReflowableTextbox, "Change ReflowableTextbox Font Size");
                targetReflowableTextbox.FontSize = cachedFontSize;

                targetReflowableTextbox.UpdateTextboxDimensions();
            }


            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            GUILayout.Label("Runtime Data", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(basePreferredDims);
                EditorGUILayout.PropertyField(handCalculatedDims);

                EditorGUILayout.PropertyField(lineHeight);
                EditorGUILayout.PropertyField(lineCount);
                EditorGUILayout.PropertyField(summedTextLenght);
            }


            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            if (GUILayout.Button(new GUIContent("Recalculate Layout")))
            {
                Undo.RecordObject(targetReflowableTextbox, "Recalculate ReflowableTextbox Dimensions");
                targetReflowableTextbox.UpdateTextboxDimensions();
            }

            serializedObject.ApplyModifiedProperties();
        }
        else
        {
            // Fallback in case of wonkiness.
            DrawDefaultInspector();
        }
    }




    private void OnSceneGUI()
    {
        if (targetReflowableTextbox != null)
        {
            // Base rect bounds.
            Handles.color = new Color(1f, 1f, 1f, 1f);
            Handles.DrawWireCube(targetReflowableTextboxRect.TransformPoint(targetReflowableTextboxRect.rect.center), targetReflowableTextboxRect.sizeDelta);


            // Preferred bounds for line height.
            Handles.color = labelStyle.normal.textColor = new Color(1f, 1f, 0f, 0.75f);
            labelStyle.alignment = TextAnchor.LowerLeft;

            Handles.Label(targetTextMainRect.position, "text bounds without added linebreaks", labelStyle);
            DrawDottedRect(targetTextMainRect.position, basePreferredDims.vector2Value, 4.0f);

            // Raw hand-calculated bounds.
            Vector3 worldTopLeftPos = targetReflowableTextboxRect.TransformPoint(new Vector2(targetReflowableTextboxRect.rect.xMin, targetReflowableTextboxRect.rect.yMax));
            Handles.color = labelStyle.normal.textColor = new Color(1f, 0.65f, 0.25f, 0.75f);

            Handles.Label(worldTopLeftPos, "text bounds calculated from ratio", labelStyle);
            DrawDottedRect(worldTopLeftPos, handCalculatedDims.vector2Value, 4.0f);


            // Actual bounds.

            // Icon.
            if (hasIcon.boolValue && targetIcon.isActiveAndEnabled)
            {
                targetIconRect.GetWorldCorners(rectPoints);
                Handles.color = Color.white;
                Handles.DrawSolidRectangleWithOutline(rectPoints, new Color32(0, 250, 0, 20), new Color32(0, 250, 0, 150));
            }

            // Overflow text.
            if (targetTextOverflow.isActiveAndEnabled)
            {
                Handles.color = labelStyle.normal.textColor = new Color32(200, 150, 255, 150);

                Handles.Label(targetTextOverflowRect.position, "overflow text bounds", labelStyle);
                DrawDottedRect(targetTextOverflowRect.position, targetTextOverflowRect.sizeDelta, 4.0f);
                DrawTranslucentRect(targetTextOverflowRect.position, targetTextOverflow.GetRenderedValues());
            }

            // Main text.
            Handles.color = labelStyle.normal.textColor = new Color32(255, 150, 200, 150);
            labelStyle.alignment = TextAnchor.LowerRight;

            Handles.Label(targetTextMainRect.position + new Vector3(targetTextMainRect.sizeDelta.x, 0.0f, 0.0f), "main text bounds", labelStyle);
            DrawDottedRect(targetTextMainRect.position, targetTextMainRect.sizeDelta, 4.0f);
            DrawTranslucentRect(targetTextMainRect.position, targetTextMain.GetRenderedValues());


            // TODO: ability to resize/scale for current ratio?
        }
    }



    // UTILITY
    private void DrawDottedRect(Vector3 topLeftPoint, Vector2 dimensions, float screenSpaceDottingSize)
    {
        rectPoints[0] = topLeftPoint;
        rectPoints[1] = topLeftPoint + new Vector3(dimensions.x, 0.0f, 0.0f);
        rectPoints[2] = topLeftPoint + new Vector3(0.0f, -dimensions.y, 0.0f);
        rectPoints[3] = topLeftPoint + new Vector3(dimensions.x, -dimensions.y, 0.0f);

        Handles.DrawDottedLines(rectPoints, rectSegmentIndices, screenSpaceDottingSize);
    }

    private void DrawTranslucentRect(Vector3 topLeftPoint, Vector2 dimensions)
    {
        rectPoints[0] = topLeftPoint + new Vector3(0.0f, -dimensions.y, 0.0f);
        rectPoints[1] = topLeftPoint;
        rectPoints[2] = topLeftPoint + new Vector3(dimensions.x, 0.0f, 0.0f);
        rectPoints[3] = topLeftPoint + new Vector3(dimensions.x, -dimensions.y, 0.0f);

        Handles.DrawSolidRectangleWithOutline(rectPoints, new Color32(255, 255, 255, 20), new Color32(255, 255, 255, 75));
    }

}
