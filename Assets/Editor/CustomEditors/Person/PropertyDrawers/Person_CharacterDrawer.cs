using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Quille.Person_Character))]
public class Person_CharacterDrawer : PropertyDrawer
{
    bool _foldoutOpen = true; // Open by default.

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
        float singleLineHeight = EditorGUIUtility.singleLineHeight;

        EditorGUI.BeginProperty(position, label, property);

        _foldoutOpen = EditorGUI.Foldout(position, _foldoutOpen, label, "FoldoutHeader");

        EditorGUILayout.BeginFadeGroup(_foldoutOpen ? 1f : 0.001f);
        if (_foldoutOpen)
        {
            // Draw the background box.
            Rect boxPositionRect = new Rect(position.x - singleLineHeight/2f, position.y + singleLineHeight * 2f, position.width + singleLineHeight / 2f, position.height - singleLineHeight * 3f);
            GUI.Box(boxPositionRect, GUIContent.none, EditorStyles.helpBox);


            // Collect properties.
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
            GUIContent headerLabel = new GUIContent(string.Format("{0} {1}'s Person_Character", firstName.stringValue, lastName.stringValue));
            if (lastName.stringValue == "")
            {
                // Handle special cases.
                if (firstName.stringValue == "")
                {
                    headerLabel.text = "Unnamed Person_Character";
                }
                else
                {
                    headerLabel.text = string.Format("{0}'s Person_Character", firstName.stringValue);
                }
            }
            Rect newPosition = new Rect(position.x, position.y + singleLineHeight, position.width - singleLineHeight / 2f, singleLineHeight);
            EditorGUI.LabelField(newPosition, headerLabel, style: "ProfilerRightPane");


            // IDENTITY

            // NAMES
            newPosition.y += singleLineHeight * 1.25f;
            EditorGUI.LabelField(newPosition, "Names", EditorStyles.boldLabel);

            // Draw properties.
            EditorGUI.indentLevel++;
            newPosition.y += singleLineHeight;
            EditorGUI.PropertyField(newPosition, firstName, true);
            newPosition.y += singleLineHeight;
            EditorGUI.PropertyField(newPosition, lastName, true);
            newPosition.y += singleLineHeight;
            EditorGUI.PropertyField(newPosition, nickName, true);
            newPosition.y += singleLineHeight * 1.5f;
            EditorGUI.PropertyField(newPosition, secondaryNames);
            newPosition.y += EditorGUI.GetPropertyHeight(secondaryNames);
            EditorGUI.indentLevel--;

            // TODO: Add extra Identity properties here as they are added to Person_Character.


            // PERSONALITY
            newPosition.y += singleLineHeight * 0.25f;
            EditorGUI.LabelField(newPosition, "Personality", EditorStyles.boldLabel);

            // Draw properties and relevant controls.
            EditorGUI.indentLevel++;

            // Personality Axes.
            newPosition.y += singleLineHeight * 1.25f;
            // Options to populate and clear the personality axe dict in editor.
            Rect buttonPosition = new Rect(newPosition.x, newPosition.y, newPosition.width / 2, singleLineHeight);
            if (GUI.Button(buttonPosition, "Populate Personality Axes (Zeroes)"))
            {
                Quille.Person_Character personCharacter = fieldInfo.GetValue(property.serializedObject.targetObject) as Quille.Person_Character;
                personCharacter.PopulatePersonalityAxesDict(false);
            }
            buttonPosition.x += newPosition.width / 2;
            if (GUI.Button(buttonPosition, "Populate Personality Axes (Randomized)"))
            {
                Quille.Person_Character personCharacter = fieldInfo.GetValue(property.serializedObject.targetObject) as Quille.Person_Character;
                personCharacter.PopulatePersonalityAxesDict(true);
            }
            newPosition.y += singleLineHeight;
            if (GUI.Button(newPosition, "Clear All Personality Axes"))
            {
                Quille.Person_Character personCharacter = fieldInfo.GetValue(property.serializedObject.targetObject) as Quille.Person_Character;
                personCharacter.ClearPersonalityAxesDict();
            }
            newPosition.y += singleLineHeight * 1.25f;
            EditorGUI.PropertyField(newPosition, myPersonalityAxes);

            // Personality Traits.
            newPosition.y += EditorGUI.GetPropertyHeight(myPersonalityAxes) + singleLineHeight * 0.75f;
            buttonPosition = new Rect(newPosition.x, newPosition.y, newPosition.width / 2, singleLineHeight);
            if (GUI.Button(buttonPosition, "Random Populate Personality Traits (Ones)"))
            {
                Quille.Person_Character personCharacter = fieldInfo.GetValue(property.serializedObject.targetObject) as Quille.Person_Character;
                personCharacter.RandomPopulatePersonalityTraitsDict(false);
            }
            buttonPosition.x += newPosition.width / 2;
            if (GUI.Button(buttonPosition, "Random Populate Personality Taits (Randomized)"))
            {
                Quille.Person_Character personCharacter = fieldInfo.GetValue(property.serializedObject.targetObject) as Quille.Person_Character;
                personCharacter.RandomPopulatePersonalityTraitsDict(true);
            }
            newPosition.y += singleLineHeight;
            if (GUI.Button(newPosition, "Clear All Personality Traits"))
            {
                Quille.Person_Character personCharacter = fieldInfo.GetValue(property.serializedObject.targetObject) as Quille.Person_Character;
                personCharacter.ClearPersonalityTraitsDict();
            }
            newPosition.y += singleLineHeight * 1.25f;
            EditorGUI.PropertyField(newPosition, myPersonalityTraits);

            // Drives.
            newPosition.y += EditorGUI.GetPropertyHeight(myPersonalityTraits) + singleLineHeight * 0.75f;
            buttonPosition = new Rect(newPosition.x, newPosition.y, newPosition.width / 2, singleLineHeight);
            if (GUI.Button(buttonPosition, "Random Populate Drives (Ones)"))
            {
                Quille.Person_Character personCharacter = fieldInfo.GetValue(property.serializedObject.targetObject) as Quille.Person_Character;
                personCharacter.RandomPopulateDrivesDict(false);
            }
            buttonPosition.x += newPosition.width / 2;
            if (GUI.Button(buttonPosition, "Random Populate Drives (Randomized)"))
            {
                Quille.Person_Character personCharacter = fieldInfo.GetValue(property.serializedObject.targetObject) as Quille.Person_Character;
                personCharacter.RandomPopulateDrivesDict(true);
            }
            newPosition.y += singleLineHeight;
            if (GUI.Button(newPosition, "Clear All Drives"))
            {
                Quille.Person_Character personCharacter = fieldInfo.GetValue(property.serializedObject.targetObject) as Quille.Person_Character;
                personCharacter.ClearDrivesDict();
            }
            newPosition.y += singleLineHeight * 1.25f;
            EditorGUI.PropertyField(newPosition, myDrives);

            // Interests.
            newPosition.y += EditorGUI.GetPropertyHeight(myDrives) + singleLineHeight * 0.75f;
            buttonPosition = new Rect(newPosition.x, newPosition.y, newPosition.width / 2, singleLineHeight);
            if (GUI.Button(buttonPosition, "Random Populate Interests (Zeroes)"))
            {
                Quille.Person_Character personCharacter = fieldInfo.GetValue(property.serializedObject.targetObject) as Quille.Person_Character;
                personCharacter.RandomPopulateInterestsDict(false);
            }
            buttonPosition.x += newPosition.width / 2;
            if (GUI.Button(buttonPosition, "Random Populate Drives (Randomized)"))
            {
                Quille.Person_Character personCharacter = fieldInfo.GetValue(property.serializedObject.targetObject) as Quille.Person_Character;
                personCharacter.RandomPopulateInterestsDict(true);
            }
            newPosition.y += singleLineHeight;
            if (GUI.Button(newPosition, "Clear All Interests"))
            {
                Quille.Person_Character personCharacter = fieldInfo.GetValue(property.serializedObject.targetObject) as Quille.Person_Character;
                personCharacter.ClearInterestDict();
            }
            newPosition.y += singleLineHeight * 1.25f;
            EditorGUI.PropertyField(newPosition, myInterests);

            newPosition.y += EditorGUI.GetPropertyHeight(myInterests);
            EditorGUI.indentLevel--;

            // TODO: Add extra Personality properties here as they are added to Person_Character.


            // Draw separator.
            newPosition.y -= singleLineHeight * 0.5f;
            EditorGUI.LabelField(newPosition, "", style: "ProfilerRightPane");
        }

        EditorGUILayout.EndFadeGroup();
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (_foldoutOpen)
        {
            float openPropertyHeight = EditorGUIUtility.singleLineHeight * 21f;

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
