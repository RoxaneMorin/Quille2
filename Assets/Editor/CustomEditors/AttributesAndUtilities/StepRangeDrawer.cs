using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(StepRangeAttribute))]
public class StepRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        StepRangeAttribute stepRangeAttribute = attribute as StepRangeAttribute;

        if (property.propertyType == SerializedPropertyType.Float)
        {
            // Calculate the stepped float.
            float roundedValue = Mathf.Round(property.floatValue / stepRangeAttribute.Step) * stepRangeAttribute.Step;
            float newValue = EditorGUI.Slider(position, label, roundedValue, stepRangeAttribute.MinValue, stepRangeAttribute.MaxValue);

            // Apply it.
            if (newValue >= stepRangeAttribute.MaxValue)
            {
                property.floatValue = stepRangeAttribute.MaxValue;
            }
            else if (newValue != property.floatValue)
            {
                property.floatValue = newValue;
            }
        }
        else if (property.propertyType == SerializedPropertyType.Integer)
        {
            // Calculate the stepped int.
            float roundedMin = Mathf.Round(stepRangeAttribute.MinValue);
            float roundedMax = Mathf.Round(stepRangeAttribute.MaxValue);
            float roundedStep = Mathf.Round(stepRangeAttribute.Step);

            float toFloor = (property.intValue - roundedMin) / roundedStep + 1 / 2;
            int closestInt = (int)roundedMin + Mathf.FloorToInt(toFloor) * (int)roundedStep;
            int newValue = Mathf.FloorToInt(EditorGUI.Slider(position, label, closestInt, roundedMin, roundedMax));

            // Apply it.
            if (newValue >= roundedMax)
            {
                property.intValue = (int)roundedMax;
            }
            else if (newValue != property.intValue)
            {
                property.intValue = newValue;
            }
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "StepRangeAttribute is only useable with numerical types.");
        }
    }
}
