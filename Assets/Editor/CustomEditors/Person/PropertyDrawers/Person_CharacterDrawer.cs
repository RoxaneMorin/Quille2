using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Quille.Person_Character))]
public class Person_CharacterDrawer : PropertyDrawer
{
    bool _foldoutOpen;

    SerializedProperty charID;
    SerializedProperty firstName;
    SerializedProperty lastName;
    SerializedProperty nickName;
    SerializedProperty secondaryNames;

    SerializedProperty myPersonalityAxes;
    SerializedProperty myPersonalityTraits;
    SerializedProperty myDrives;
    SerializedProperty myInterests;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        _foldoutOpen = EditorGUI.Foldout(position, _foldoutOpen, label, "FoldoutHeader");

        EditorGUILayout.BeginFadeGroup(_foldoutOpen ? 1f : 0.001f);
        if (_foldoutOpen)
        {
            // Collect properties.
            charID = property.FindPropertyRelative("charID");
            firstName = property.FindPropertyRelative("firstName");
            lastName = property.FindPropertyRelative("lastName");
            nickName = property.FindPropertyRelative("nickName");
            secondaryNames = property.FindPropertyRelative("secondaryNames");

            // Collect properties.
            myPersonalityAxes = property.FindPropertyRelative("myPersonalityAxes");
            myPersonalityTraits = property.FindPropertyRelative("myPersonalityTraits");
            myDrives = property.FindPropertyRelative("myDrives");
            myInterests = property.FindPropertyRelative("myInterests");


            // Make and draw label field.
            GUIContent headerLabel = new GUIContent(string.Format("{1} {2} (#{0})'s Person_Character", charID.intValue, firstName.stringValue, lastName.stringValue));
            Rect newPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(newPosition, headerLabel, style: "ProfilerRightPane");


            // IDENTITY

            // charID.
            
            newPosition.y += 1.5f * EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, charID, true);

            // NAMES
            newPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;
            EditorGUI.LabelField(newPosition, "Names", EditorStyles.boldLabel);

            // Draw properties.
            EditorGUI.indentLevel++;
            newPosition.y += EditorGUIUtility.singleLineHeight / 2f;
            EditorGUI.PropertyField(newPosition, firstName, true);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, lastName, true);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, nickName, true);
            newPosition.y += EditorGUIUtility.singleLineHeight * 1.5f;
            EditorGUI.PropertyField(newPosition, secondaryNames);
            newPosition.y += EditorGUI.GetPropertyHeight(secondaryNames);
            EditorGUI.indentLevel--;

            // TODO: Add extra Identity properties here as they are added to Person_Character.


            // PERSONALITY
            newPosition.y += EditorGUIUtility.singleLineHeight * 0.25f;
            EditorGUI.LabelField(newPosition, "Personality", EditorStyles.boldLabel);

            // Draw properties.
            EditorGUI.indentLevel++;
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, myPersonalityAxes);
            newPosition.y += EditorGUI.GetPropertyHeight(myPersonalityAxes) + EditorGUIUtility.singleLineHeight * 0.25f;
            EditorGUI.PropertyField(newPosition, myPersonalityTraits);
            newPosition.y += EditorGUI.GetPropertyHeight(myPersonalityTraits) + EditorGUIUtility.singleLineHeight * 0.25f;
            EditorGUI.PropertyField(newPosition, myDrives);
            newPosition.y += EditorGUI.GetPropertyHeight(myDrives) + EditorGUIUtility.singleLineHeight * 0.25f;
            EditorGUI.PropertyField(newPosition, myInterests);
            newPosition.y += EditorGUI.GetPropertyHeight(myInterests);
            EditorGUI.indentLevel--;

            // TODO: Add extra Personality properties here as they are added to Person_Character.


            // Draw separator.
            newPosition.y -= EditorGUIUtility.singleLineHeight * 0.5f;
            EditorGUI.LabelField(newPosition, "", style: "ProfilerRightPane");
        }

        EditorGUILayout.EndFadeGroup();
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (_foldoutOpen)
        {
            float openPropertyHeight = EditorGUIUtility.singleLineHeight * 10.5f;

            if (secondaryNames != null)
                openPropertyHeight += EditorGUI.GetPropertyHeight(secondaryNames);
            else openPropertyHeight += EditorGUIUtility.singleLineHeight;

            if (myPersonalityAxes != null)
                openPropertyHeight += EditorGUI.GetPropertyHeight(myPersonalityAxes);
            else openPropertyHeight += EditorGUIUtility.singleLineHeight;

            if (myPersonalityTraits != null)
                openPropertyHeight += EditorGUI.GetPropertyHeight(myPersonalityTraits);
            else openPropertyHeight += EditorGUIUtility.singleLineHeight;

            if (myDrives != null)
                openPropertyHeight += EditorGUI.GetPropertyHeight(myDrives);
            else openPropertyHeight += EditorGUIUtility.singleLineHeight;

            if (myInterests != null)
                openPropertyHeight += EditorGUI.GetPropertyHeight(myInterests);
            else openPropertyHeight += EditorGUIUtility.singleLineHeight;

            return openPropertyHeight;
        }

        return EditorGUIUtility.singleLineHeight;
    }
}
