using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(proceduralGrid.CoordPair))]
public class CoordPairDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Handle the shifting rects.
        float itemWidth = position.width / 2;
        itemWidth = itemWidth < 0 ? 0 : itemWidth;

        var itemXRect = new Rect(position.x, position.y, itemWidth, position.height);
        var itemZRect = new Rect(position.x + itemWidth + 2, position.y, itemWidth, position.height);

        var itemXProp = property.FindPropertyRelative("x");
        var itemZProp = property.FindPropertyRelative("z");

        EditorGUI.PropertyField(itemXRect, itemXProp, GUIContent.none);
        EditorGUI.PropertyField(itemZRect, itemZProp, GUIContent.none);

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
