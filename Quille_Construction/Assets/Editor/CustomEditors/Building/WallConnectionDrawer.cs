using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Building.WallConnection))]
public class WallConnectionDrawer : PropertyDrawer
{
    SerializedProperty connectedWallSegment;
    SerializedProperty connectedAnchor;
    SerializedProperty angle;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Change the label text to the connecterAnchor's name.
        connectedAnchor = property.FindPropertyRelative("connectedAnchor");
        label.text += string.Format(" ({0})", connectedAnchor.objectReferenceValue);

        EditorGUI.BeginProperty(position, label, property);

        // Draw the foldout header
        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

        // If the foldout is open, draw everything else.
        if (property.isExpanded)
        {
            // Collect the remaining properties.
            connectedWallSegment = property.FindPropertyRelative("connectedWallSegment");
            angle = property.FindPropertyRelative("angle");

            // Draw properties.
            Rect newPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 1.25f, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(newPosition, connectedWallSegment);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, connectedAnchor);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, angle);
        }

        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return property.isExpanded ? EditorGUI.GetPropertyHeight(property) : EditorGUIUtility.singleLineHeight;
    }
}
