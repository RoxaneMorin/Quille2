using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomPropertyDrawer(typeof(Quille.BasicNeed), true)]
public class BasicNeedEditor : PropertyDrawer
{
    bool propFoldout = true;
    bool[] propFoldoutArray;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get target object.
        var targetObject = fieldInfo.GetValue(property.serializedObject.targetObject);


        // Prevent property fields from clipping beyond their display space.
        position.xMax = position.xMax - 10;



        
        // 
        if (targetObject.GetType().IsArray)
        {
            Quille.BasicNeed[] currentBasicNeedArray = targetObject as Quille.BasicNeed[];

            

            EditorGUI.BeginProperty(position, label, property);


            foreach (Quille.BasicNeed basicNeed in currentBasicNeedArray)
                DrawBasicNeedFoldout(position, property, label, basicNeed);


            EditorGUI.EndProperty();
        }
        // If it's not, 
        else
        {
            Quille.BasicNeed currentBasicNeed = targetObject as Quille.BasicNeed;


            EditorGUI.BeginProperty(position, label, property);


            DrawBasicNeedFoldout(position, property, label, currentBasicNeed);

            
            EditorGUI.EndProperty();
        }
        



        

        
    }


    override public float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (propFoldout) ? base.GetPropertyHeight(property, label) + 135 : EditorGUIUtility.singleLineHeight;
    }


    private void DrawBasicNeedFoldout(Rect position, SerializedProperty property, GUIContent label, Quille.BasicNeed currentBasicNeed)
    {
        // Set property label to display the need's name.
        label = new GUIContent(string.Format("Basic Need: {0}", (currentBasicNeed.NeedName)));

        // Begin property, ajust height.
        //EditorGUI.BeginProperty(position, label, property);
        position.height = EditorGUIUtility.singleLineHeight;

        // Begin foldout.
        propFoldout = EditorGUI.BeginFoldoutHeaderGroup(position, propFoldout, label);

        position.y += EditorGUIUtility.singleLineHeight;

        if (propFoldout)
        {
            position.x += 10;

            // Calculate rects
            var aiPriorityWeightingRect = new Rect(position.x, position.y + 5, position.width, 20);

            var levelFullRect = new Rect(position.x, position.y + 30, position.width, 20);
            var levelCurrentRect = new Rect(position.x, position.y + 50, position.width, 20);

            var defaultDecayRateRect = new Rect(position.x, position.y + 75, position.width, 20);
            var baseDecayRateRect = new Rect(position.x, position.y + 95, position.width, 20);
            var currentDecayRateRect = new Rect(position.x, position.y + 115, position.width, 20);


            // Generate labels
            var aiPriorityWeightingLabel = new GUIContent("AI Prioritization Weight: ");

            var levelFullLabel = new GUIContent("Maximum Fulfillment Level:");
            var levelCurrentLabel = new GUIContent("Current Fulfillment Level:");

            var defaultChangeRateLabel = new GUIContent("Default Change Rate:");
            var defaultChangeRateValueLabel = new GUIContent(currentBasicNeed.DefaultChangeRate.ToString());
            var baseChangeRateLabel = new GUIContent("Base Change Rate:");
            var currentChangeRateLabel = new GUIContent("Current Change Rate:");


            // Draw the properties themselves.

            // AI Priority Weighting.
            EditorGUI.PropertyField(aiPriorityWeightingRect, property.FindPropertyRelative("aiPriorityWeighting"), aiPriorityWeightingLabel);


            // Maximum Fullfilment Level.
            EditorGUI.PropertyField(levelFullRect, property.FindPropertyRelative("levelFull"), levelFullLabel);

            // Ensure maximum fulfillment level is always one or larger.
            if (currentBasicNeed.LevelFull < 1)
            {
                property.FindPropertyRelative("levelFull").floatValue = 1;
            }

            // Update the value of the LevelCurrent slider as needed.
            if (currentBasicNeed.LevelCurrent > currentBasicNeed.LevelFull)
            {
                property.FindPropertyRelative("levelCurrent").floatValue = property.FindPropertyRelative("levelFull").floatValue;
            }

            // Current Fullfilment Level.
            EditorGUI.Slider(levelCurrentRect, property.FindPropertyRelative("levelCurrent"), currentBasicNeed.LevelEmpty, currentBasicNeed.LevelFull, levelCurrentLabel);


            // Default Decay Rate (read only)
            EditorGUI.LabelField(defaultDecayRateRect, defaultChangeRateLabel, defaultChangeRateValueLabel);

            // Current Character's Base Decay Rate.
            EditorGUI.PropertyField(baseDecayRateRect, property.FindPropertyRelative("baseChangeRate"), baseChangeRateLabel);

            // Current Character's Actual Decay Rate.
            EditorGUI.PropertyField(currentDecayRateRect, property.FindPropertyRelative("currentChangeRate"), currentChangeRateLabel);

        }

        // Endings.
        EditorGUI.EndFoldoutHeaderGroup();
    }
}


