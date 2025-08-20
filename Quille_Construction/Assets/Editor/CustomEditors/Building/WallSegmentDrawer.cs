using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(Building.WallSegment))]
//public class WallSegmentDrawer : PropertyDrawer
//{
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        EditorGUI.BeginProperty(position, label, property);

//        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

//        var indent = EditorGUI.indentLevel;
//        EditorGUI.indentLevel = 0;

//        // Handle the shifting rects.
//        float itemWidth = position.width / 2;
//        itemWidth = itemWidth < 0 ? 0 : itemWidth;

//        var anchorARect = new Rect(position.x, position.y, itemWidth, position.height);
//        var anchorBRect = new Rect(position.x + itemWidth + 2, position.y, itemWidth, position.height);

//        var anchorAProp = property.FindPropertyRelative("anchorA");
//        var anchorBProp = property.FindPropertyRelative("anchorB");

//        EditorGUI.PropertyField(anchorARect, anchorAProp, GUIContent.none);
//        EditorGUI.PropertyField(anchorBRect, anchorBProp, GUIContent.none);

//        EditorGUI.indentLevel = indent;
//        EditorGUI.EndProperty();
//    }
//}
