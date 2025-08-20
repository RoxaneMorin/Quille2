using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(floatPair))]
public class FloatPairDrawer : PropertyDrawer
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

        var item1Rect = new Rect(position.x, position.y, itemWidth, position.height);
        var item2Rect = new Rect(position.x + itemWidth + 2, position.y, itemWidth, position.height);

        var item1Prop = property.FindPropertyRelative("Item1");
        var item2Prop = property.FindPropertyRelative("Item2");

        EditorGUI.PropertyField(item1Rect, item1Prop, GUIContent.none);
        EditorGUI.PropertyField(item2Rect, item2Prop, GUIContent.none);

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
