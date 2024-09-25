using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Quille.Person_NeedController.NeedController_Data))]
public class NeedControllerDataDrawer : PropertyDrawer
{
    // Mainly exists to hide the indent/nature of the NeedController_Data subclass.

    SerializedProperty myBasicNeeds;
    SerializedProperty mySubjectiveNeeds;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        myBasicNeeds = property.FindPropertyRelative("myBasicNeeds");
        mySubjectiveNeeds = property.FindPropertyRelative("mySubjectiveNeeds");

        EditorGUI.BeginProperty(position, label, property);

        // Draw properties.
        EditorGUI.PropertyField(position, myBasicNeeds);
        position.y += EditorGUI.GetPropertyHeight(myBasicNeeds);
        EditorGUI.PropertyField(position, mySubjectiveNeeds);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float foldoutHeight = 0;

        if (myBasicNeeds != null)
            foldoutHeight += EditorGUI.GetPropertyHeight(myBasicNeeds);
        if (mySubjectiveNeeds != null)
            foldoutHeight += EditorGUI.GetPropertyHeight(mySubjectiveNeeds);

        return foldoutHeight;
    }
}