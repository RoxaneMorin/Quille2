using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(SliderRadial), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the Slider Component.
    /// Extend this class to write a custom editor for a component derived from Slider.
    /// </summary>
    public class SliderRadialEditor : SelectableEditor
    {
        SerializedProperty m_Shape;
        SerializedProperty m_Origin;
        SerializedProperty m_Clockwise;
        SerializedProperty m_HandleShift;

        SerializedProperty m_OriginalWidthAndHeight;
        SerializedProperty m_RootRect;
        SerializedProperty m_BackgroundRect;
        SerializedProperty m_FillRect;
        SerializedProperty m_HandleRect;

        SerializedProperty m_BackgroundSprite360;
        SerializedProperty m_FillSprite360;
        SerializedProperty m_BackgroundSprite180;
        SerializedProperty m_FillSprite180;
        SerializedProperty m_BackgroundSprite90;
        SerializedProperty m_FillSprite90;

        SerializedProperty m_MinValue;
        SerializedProperty m_MaxValue;
        SerializedProperty m_WholeNumbers;
        SerializedProperty m_Value;
        SerializedProperty m_OnValueChanged;

        SerializedProperty testCenterPoint;
        SerializedProperty testInitialPoint;
        //SerializedProperty testHandlePoint;


        // TODO: add button & function to place the handle based on the latest params.

        private class Styles
        {
            public static GUIContent text = EditorGUIUtility.TrTextContent("Fill Origin");
            public static GUIContent[] Origin90Style =
            {
                EditorGUIUtility.TrTextContent("BottomLeft"),
                EditorGUIUtility.TrTextContent("TopLeft"),
                EditorGUIUtility.TrTextContent("TopRight"),
                EditorGUIUtility.TrTextContent("BottomRight")
            };

            public static GUIContent[] Origin180Style =
            {
                EditorGUIUtility.TrTextContent("Bottom"),
                EditorGUIUtility.TrTextContent("Left"),
                EditorGUIUtility.TrTextContent("Top"),
                EditorGUIUtility.TrTextContent("Right")
            };

            public static GUIContent[] Origin360Style =
            {
                EditorGUIUtility.TrTextContent("Bottom"),
                EditorGUIUtility.TrTextContent("Right"),
                EditorGUIUtility.TrTextContent("Top"),
                EditorGUIUtility.TrTextContent("Left")
            };
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_OriginalWidthAndHeight = serializedObject.FindProperty("m_OriginalWidthAndHeight");
            m_RootRect = serializedObject.FindProperty("m_RootRect");
            m_BackgroundRect = serializedObject.FindProperty("m_BackgroundRect");
            m_FillRect = serializedObject.FindProperty("m_FillRect");
            m_HandleRect = serializedObject.FindProperty("m_HandleRect");

            testCenterPoint = serializedObject.FindProperty("testCenterPoint");
            testInitialPoint = serializedObject.FindProperty("testInitialPoint");
            //testHandlePoint = serializedObject.FindProperty("testHandlePoint");

            m_Shape = serializedObject.FindProperty("m_Shape");
            m_Origin = serializedObject.FindProperty("m_Origin");
            m_Clockwise = serializedObject.FindProperty("m_Clockwise");
            m_HandleShift = serializedObject.FindProperty("m_HandleShift");

            m_BackgroundSprite360 = serializedObject.FindProperty("m_BackgroundSprite360");
            m_FillSprite360 = serializedObject.FindProperty("m_FillSprite360");
            m_BackgroundSprite180 = serializedObject.FindProperty("m_BackgroundSprite180");
            m_FillSprite180 = serializedObject.FindProperty("m_FillSprite180");
            m_BackgroundSprite90 = serializedObject.FindProperty("m_BackgroundSprite90");
            m_FillSprite90 = serializedObject.FindProperty("m_FillSprite90");

            m_MinValue = serializedObject.FindProperty("m_MinValue");
            m_MaxValue = serializedObject.FindProperty("m_MaxValue");
            m_WholeNumbers = serializedObject.FindProperty("m_WholeNumbers");
            m_Value = serializedObject.FindProperty("m_Value");
            m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.LabelField("Rect References", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(m_RootRect);
            EditorGUILayout.PropertyField(m_OriginalWidthAndHeight);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_BackgroundRect);
            EditorGUILayout.PropertyField(m_FillRect);
            EditorGUILayout.PropertyField(m_HandleRect);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Sprite References", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_BackgroundSprite360);
            EditorGUILayout.PropertyField(m_FillSprite360);
            EditorGUILayout.PropertyField(m_BackgroundSprite180);
            EditorGUILayout.PropertyField(m_FillSprite180);
            EditorGUILayout.PropertyField(m_BackgroundSprite90);
            EditorGUILayout.PropertyField(m_FillSprite90);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Testing", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(testCenterPoint);
            EditorGUILayout.PropertyField(testInitialPoint);
            //EditorGUILayout.PropertyField(testHandlePoint);

            if (m_FillRect.objectReferenceValue != null || m_HandleRect.objectReferenceValue != null)
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Slider Parameters", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_Shape);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects(serializedObject.targetObjects, "Change Slider Shape");

                    SliderRadial.Shape shape = (SliderRadial.Shape)m_Shape.enumValueIndex;
                    foreach (var obj in serializedObject.targetObjects)
                    {
                        SliderRadial slider = obj as SliderRadial;
                        slider.SetShape(shape);
                    }
                }

                EditorGUI.BeginChangeCheck();
                var shapeRect = EditorGUILayout.GetControlRect(true);
                switch ((SliderRadial.Shape)m_Shape.enumValueIndex)
                {
                    case SliderRadial.Shape.Quarter90:
                        EditorGUI.Popup(shapeRect, m_Origin, Styles.Origin90Style, Styles.text);
                        break;
                    case SliderRadial.Shape.Half180:
                        EditorGUI.Popup(shapeRect, m_Origin, Styles.Origin180Style, Styles.text);
                        break;
                    case SliderRadial.Shape.Full360:
                        EditorGUI.Popup(shapeRect, m_Origin, Styles.Origin360Style, Styles.text);
                        break;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects(serializedObject.targetObjects, "Change Slider Origin");

                    int originID = m_Origin.intValue;
                    foreach (var obj in serializedObject.targetObjects)
                    {
                        SliderRadial slider = obj as SliderRadial;
                        slider.SetOrigin(originID);
                    }
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_HandleShift);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects(serializedObject.targetObjects, "Change Handle Shift");

                    foreach (var obj in serializedObject.targetObjects)
                    {
                        SliderRadial slider = obj as SliderRadial;
                        slider.SetAllData();
                        // Do we need to make it update the graphics here?
                    }
                }

                //EditorGUILayout.PropertyField(m_Origin);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_Clockwise);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects(serializedObject.targetObjects, "Change Slider Direction");

                    bool isClockwise = m_Clockwise.boolValue;
                    foreach (var obj in serializedObject.targetObjects)
                    {
                        SliderRadial slider = obj as SliderRadial;
                        slider.SetClockwise(isClockwise);
                    }
                }

                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();
                float newMin = EditorGUILayout.FloatField("Min Value", m_MinValue.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    if (m_WholeNumbers.boolValue ? Mathf.Round(newMin) < m_MaxValue.floatValue : newMin < m_MaxValue.floatValue)
                    {
                        m_MinValue.floatValue = newMin;
                        if (m_Value.floatValue < newMin)
                            m_Value.floatValue = newMin;
                    }
                }

                EditorGUI.BeginChangeCheck();
                float newMax = EditorGUILayout.FloatField("Max Value", m_MaxValue.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    if (m_WholeNumbers.boolValue ? Mathf.Round(newMax) > m_MinValue.floatValue : newMax > m_MinValue.floatValue)
                    {
                        m_MaxValue.floatValue = newMax;
                        if (m_Value.floatValue > newMax)
                            m_Value.floatValue = newMax;
                    }
                }

                EditorGUILayout.PropertyField(m_WholeNumbers);

                bool areMinMaxEqual = (m_MinValue.floatValue == m_MaxValue.floatValue);

                if (areMinMaxEqual)
                    EditorGUILayout.HelpBox("Min Value and Max Value cannot be equal.", MessageType.Warning);

                if (m_WholeNumbers.boolValue)
                    m_Value.floatValue = Mathf.Round(m_Value.floatValue);

                EditorGUI.BeginDisabledGroup(areMinMaxEqual);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.Slider(m_Value, m_MinValue.floatValue, m_MaxValue.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    // Apply the change before sending the event
                    serializedObject.ApplyModifiedProperties();

                    foreach (var t in targets)
                    {
                        if (t is Slider slider)
                        {
                            slider.onValueChanged?.Invoke(slider.value);
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();

                bool warning = false;
                foreach (var obj in serializedObject.targetObjects)
                {
                    Slider slider = obj as Slider;
                    Slider.Direction dir = slider.direction;
                    if (dir == Slider.Direction.LeftToRight || dir == Slider.Direction.RightToLeft)
                        warning = (slider.navigation.mode != Navigation.Mode.Automatic && (slider.FindSelectableOnLeft() != null || slider.FindSelectableOnRight() != null));
                    else
                        warning = (slider.navigation.mode != Navigation.Mode.Automatic && (slider.FindSelectableOnDown() != null || slider.FindSelectableOnUp() != null));
                }

                if (warning)
                    EditorGUILayout.HelpBox("The selected slider direction conflicts with navigation. Not all navigation options may work.", MessageType.Warning);

                // Draw the event notification options
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_OnValueChanged);
            }
            else
            {
                EditorGUILayout.HelpBox("Specify a RectTransform for the slider fill or the slider handle or both. Each must have a parent RectTransform that it can slide within.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

