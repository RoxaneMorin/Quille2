using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using World;

[CustomPropertyDrawer(typeof(InteractionNeedEffect))]
public class InteractionNeedEffectDrawer : PropertyDrawer
{
    SerializedProperty targetNeed;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Fetch targetNeed to use in the label text.
        targetNeed = property.FindPropertyRelative("targetNeed");
        if (targetNeed != null)
        {
            Quille.BasicNeedSO targetNeedSO = targetNeed.objectReferenceValue as Quille.BasicNeedSO;
            label.text = string.Format("{0} : {1}", label.text, targetNeedSO.NeedName);
        }

        EditorGUI.BeginProperty(position, label, property);

        // If the property is expanded, draw a background box.
        float singleLineHeight = EditorGUIUtility.singleLineHeight;
        if (property.isExpanded)
        {
            Rect boxPositionRect = new Rect(position.x - singleLineHeight, position.y + singleLineHeight, position.width + singleLineHeight, position.height - singleLineHeight);
            GUI.Box(boxPositionRect, GUIContent.none, EditorStyles.helpBox);
        }

        // Draw the default property field.
        EditorGUI.PropertyField(position, property, label, true);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float foldoutHeight = EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.singleLineHeight/5f;
        return property.isExpanded ? foldoutHeight : EditorGUIUtility.singleLineHeight;
    }
}
