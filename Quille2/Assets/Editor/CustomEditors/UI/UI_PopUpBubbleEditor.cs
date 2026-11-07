using QuilleUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(QuilleUI.UI_PopUpBubble))]
public class UI_PopUpBubbleEditor : Editor
{
    // VARIABLES
    // Object References
    private UI_PopUpBubble targetPopUpBubble;
    private UI_ReflowableTextbox targetReflowableTextbox;

    private RectTransform targetPopUpBubbleRect;
    private RectTransform targetOwnerRect;
    private RectTransform targetReflowableTextboxRect;

    // Serialized Properties
    private SerializedProperty scriptProperty;

    private SerializedProperty myOwner;
    private SerializedProperty myTail;
    private SerializedProperty myTailsImage;
    private SerializedProperty myReflowableTextbox;

    private SerializedProperty myRect;
    private SerializedProperty myOwnersRect;
    private SerializedProperty myTailsRect;
    private SerializedProperty myReflowableTextboxsRect;

    private SerializedProperty tailSpritePointingDown;
    private SerializedProperty tailSpritePointingUp;
    private SerializedProperty tailSpritePointingLeft;
    private SerializedProperty tailSpritePointingRight;

    private SerializedProperty ownerScreenPosXRightOfPivot;
    private SerializedProperty ownerScreenPosXLeftOfPivot;
    private SerializedProperty ownerScreenPosYTopOfPivot;
    private SerializedProperty ownerScreenPosYBottomOfPivot;

    private SerializedProperty currentDirection;

    // Local and cached variables 
    private GameObject cachedOwner;

    private Sprite cachedIconSprite;
    private string cachedTextContent;

    private bool cachedHasIcon;
    private Vector2 cachedDimensionRatio;
    private float cachedInteriorPadding;
    private Vector2 cachedIconSize;
    private float cachedFontSize;

    private float cachedDistanceFromOwner;
    private float cachedTailDistance;
    private Vector2 cachedTailSize;

    private FourDirections cachedCurrentDirection;


    // METHODS
    private void OnEnable()
    {
        // Fetch the target PopUpBubble proper.
        targetPopUpBubble = serializedObject.targetObject as UI_PopUpBubble;
        if (targetPopUpBubble != null)
        {
        }

        // Fetch serialized properties
        scriptProperty = serializedObject.FindProperty("m_Script");

        tailSpritePointingDown = serializedObject.FindProperty("tailSpritePointingDown");
        tailSpritePointingUp = serializedObject.FindProperty("tailSpritePointingUp");
        tailSpritePointingLeft = serializedObject.FindProperty("tailSpritePointingLeft");
        tailSpritePointingRight = serializedObject.FindProperty("tailSpritePointingRight");

        myOwner = serializedObject.FindProperty("myOwner");
        myTail = serializedObject.FindProperty("myTail");
        myTailsImage = serializedObject.FindProperty("myTailsImage");
        myReflowableTextbox = serializedObject.FindProperty("myReflowableTextbox");

        myRect = serializedObject.FindProperty("myRect");
        myOwnersRect = serializedObject.FindProperty("myOwnersRect");
        myTailsRect = serializedObject.FindProperty("myTailsRect");
        myReflowableTextboxsRect = serializedObject.FindProperty("myReflowableTextboxsRect");

        ownerScreenPosXRightOfPivot = serializedObject.FindProperty("ownerScreenPosXRightOfPivot");
        ownerScreenPosXLeftOfPivot = serializedObject.FindProperty("ownerScreenPosXLeftOfPivot");
        ownerScreenPosYTopOfPivot = serializedObject.FindProperty("ownerScreenPosYTopOfPivot");
        ownerScreenPosYBottomOfPivot = serializedObject.FindProperty("ownerScreenPosYBottomOfPivot");

        currentDirection = serializedObject.FindProperty("currentDirection");

        // Fetch rects from serialized properties
        targetReflowableTextbox = (UI_ReflowableTextbox)myReflowableTextbox.objectReferenceValue;

        targetPopUpBubbleRect = (RectTransform)myRect.objectReferenceValue;
        targetOwnerRect = (RectTransform)myOwnersRect.objectReferenceValue;
        targetReflowableTextboxRect = (RectTransform)myReflowableTextboxsRect.objectReferenceValue;
    }


    public override void OnInspectorGUI()
    {
        if (targetPopUpBubble != null)
        {
            serializedObject.Update();

            // Draw script field.
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(scriptProperty);
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            // Draw properties
            GUILayout.Label("Resources", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tail Sprites", GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(tailSpritePointingUp, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(EditorGUIUtility.labelWidth)); // Is there a better way to do this than with an empty label?
            EditorGUILayout.PropertyField(tailSpritePointingLeft, GUIContent.none);
            EditorGUILayout.PropertyField(tailSpritePointingRight, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(tailSpritePointingDown, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            GUILayout.Label("References", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(myRect);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(myOwner.displayName, GUILayout.Width(EditorGUIUtility.labelWidth));
            //EditorGUILayout.PropertyField(myOwner, GUIContent.none);

            EditorGUI.BeginChangeCheck();
            cachedOwner = (GameObject)EditorGUILayout.ObjectField(targetPopUpBubble.Owner, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Owner");
                targetPopUpBubble.Owner = cachedOwner;
            }

            
            EditorGUILayout.PropertyField(myOwnersRect, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            // TODO: auto update of owner data


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(myReflowableTextbox.displayName, GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(myReflowableTextbox, GUIContent.none);
            EditorGUILayout.PropertyField(myReflowableTextboxsRect, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(myTail.displayName, GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(myTail, GUIContent.none);
            EditorGUILayout.PropertyField(myTailsRect, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(myTailsImage);


            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            GUILayout.Label("Contents", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Icon"), GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUI.BeginChangeCheck();
            cachedIconSprite = (Sprite)EditorGUILayout.ObjectField(targetPopUpBubble.IconSprite, typeof(Sprite), false);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Icon Sprite");
                targetPopUpBubble.IconSprite = cachedIconSprite;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Text"), GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUI.BeginChangeCheck();
            cachedTextContent = GUILayout.TextArea(targetPopUpBubble.TextContent);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Text Content");
                targetPopUpBubble.TextContent = cachedTextContent;
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            GUILayout.Label("Layout", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            cachedDimensionRatio = EditorGUILayout.Vector2Field("Dimension Ratio", targetPopUpBubble.DimensionRatio);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Dimension Ratio");
                targetPopUpBubble.DimensionRatio = cachedDimensionRatio;
            }

            EditorGUI.BeginChangeCheck();
            cachedInteriorPadding = EditorGUILayout.Slider("Interior Padding", targetPopUpBubble.InteriorPadding, 0.0f, 50.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Interior Padding");
                targetPopUpBubble.InteriorPadding = cachedInteriorPadding;
            }

            EditorGUI.BeginChangeCheck();
            cachedHasIcon = EditorGUILayout.Toggle("Has Icon", targetPopUpBubble.HasIcon);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Toggle PopUpBubble Icon");
                targetPopUpBubble.HasIcon = cachedHasIcon;
            }
            EditorGUI.BeginChangeCheck();
            cachedIconSize = EditorGUILayout.Vector2Field("Icon Size", targetPopUpBubble.IconSize);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Icon Size");
                targetPopUpBubble.IconSize = cachedIconSize;
            }

            EditorGUI.BeginChangeCheck();
            cachedFontSize = EditorGUILayout.Slider("Font Size", targetPopUpBubble.FontSize, 5.0f, 50.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Font Size");
                targetPopUpBubble.FontSize = cachedFontSize;
            }

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 4f);

            EditorGUI.BeginChangeCheck();
            cachedDistanceFromOwner = EditorGUILayout.Slider("Distance from Owner", targetPopUpBubble.DistanceFromOwner, -50.0f, 100.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Distance from Owner");
                targetPopUpBubble.DistanceFromOwner = cachedDistanceFromOwner;
            }

            EditorGUI.BeginChangeCheck();
            cachedTailDistance = EditorGUILayout.Slider("Tail Distance", targetPopUpBubble.TailDistance, -50.0f, 100.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Tail Distance");
                targetPopUpBubble.TailDistance = cachedTailDistance;
            }

            EditorGUI.BeginChangeCheck();
            cachedTailSize = EditorGUILayout.Vector2Field("Tail Size", targetPopUpBubble.TailSize);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Tail Size");
                targetPopUpBubble.TailSize = cachedTailSize;
            }


            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            EditorGUI.BeginChangeCheck();
            cachedCurrentDirection = (FourDirections)EditorGUILayout.EnumPopup("Current Direction", targetPopUpBubble.CurrentDirection);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Current Direction");
                targetPopUpBubble.CurrentDirection = cachedCurrentDirection;
            }


            serializedObject.ApplyModifiedProperties();

            if (targetPopUpBubble.IsDirty)
            {
                targetPopUpBubble.UpdateLayout();
            }

        }
        else
        {
            // Fallback in case of wonkiness.
            DrawDefaultInspector();
        }
    }


    // TODO:
    // Button to recalculate owner data if an owner is set
    // Button to set the current dir in editor
    // Changing the tail and owner distances changes the items in editor

    private void OnSceneGUI()
    {
        if (targetPopUpBubble != null)
        {
            float handleSize = HandleUtility.GetHandleSize(targetOwnerRect.position) * 0.08f;

            // Draw the owner's side positions.
            Handles.DrawSolidDisc(new Vector3(ownerScreenPosXLeftOfPivot.floatValue, targetOwnerRect.position.y, 0.0f), Vector3.forward, handleSize);
            Handles.DrawSolidDisc(new Vector3(ownerScreenPosXRightOfPivot.floatValue, targetOwnerRect.position.y, 0.0f), Vector3.forward, handleSize);
            Handles.DrawSolidDisc(new Vector3(targetOwnerRect.position.x, ownerScreenPosYBottomOfPivot.floatValue, 0.0f), Vector3.forward, handleSize);
            Handles.DrawSolidDisc(new Vector3(targetOwnerRect.position.x, ownerScreenPosYTopOfPivot.floatValue, 0.0f), Vector3.forward, handleSize);

            
        }
    }
}
