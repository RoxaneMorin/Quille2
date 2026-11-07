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

    private SerializedProperty paddedIconWidth;
    private SerializedProperty paddedIconHeight;

    private SerializedProperty lineHeight;
    private SerializedProperty lineCount;

    private SerializedProperty summedTextLenght;
    private SerializedProperty iconAsAdditionalTextLenght;
    
    private SerializedProperty shortestTextDimensions;
    private SerializedProperty calculatedTextDimensions;


    // Local and cached variables 
    private Sprite cachedIconSprite;
    private string cachedTextContent;

    private bool cachedHasIcon;
    private Vector2 cachedDimensionRatio;
    private float cachedInteriorPadding;
    private Vector2 cachedIconSize;
    private float cachedFontSize;

    private Vector3[] rectPoints;
    private int[] rectSegmentIndices;
    private GUIStyle labelStyle;



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

        shortestTextDimensions = serializedObject.FindProperty("shortestTextDimensions");
        calculatedTextDimensions = serializedObject.FindProperty("calculatedTextDimensions");

        paddedIconWidth = serializedObject.FindProperty("paddedIconWidth");
        paddedIconHeight = serializedObject.FindProperty("paddedIconHeight");

        lineHeight = serializedObject.FindProperty("lineHeight");
        lineCount = serializedObject.FindProperty("lineCount");

        summedTextLenght = serializedObject.FindProperty("summedTextLenght");
        iconAsAdditionalTextLenght = serializedObject.FindProperty("iconAsAdditionalTextLenght");

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
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            GUILayout.Label("Layout", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            cachedDimensionRatio = EditorGUILayout.Vector2Field("Dimension Ratio", targetReflowableTextbox.DimensionRatio);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetReflowableTextbox, "Change ReflowableTextbox Dimension Ratio");
                targetReflowableTextbox.DimensionRatio = cachedDimensionRatio;
            }

            EditorGUI.BeginChangeCheck();
            cachedInteriorPadding = EditorGUILayout.Slider("Interior Padding", targetReflowableTextbox.InteriorPadding, 0.0f, 50.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetReflowableTextbox, "Change ReflowableTextbox Interior Padding");
                targetReflowableTextbox.InteriorPadding = cachedInteriorPadding;
            }

            EditorGUI.BeginChangeCheck();
            cachedHasIcon = EditorGUILayout.Toggle("Has Icon", targetReflowableTextbox.HasIcon);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetReflowableTextbox, "Toggle ReflowableTextbox Icon");
                targetReflowableTextbox.HasIcon = cachedHasIcon;
            }
            EditorGUI.BeginChangeCheck();
            cachedIconSize = EditorGUILayout.Vector2Field("Icon Size", targetReflowableTextbox.IconSize);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetReflowableTextbox, "Change ReflowableTextbox Icon Size");
                targetReflowableTextbox.IconSize = cachedIconSize;
            }

            EditorGUI.BeginChangeCheck();
            cachedFontSize = EditorGUILayout.Slider("Font Size", targetReflowableTextbox.FontSize, 5.0f, 50.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetReflowableTextbox, "Change ReflowableTextbox Font Size");
                targetReflowableTextbox.FontSize = cachedFontSize;
            }


            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            GUILayout.Label("Runtime Data", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Padded Icon", GUILayout.Width(EditorGUIUtility.labelWidth));
                GUILayout.Label("Width");
                EditorGUILayout.PropertyField(paddedIconWidth, GUIContent.none);
                GUILayout.Label("Height");
                EditorGUILayout.PropertyField(paddedIconHeight, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Line", GUILayout.Width(EditorGUIUtility.labelWidth));
                GUILayout.Label("Count");
                EditorGUILayout.PropertyField(lineCount, GUIContent.none);
                GUILayout.Label("Height");
                EditorGUILayout.PropertyField(lineHeight, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(summedTextLenght);
                EditorGUILayout.PropertyField(iconAsAdditionalTextLenght);

                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 4f);

                EditorGUILayout.PropertyField(shortestTextDimensions);
                EditorGUILayout.PropertyField(calculatedTextDimensions);
            }


            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            if (GUILayout.Button(new GUIContent("Recalculate Layout")))
            {
                Undo.RecordObject(targetReflowableTextbox, "Recalculate ReflowableTextbox Dimensions");
                targetReflowableTextbox.UpdateLayout(true);
            }
            
            serializedObject.ApplyModifiedProperties();

            if (targetReflowableTextbox.IsDirty)
            {
                targetReflowableTextbox.UpdateLayout();
            }
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
            Handles.color = new Color(0.5f, 0.75f, 1.0f, 1.0f);
            Handles.DrawWireCube(targetReflowableTextboxRect.TransformPoint(targetReflowableTextboxRect.rect.center), targetReflowableTextboxRect.sizeDelta);


            // Preferred bounds for line height.
            Handles.color = labelStyle.normal.textColor = new Color(1f, 1f, 0f, 0.75f);
            labelStyle.alignment = TextAnchor.LowerLeft;

            //Handles.Label(targetTextMainRect.position, "text bounds without added linebreaks", labelStyle);
            //DrawDottedRect(targetTextMainRect.position, shortestTextDimensions.vector2Value, 4.0f);

            // Raw hand-calculated bounds.
            Vector3 worldTopLeftPos = targetReflowableTextboxRect.TransformPoint(new Vector2(targetReflowableTextboxRect.rect.xMin, targetReflowableTextboxRect.rect.yMax));
            Handles.color = labelStyle.normal.textColor = new Color(1f, 0.65f, 0.25f, 0.75f);

            Handles.Label(worldTopLeftPos, "text bounds calculated from ratio", labelStyle);
            EditorUtilities.DrawDottedRect(worldTopLeftPos, calculatedTextDimensions.vector2Value, 4.0f, rectPoints, rectSegmentIndices);


            // Actual bounds.

            // Icon.
            if (targetReflowableTextbox.HasIcon && targetIcon.isActiveAndEnabled)
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
                EditorUtilities.DrawDottedRect(targetTextOverflowRect.position, targetTextOverflowRect.sizeDelta, 4.0f, rectPoints, rectSegmentIndices);
                EditorUtilities.DrawTranslucentRect(targetTextOverflowRect.position, targetTextOverflow.GetRenderedValues(), rectPoints);
            }

            // Main text.
            Handles.color = labelStyle.normal.textColor = new Color32(255, 150, 200, 150);
            labelStyle.alignment = TextAnchor.LowerRight;

            Handles.Label(targetTextMainRect.position + new Vector3(targetTextMainRect.sizeDelta.x, 0.0f, 0.0f), "main text bounds", labelStyle);
            EditorUtilities.DrawDottedRect(targetTextMainRect.position, targetTextMainRect.sizeDelta, 4.0f, rectPoints, rectSegmentIndices);
            EditorUtilities.DrawTranslucentRect(targetTextMainRect.position, targetTextMain.GetRenderedValues(), rectPoints);


            // TODO: ability to resize/scale for current ratio?
        }
    }
}
