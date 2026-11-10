using QuilleUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(QuilleUI.UI_FoldingPanel))]
public class UI_FoldingPanelEditor : Editor
{
    // VARIABLES
    // Object References
    private UI_FoldingPanel targetFoldingPanel;

    private RectTransform targetFoldingPanelsRect;
    private RectTransform targetButtonsRect;
    private RectTransform targetDragGripsRect;
    private RectTransform targetContentsRect;

    // Serialized Properties
    private SerializedProperty scriptProperty;
    private SerializedProperty myDragGrip;
    private SerializedProperty myDragGripImage;
    private SerializedProperty myButton;
    private SerializedProperty myButtonText;
    private SerializedProperty myContent;
    private SerializedProperty myRect;
    private SerializedProperty myDragGripsRect;
    private SerializedProperty myButtonsRect;
    private SerializedProperty myContentsRect;

    private SerializedProperty dragGripSpriteLeft;
    private SerializedProperty dragGripSpriteRight;

    private SerializedProperty closedPanelWidth;
    private SerializedProperty contentWidth;
    private SerializedProperty contentHeight;

    // Locally cached values
    private float cachedMinOpenPanelWidth;
    private float cachedMaxOpenPanelWidth;
    private float cachedOpenPanelWidth;
    private float cachedInteriorPadding;
    private float cachedButtomWidth;
    private float cachedDragGripDistance;
    private bool cachedIsRightSidePanel;


    // METHODS
    private void OnEnable()
    {
        // Fetch the target FoldingPanel proper.
        targetFoldingPanel = serializedObject.targetObject as UI_FoldingPanel;

        // Fetch serialized properties
        scriptProperty = serializedObject.FindProperty("m_Script");

        myDragGrip = serializedObject.FindProperty("myDragGrip");
        myDragGripImage = serializedObject.FindProperty("myDragGripImage");
        myButton = serializedObject.FindProperty("myButton");
        myButtonText = serializedObject.FindProperty("myButtonText");
        myContent = serializedObject.FindProperty("myContent");
        myRect = serializedObject.FindProperty("myRect");
        myDragGripsRect = serializedObject.FindProperty("myDragGripsRect");
        myButtonsRect = serializedObject.FindProperty("myButtonsRect");
        myContentsRect = serializedObject.FindProperty("myContentsRect");

        dragGripSpriteLeft = serializedObject.FindProperty("dragGripSpriteLeft");
        dragGripSpriteRight = serializedObject.FindProperty("dragGripSpriteRight");

        closedPanelWidth = serializedObject.FindProperty("closedPanelWidth");
        contentWidth = serializedObject.FindProperty("contentWidth");
        contentHeight = serializedObject.FindProperty("contentHeight");

        // Fetchrect rects from serialized properties
        targetFoldingPanelsRect = (RectTransform)myRect.objectReferenceValue;
        targetButtonsRect = (RectTransform)myButtonsRect.objectReferenceValue;
        targetDragGripsRect = (RectTransform)myDragGripsRect.objectReferenceValue;
        targetContentsRect = (RectTransform)myContentsRect.objectReferenceValue;
    }

    public override void OnInspectorGUI()
    {
        if (targetFoldingPanel != null)
        {
            serializedObject.Update();

            // Draw script field.
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(scriptProperty);
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            // Draw properties
            GUILayout.Label("Resources", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Drag Grip Sprites", GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(dragGripSpriteLeft, GUIContent.none);
            EditorGUILayout.PropertyField(dragGripSpriteRight, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            GUILayout.Label("References", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(myRect);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(myDragGrip.displayName, GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(myDragGrip, GUIContent.none);
            EditorGUILayout.PropertyField(myDragGripsRect, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(myDragGripImage);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(myButton.displayName, GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(myButton, GUIContent.none);
            EditorGUILayout.PropertyField(myButtonsRect, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(myButtonText);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(myContent.displayName, GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(myContent, GUIContent.none);
            EditorGUILayout.PropertyField(myContentsRect, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            GUILayout.Label("Layout", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            cachedIsRightSidePanel = EditorGUILayout.Toggle(new GUIContent("Is Right Side Panel"), targetFoldingPanel.IsRightSidePanel);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(new Object[] { targetFoldingPanel, targetFoldingPanelsRect, targetButtonsRect, targetContentsRect }, "Change FoldingPanel Side");
                targetFoldingPanel.IsRightSidePanel = cachedIsRightSidePanel;
                SceneView.RepaintAll();
            }

            EditorGUI.BeginChangeCheck();
            cachedInteriorPadding = EditorGUILayout.Slider(new GUIContent("Interior Padding"), targetFoldingPanel.InteriorPadding, 0.0f, 10.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(new Object[] { targetFoldingPanel, targetFoldingPanelsRect, targetButtonsRect, targetContentsRect }, "Change FoldingPanel Interior Padding");
                targetFoldingPanel.InteriorPadding = cachedInteriorPadding;
            }

            EditorGUI.BeginChangeCheck();
            cachedButtomWidth = EditorGUILayout.Slider(new GUIContent("Button Width"), targetFoldingPanel.ButtonWidth, 10.0f, 50.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(new Object[] {targetFoldingPanel, targetFoldingPanelsRect, targetButtonsRect, targetContentsRect}, "Change FoldingPanel Button Width");
                targetFoldingPanel.ButtonWidth = cachedButtomWidth;
            }

            EditorGUI.BeginChangeCheck();
            cachedDragGripDistance = EditorGUILayout.Slider(new GUIContent("Drag Grip Distance"), targetFoldingPanel.DragGripDistance, 0.0f, 60.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(new Object[] {targetFoldingPanel, targetDragGripsRect}, "Change FoldingPanel Drag Grip Distance");
                targetFoldingPanel.DragGripDistance = cachedDragGripDistance;
            }

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 0.1f);

            EditorGUI.BeginChangeCheck();
            cachedMinOpenPanelWidth = EditorGUILayout.Slider(new GUIContent("Min Open Panel Width"), targetFoldingPanel.MinOpenPanelWidth, targetFoldingPanel.ClosedPanelWidth, targetFoldingPanel.OpenPanelWidth);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetFoldingPanel, "Change FoldingPanel Minimum Open Width");
                targetFoldingPanel.MinOpenPanelWidth = cachedMinOpenPanelWidth;
                SceneView.RepaintAll();
            }
            EditorGUI.BeginChangeCheck();
            cachedMaxOpenPanelWidth = EditorGUILayout.Slider(new GUIContent("Max Open Panel Width"), targetFoldingPanel.MaxOpenPanelWidth, targetFoldingPanel.OpenPanelWidth, Screen.currentResolution.width);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetFoldingPanel, "Change FoldingPanel Maximum Open Width");
                targetFoldingPanel.MaxOpenPanelWidth = cachedMaxOpenPanelWidth;
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            GUILayout.Label("Runtime", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Toggle(new GUIContent("Is Open"), targetFoldingPanel.IsOpen);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetFoldingPanel, "Toggle FoldingPanel Open");
                targetFoldingPanel.ToggleOpen();
                SceneView.RepaintAll();
            }

            EditorGUI.BeginChangeCheck();
            cachedOpenPanelWidth = EditorGUILayout.Slider(new GUIContent("Open Panel Width"), targetFoldingPanel.OpenPanelWidth, targetFoldingPanel.MinOpenPanelWidth, targetFoldingPanel.MaxOpenPanelWidth);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(new Object[] { targetFoldingPanel, targetFoldingPanelsRect, targetButtonsRect, targetContentsRect }, "Change FoldingPanel Open Width");
                targetFoldingPanel.OpenPanelWidth = cachedOpenPanelWidth;
            }
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(closedPanelWidth);
                EditorGUILayout.PropertyField(contentWidth);
                EditorGUILayout.PropertyField(contentHeight);
            }
            
            serializedObject.ApplyModifiedProperties();

            if (targetFoldingPanel.IsDirty)
            {
                targetFoldingPanel.UpdatePanelLayout(true);
                SceneView.RepaintAll();
            }


            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            if (GUILayout.Button(new GUIContent("Refresh Layout")))
            {
                Undo.RecordObject(targetFoldingPanel, "Refresh FoldingPanel Layout");
                targetFoldingPanel.UpdatePanelLayout(true);
                targetFoldingPanel.UpdatePanelSide();
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
        if (targetFoldingPanel != null)
        {
            Quaternion handleQuat = Quaternion.AngleAxis(90, Vector3.up);
            float handleSize = HandleUtility.GetHandleSize(targetFoldingPanelsRect.position) * 5.0f;

            Vector3 panelOutwardBottomPos;
            Vector3 panelWidthHandlePos;
            float sideInfluence;

            if (targetFoldingPanel.IsRightSidePanel)
            {
                panelOutwardBottomPos = targetFoldingPanelsRect.GetWorldSpaceBottomRightPoint();
                panelWidthHandlePos = targetFoldingPanelsRect.GetWorldSpaceMiddleLeftPoint();
                handleSize *= -1.0f;
            }
            else
            {
                panelOutwardBottomPos = targetFoldingPanelsRect.GetWorldSpaceBottomLeftPoint();
                panelWidthHandlePos = targetFoldingPanelsRect.GetWorldSpaceMiddleRightPoint();
            }


            // Open panel size (editable)
            Handles.color = Color.green;
            EditorUtilities.DrawSquare(panelOutwardBottomPos,
                                                        new Vector2(targetFoldingPanel.IsRightSidePanel ? -targetFoldingPanel.OpenPanelWidth : targetFoldingPanel.OpenPanelWidth,
                                                                    targetFoldingPanelsRect.sizeDelta.y));
            EditorGUI.BeginChangeCheck();
            cachedOpenPanelWidth = Handles.ScaleValueHandle(targetFoldingPanel.OpenPanelWidth, panelWidthHandlePos, handleQuat, handleSize, Handles.ArrowHandleCap, 1.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(new Object[] { targetFoldingPanel, targetFoldingPanelsRect, targetButtonsRect, targetContentsRect }, "Change FoldingPanel Open Width");
                targetFoldingPanel.OpenPanelWidth = cachedOpenPanelWidth;
                Repaint();
            }

            // Closed panel size
            Handles.color = Color.yellow;
            EditorUtilities.DrawSquare(panelOutwardBottomPos,
                                                        new Vector2(targetFoldingPanel.IsRightSidePanel ? -targetFoldingPanel.ClosedPanelWidth : targetFoldingPanel.ClosedPanelWidth,
                                                                    targetFoldingPanelsRect.sizeDelta.y));


            // Minimum panel size (editable)
            sideInfluence = targetFoldingPanel.IsRightSidePanel ? -targetFoldingPanel.MinOpenPanelWidth : targetFoldingPanel.MinOpenPanelWidth;
            panelWidthHandlePos.x = panelOutwardBottomPos.x + sideInfluence;

            Handles.color = Color.cyan;
            EditorUtilities.DrawSquare(panelOutwardBottomPos, new Vector2(sideInfluence, 
                                                                                          targetFoldingPanelsRect.sizeDelta.y));
            EditorGUI.BeginChangeCheck();
            cachedMinOpenPanelWidth = Handles.ScaleValueHandle(targetFoldingPanel.MinOpenPanelWidth, panelWidthHandlePos, handleQuat, handleSize, Handles.ArrowHandleCap, 1.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(new Object[] { targetFoldingPanel, targetFoldingPanelsRect, targetButtonsRect, targetContentsRect }, "Change FoldingPanel Minimum Open Width");
                targetFoldingPanel.MinOpenPanelWidth = cachedMinOpenPanelWidth;
                Repaint();
            }

            // Maximum panel size (editable)
            sideInfluence = targetFoldingPanel.IsRightSidePanel ? -targetFoldingPanel.MaxOpenPanelWidth : targetFoldingPanel.MaxOpenPanelWidth;
            panelWidthHandlePos.x = panelOutwardBottomPos.x + sideInfluence;

            Handles.color = Color.magenta;
            EditorUtilities.DrawSquare(panelOutwardBottomPos, new Vector2(sideInfluence,
                                                                                          targetFoldingPanelsRect.sizeDelta.y));
            EditorGUI.BeginChangeCheck();
            cachedMaxOpenPanelWidth = Handles.ScaleValueHandle(targetFoldingPanel.MaxOpenPanelWidth, panelWidthHandlePos, handleQuat, handleSize, Handles.ArrowHandleCap, 1.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(new Object[] { targetFoldingPanel, targetFoldingPanelsRect, targetButtonsRect, targetContentsRect }, "Change FoldingPanel Maximum Open Width");
                targetFoldingPanel.MaxOpenPanelWidth = cachedMaxOpenPanelWidth;
                Repaint();
            }
        }
    }
}
