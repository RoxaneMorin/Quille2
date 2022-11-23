using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Syrus.Plugins.ChartEditor;

// https://github.com/alessandroTironi/UnityGUIChartEditor

[CustomPropertyDrawer(typeof(PyramidFct))]
public class PyramidFctPropertyDrawer : PropertyDrawer
{
    // A shader is required to render the OpenGl stuff.
    Material material = new Material(Shader.Find("Hidden/Internal-Colored"));

    Color axeGrey = new Color(0.75f, 0.75f, 0.75f);
    Color gridGrey = new Color(0.45f, 0.45f, 0.45f);

    int graphHeight = 200;
    bool propFoldout = true;


    override public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
        PyramidFct currentPyramidFct = obj as PyramidFct;

        // Begin property, ajust height.
        EditorGUI.BeginProperty(position, label, property);
        position.height = EditorGUIUtility.singleLineHeight;

        // Begin foldout.
        propFoldout = EditorGUI.BeginFoldoutHeaderGroup(position, propFoldout, label);
        position.y += EditorGUIUtility.singleLineHeight;

        if (propFoldout)
        {
            // Display this instance's Vector2 properties.

            // Calculate rects
            var leftMostRect = new Rect(position.x, position.y, position.width, 20);
            var summitRect = new Rect(position.x, position.y + 20, position.width, 20);
            var rightmostRect = new Rect(position.x, position.y + 40, position.width, 20);

            // Generate labels
            var leftMostLabel = new GUIContent("Leftmost Point Coords: ");
            var summitLabel = new GUIContent("Summit Point Coords: ");
            var rightMostLabel = new GUIContent("Rightmost Point Coords: ");

            // Draw Vector2 properties' fields.
            EditorGUI.PropertyField(leftMostRect, property.FindPropertyRelative("leftMost"), leftMostLabel);
            EditorGUI.PropertyField(summitRect, property.FindPropertyRelative("summit"), summitLabel);
            EditorGUI.PropertyField(rightmostRect, property.FindPropertyRelative("rightMost"), rightMostLabel);


            // Prepare and display the graph.

            // Calculate the graph's rect.
            var graphRect = new Rect(position.x, position.y + 65, position.width - 10, graphHeight);

            // Parametrize and begin the graph.
            GUIChartEditor.BeginChart(graphRect, Color.gray,
                                      GUIChartEditorOptions.ChartBounds(-1.1f, 1.1f, -0.1f, 1.1f),
                                      GUIChartEditorOptions.SetOrigin(ChartOrigins.BottomLeft),
                                      GUIChartEditorOptions.ShowAxes(axeGrey),
                                      GUIChartEditorOptions.ShowGrid(0.2f, 0.2f, gridGrey, true));

            // Draw the lines!
            Vector2[] samples = new Vector2[] { currentPyramidFct.leftMost, currentPyramidFct.summit, currentPyramidFct.rightMost };
            GUIChartEditor.PushLineChart(samples, Color.white);

            GUIChartEditor.EndChart(); 
        }

        // Endings.
        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.EndProperty();
    }

    
    override public float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (propFoldout) ? base.GetPropertyHeight(property, label)+graphHeight+75 : EditorGUIUtility.singleLineHeight;
    }

}


