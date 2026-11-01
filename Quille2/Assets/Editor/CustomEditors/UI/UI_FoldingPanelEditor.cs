using QuilleUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuilleUI.UI_FoldingPanel))]
public class UI_FoldingPanelEditor : Editor
{
    // VARIABLES
    // Object References
    private UI_FoldingPanel targetFoldingPanel;
    private RectTransform targetFoldingPanelsRect;

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

    private SerializedProperty currentClosedPanelWidth;
    private SerializedProperty currentContentWidth;
    private SerializedProperty currentContentHeight;

    // Locally cached values
    private float cachedMinOpenPanelWidth;
    private float cachedMaxOpenPanelWidth;
    private float cachedCurrentOpenPanelWidth;
    private float cachedCurrentInteriorPadding;
    private float cachedCurrentButtomWidth;
    private float cachedCurrentDragGripDistance;
    private bool cachedIsRightSidePanel;



    // METHODS
    private void OnEnable()
    {
        // Fetch the target FoldingPanel proper.
        targetFoldingPanel = serializedObject.targetObject as UI_FoldingPanel;
        if (targetFoldingPanel != null)
        {
            targetFoldingPanelsRect = (RectTransform)targetFoldingPanel.transform;
        }

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

        currentClosedPanelWidth = serializedObject.FindProperty("currentClosedPanelWidth");
        currentContentWidth = serializedObject.FindProperty("currentContentWidth");
        currentContentHeight = serializedObject.FindProperty("currentContentHeight");
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
                Undo.RecordObject(targetFoldingPanel, "Change Folding Panel Side");
                targetFoldingPanel.SetPanelSide(cachedIsRightSidePanel);
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 0.1f);

            EditorGUI.BeginChangeCheck();
            cachedMinOpenPanelWidth = EditorGUILayout.Slider(new GUIContent("Min Open Panel Width"), targetFoldingPanel.MinOpenPanelWidth, targetFoldingPanel.CurrentClosedPanelWidth, targetFoldingPanel.CurrentOpenPanelWidth);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetFoldingPanel, "Change Folding Panel Minimum Width");
                targetFoldingPanel.MinOpenPanelWidth = cachedMinOpenPanelWidth;
                SceneView.RepaintAll();
            }
            EditorGUI.BeginChangeCheck();
            cachedMaxOpenPanelWidth = EditorGUILayout.Slider(new GUIContent("Max Open Panel Width"), targetFoldingPanel.MaxOpenPanelWidth, targetFoldingPanel.CurrentOpenPanelWidth, Screen.currentResolution.width);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetFoldingPanel, "Change Folding Panel Maximum Width");
                targetFoldingPanel.MaxOpenPanelWidth = cachedMaxOpenPanelWidth;
                SceneView.RepaintAll();
            }

            EditorGUI.BeginChangeCheck();
            cachedCurrentOpenPanelWidth = EditorGUILayout.Slider(new GUIContent("Current Open Panel Width"), targetFoldingPanel.CurrentOpenPanelWidth, targetFoldingPanel.MinOpenPanelWidth, targetFoldingPanel.MaxOpenPanelWidth);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetFoldingPanel, "Change Folding Panel Current Width");
                targetFoldingPanel.UpdatePanelWidth(cachedCurrentOpenPanelWidth);
                SceneView.RepaintAll();
            }
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(currentClosedPanelWidth);

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 0.1f);

            EditorGUI.BeginChangeCheck();
            cachedCurrentInteriorPadding = EditorGUILayout.Slider(new GUIContent("Interior Padding"), targetFoldingPanel.CurrentInteriorPadding, 0.0f, 10.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetFoldingPanel, "Change Interior Padding");
                targetFoldingPanel.UpdatePadding(cachedCurrentInteriorPadding);
                SceneView.RepaintAll();
            }
            EditorGUI.BeginChangeCheck();
            cachedCurrentButtomWidth = EditorGUILayout.Slider(new GUIContent("Button Width"), targetFoldingPanel.CurrentButtonWidth, 10.0f, 50.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetFoldingPanel, "Change Button Width");
                targetFoldingPanel.UpdateButtonWidth(cachedCurrentButtomWidth);
                SceneView.RepaintAll();
            }
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(currentContentWidth);
                EditorGUILayout.PropertyField(currentContentHeight);
            }

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 0.1f);

            EditorGUI.BeginChangeCheck();
            cachedCurrentDragGripDistance = EditorGUILayout.Slider(new GUIContent("Drag Grip Distance"), targetFoldingPanel.CurrentDragGripDistance, 0.0f, 60.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetFoldingPanel, "Change Drag Grip Distance");
                targetFoldingPanel.UpdateDragGripDistance(cachedCurrentDragGripDistance);
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 0.1f);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Toggle(new GUIContent("Is Open"), targetFoldingPanel.IsOpen);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetFoldingPanel, "Toggle Folding Panel Open");
                targetFoldingPanel.ToggleOpen();
                SceneView.RepaintAll();
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
        if (targetFoldingPanel != null)
        {
            Vector3 panelWidthPreviewSize;
            Vector3 panelWidthPreviewPos;
            Vector3 panelWidthHandlePos;

            Quaternion handleQuat = Quaternion.AngleAxis(90, Vector3.up);
            float handleSize = HandleUtility.GetHandleSize(targetFoldingPanelsRect.position) * 5.0f;

            // Open panel size.
            panelWidthPreviewSize = new Vector3(targetFoldingPanel.CurrentOpenPanelWidth, targetFoldingPanelsRect.sizeDelta.y, 0.0f);
            if (targetFoldingPanel.IsRightSidePanel)
            {
                panelWidthPreviewPos = targetFoldingPanelsRect.position + new Vector3(-panelWidthPreviewSize.x / 2.0f, 0.0f, 0.0f);
                panelWidthHandlePos = targetFoldingPanelsRect.position + new Vector3(-panelWidthPreviewSize.x, 0.0f, 0.0f);

                handleSize *= -1.0f;
            }
            else
            {
                panelWidthPreviewPos = targetFoldingPanelsRect.position + new Vector3(panelWidthPreviewSize.x / 2.0f, 0.0f, 0.0f);
                panelWidthHandlePos = targetFoldingPanelsRect.position + new Vector3(panelWidthPreviewSize.x, 0.0f, 0.0f);
            }

            Handles.color = Color.green;
            Handles.DrawWireCube(panelWidthPreviewPos, panelWidthPreviewSize);

            EditorGUI.BeginChangeCheck();
            cachedCurrentOpenPanelWidth = Handles.ScaleValueHandle(targetFoldingPanel.CurrentOpenPanelWidth, panelWidthHandlePos, handleQuat, handleSize, Handles.ArrowHandleCap, 1.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetFoldingPanel, "Change Folding Panel Open Width");
                targetFoldingPanel.UpdatePanelWidth(cachedCurrentOpenPanelWidth);
                Repaint();
            }


            // Minimum (closed) panel size.
            panelWidthPreviewSize = new Vector3(targetFoldingPanel.MinOpenPanelWidth, targetFoldingPanelsRect.sizeDelta.y, 0.0f);
            if (targetFoldingPanel.IsRightSidePanel)
            {
                panelWidthPreviewPos = targetFoldingPanelsRect.position + new Vector3(-panelWidthPreviewSize.x / 2.0f, 0.0f, 0.0f);
                panelWidthHandlePos = targetFoldingPanelsRect.position + new Vector3(-panelWidthPreviewSize.x, 0.0f, 0.0f);
            }
            else
            {
                panelWidthPreviewPos = targetFoldingPanelsRect.position + new Vector3(panelWidthPreviewSize.x / 2.0f, 0.0f, 0.0f);
                panelWidthHandlePos = targetFoldingPanelsRect.position + new Vector3(panelWidthPreviewSize.x, 0.0f, 0.0f);
            }

            Handles.color = Color.cyan;
            Handles.DrawWireCube(panelWidthPreviewPos, panelWidthPreviewSize);

            EditorGUI.BeginChangeCheck();
            cachedMinOpenPanelWidth = Handles.ScaleValueHandle(targetFoldingPanel.MinOpenPanelWidth, panelWidthHandlePos, handleQuat, handleSize, Handles.ArrowHandleCap, 1.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetFoldingPanel, "Change Folding Panel Minimum Width");
                targetFoldingPanel.MinOpenPanelWidth = cachedMinOpenPanelWidth;
                Repaint();
            }


            // Maximum panel size.
            panelWidthPreviewSize.x = targetFoldingPanel.MaxOpenPanelWidth;

            if (targetFoldingPanel.IsRightSidePanel)
            {
                panelWidthPreviewPos = targetFoldingPanelsRect.position + new Vector3(-panelWidthPreviewSize.x / 2.0f, 0.0f, 0.0f);
                panelWidthHandlePos = targetFoldingPanelsRect.position + new Vector3(-panelWidthPreviewSize.x, 0.0f, 0.0f);
            }
            else
            {
                panelWidthPreviewPos = targetFoldingPanelsRect.position + new Vector3(panelWidthPreviewSize.x / 2.0f, 0.0f, 0.0f);
                panelWidthHandlePos = targetFoldingPanelsRect.position + new Vector3(panelWidthPreviewSize.x, 0.0f, 0.0f);
            }

            Handles.color = Color.magenta;
            Handles.DrawWireCube(panelWidthPreviewPos, panelWidthPreviewSize);

            EditorGUI.BeginChangeCheck();
            cachedMaxOpenPanelWidth = Handles.ScaleValueHandle(targetFoldingPanel.MaxOpenPanelWidth, panelWidthHandlePos, handleQuat, handleSize, Handles.ArrowHandleCap, 1.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetFoldingPanel, "Change Folding Panel Maximum Width");
                targetFoldingPanel.MaxOpenPanelWidth = cachedMaxOpenPanelWidth;
                Repaint();
            }
        }
    }
}
