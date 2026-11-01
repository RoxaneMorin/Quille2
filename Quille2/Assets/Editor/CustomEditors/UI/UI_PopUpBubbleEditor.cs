using QuilleUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(QuilleUI.UI_PopUpBubble))]
public class UI_PopUpBubbleEditor : Editor
{
    // VARIABLES
    // Object References
    private UI_PopUpBubble targetPopUpBubble;
    private TMP_Text targetTextMain;
    private TMP_Text targetTextOverflow;
    private Image targetIcon;

    private RectTransform targetPopUpBubbleRect;
    private RectTransform targetOwnerRect;
    private RectTransform targetTextMainRect;
    private RectTransform targetTextOverflowRect;
    private RectTransform targetIconRect;

    // Serialized Properties
    private SerializedProperty scriptProperty;

    private SerializedProperty myOwner;
    private SerializedProperty myTail;
    private SerializedProperty myTailsImage;
    private SerializedProperty myTextMain;
    private SerializedProperty myTextOverflow;
    private SerializedProperty myIcon;

    private SerializedProperty myRect;
    private SerializedProperty myOwnersRect;
    private SerializedProperty myTailsRect;
    private SerializedProperty myTextMainsRect;
    private SerializedProperty myTextOverflowsRect;
    private SerializedProperty myIconsRect;

    private SerializedProperty tailSpritePointingDown;
    private SerializedProperty tailSpritePointingUp;
    private SerializedProperty tailSpritePointingLeft;
    private SerializedProperty tailSpritePointingRight;

    private SerializedProperty hasIcon;
    private SerializedProperty tailDistance;
    private SerializedProperty distanceFromOwner;
    private SerializedProperty bubbleRatio;

    private SerializedProperty ownerScreenPosXRightOfPivot;
    private SerializedProperty ownerScreenPosXLeftOfPivot;
    private SerializedProperty ownerScreenPosYTopOfPivot;
    private SerializedProperty ownerScreenPosYBottomOfPivot;

    private SerializedProperty currentDirection;

    // Locally cached values

    private float cachedFontSize;

    private float cachedBubblePadding;



    private string cachedTextContent;
    private Sprite cachedIconSprite;


    Vector3[] rectWorldCorners;



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
        myTextMain = serializedObject.FindProperty("myTextMain");
        myTextOverflow = serializedObject.FindProperty("myTextOverflow");
        myIcon = serializedObject.FindProperty("myIcon");

        myRect = serializedObject.FindProperty("myRect");
        myOwnersRect = serializedObject.FindProperty("myOwnersRect");
        myTailsRect = serializedObject.FindProperty("myTailsRect");
        myTextMainsRect = serializedObject.FindProperty("myTextMainsRect");
        myTextOverflowsRect = serializedObject.FindProperty("myTextOverflowsRect");
        myIconsRect = serializedObject.FindProperty("myIconsRect");

        hasIcon = serializedObject.FindProperty("hasIcon");
        tailDistance = serializedObject.FindProperty("tailDistance");
        distanceFromOwner = serializedObject.FindProperty("distanceFromOwner");
        bubbleRatio = serializedObject.FindProperty("bubbleRatio");

        ownerScreenPosXRightOfPivot = serializedObject.FindProperty("ownerScreenPosXRightOfPivot");
        ownerScreenPosXLeftOfPivot = serializedObject.FindProperty("ownerScreenPosXLeftOfPivot");
        ownerScreenPosYTopOfPivot = serializedObject.FindProperty("ownerScreenPosYTopOfPivot");
        ownerScreenPosYBottomOfPivot = serializedObject.FindProperty("ownerScreenPosYBottomOfPivot");

        currentDirection = serializedObject.FindProperty("currentDirection");

        // Fetch rects from serialized properties
        targetTextMain = (TMP_Text)myTextMain.objectReferenceValue;
        targetTextOverflow = (TMP_Text)myTextOverflow.objectReferenceValue;
        targetIcon = (Image)myIcon.objectReferenceValue;

        targetPopUpBubbleRect = (RectTransform)myRect.objectReferenceValue;
        targetOwnerRect = (RectTransform)myOwnersRect.objectReferenceValue;
        targetTextMainRect = (RectTransform)myTextMainsRect.objectReferenceValue;
        targetTextOverflowRect = (RectTransform)myTextOverflowsRect.objectReferenceValue;
        targetIconRect = (RectTransform)myIconsRect.objectReferenceValue;


        // Misc
        rectWorldCorners = new Vector3[4];

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
            EditorGUILayout.PropertyField(myOwner, GUIContent.none);
            EditorGUILayout.PropertyField(myOwnersRect, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(myTail.displayName, GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(myTail, GUIContent.none);
            EditorGUILayout.PropertyField(myTailsRect, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(myTailsImage);

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

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(myIcon.displayName, GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(myIcon, GUIContent.none);
            EditorGUILayout.PropertyField(myIconsRect, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            GUILayout.Label("Layout", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            cachedBubblePadding = EditorGUILayout.Slider(new GUIContent("Interior Padding"), targetPopUpBubble.InteriorPadding, 0.0f, 20.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Interior Padding");

                targetPopUpBubble.InteriorPadding = cachedBubblePadding;

                targetPopUpBubble.UpdateBubbleDimensions();
            }

            EditorGUI.BeginChangeCheck();
            cachedFontSize = EditorGUILayout.Slider(new GUIContent("Font Size"), targetTextMain.fontSize, 10.0f, 30.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Font Size");
                targetTextMain.fontSize = targetTextOverflow.fontSize = cachedFontSize;
                targetPopUpBubble.UpdateBubbleDimensions();
            }

            EditorGUILayout.PropertyField(hasIcon);
            EditorGUILayout.PropertyField(tailDistance);
            EditorGUILayout.PropertyField(distanceFromOwner);


            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(bubbleRatio);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Ratio");
                targetPopUpBubble.UpdateBubbleDimensions();
            }


            // TODO:
            // Icon size
            // Tail size


            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

            GUILayout.Label("Contents", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Icon"), GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUI.BeginChangeCheck();
            cachedIconSprite = (Sprite)EditorGUILayout.ObjectField(targetIcon.sprite, typeof(Sprite), false);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Icon Sprite");
                targetIcon.sprite = cachedIconSprite;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Text"), GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUI.BeginChangeCheck();
            cachedTextContent = GUILayout.TextArea(targetTextMain.text);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPopUpBubble, "Change PopUpBubble Text Content");
                targetPopUpBubble.SetBubbleText(cachedTextContent);
            }
            EditorGUILayout.EndHorizontal();

            

            

            serializedObject.ApplyModifiedProperties();

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

            Handles.color = Color.white;

            // Main text bounds
            targetTextMainRect.GetWorldCorners(rectWorldCorners);
            Handles.DrawSolidRectangleWithOutline(rectWorldCorners, new Color32(250, 250, 0, 25), new Color32(250, 250, 0, 255));

            if (targetTextMain.overflowMode == TextOverflowModes.Overflow)
            {
                // Overflow text actual length
                Vector2 renderedSize = targetTextMain.GetRenderedValues();
                rectWorldCorners[0] = rectWorldCorners[1] - new Vector3(0.0f, renderedSize.y, 0.0f);
                rectWorldCorners[3] = rectWorldCorners[2] - new Vector3(0.0f, renderedSize.y, 0.0f);
                Handles.DrawSolidRectangleWithOutline(rectWorldCorners, new Color32(255, 255, 0, 10), new Color32(255, 255, 0, 50));
            }

            if (hasIcon.boolValue)
            {
                // Icon bounds
                targetIconRect.GetWorldCorners(rectWorldCorners);
                Handles.DrawSolidRectangleWithOutline(rectWorldCorners, new Color32(0, 250, 0, 25), new Color32(0, 250, 0, 255));

                if (targetTextOverflow.enabled)
                {
                    // Overflow text bounds
                    targetTextOverflowRect.GetWorldCorners(rectWorldCorners);
                    Handles.DrawSolidRectangleWithOutline(rectWorldCorners, new Color32(255, 150, 0, 25), new Color32(255, 150, 0, 255));

                    // Overflow text actual length
                    Vector2 renderedSize = targetTextOverflow.GetRenderedValues();
                    rectWorldCorners[0] = rectWorldCorners[1] - new Vector3(0.0f, renderedSize.y, 0.0f);
                    rectWorldCorners[3] = rectWorldCorners[2] - new Vector3(0.0f, renderedSize.y, 0.0f);
                    Handles.DrawSolidRectangleWithOutline(rectWorldCorners, new Color32(255, 150, 0, 10), new Color32(255, 150, 0, 50));
                }
            }
        }
    }
}
