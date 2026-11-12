using Quille;
using QuilleUI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Quille.Thought))]
public class ThoughtEditor : Editor
{
    // VARIABLES
    // Object References
    protected Thought targetThought;

    // Serialized Properties
    protected SerializedProperty scriptProperty;

    protected SerializedProperty data;
    protected SerializedProperty totalLifespan;
    protected SerializedProperty remainingLifespan;
    protected SerializedProperty decayRate;

    // Misc
    public bool drawScriptProperty = true;
    bool _thoughtDataFoldoutOpen = true; // Open by default.

    private GUIStyle customGUIStyle;




    // METHODS
    private void OnEnable()
    {
        // Fetch the target Thought proper.
        targetThought = serializedObject.targetObject as Thought;

        // Fetch serialized properties
        scriptProperty = serializedObject.FindProperty("m_Script");

        data = serializedObject.FindProperty("data");
        totalLifespan = serializedObject.FindProperty("totalLifespan");
        remainingLifespan = serializedObject.FindProperty("remainingLifespan");
        decayRate = serializedObject.FindProperty("decayRate");

    }

    public override void OnInspectorGUI()
    {
        if (targetThought != null)
        {
            serializedObject.Update();

            if (drawScriptProperty)
            {
                // Draw script field.
                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.PropertyField(scriptProperty);
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);
            }


            // Data
            Rect foldoutRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            float foldoutSpace = EditorGUIUtility.labelWidth + 2.0f;
            Rect propertyRect = new Rect(foldoutRect.position + new Vector2(foldoutSpace, 0.0f), new Vector2(foldoutRect.width - foldoutSpace, EditorGUIUtility.singleLineHeight));
            foldoutRect.width = EditorGUIUtility.labelWidth;

            _thoughtDataFoldoutOpen = EditorGUI.Foldout(foldoutRect, _thoughtDataFoldoutOpen, "Thought Data", "FoldoutHeader");
            EditorGUI.PropertyField(propertyRect, data, GUIContent.none);

            EditorGUILayout.BeginFadeGroup(_thoughtDataFoldoutOpen ? 1f : 0.001f);
            if (_thoughtDataFoldoutOpen)
            {
                customGUIStyle = new GUIStyle("framebox");
                customGUIStyle.padding = new RectOffset(8, 8, 8, 8);
                EditorGUILayout.BeginHorizontal(customGUIStyle);

                EditorGUILayout.BeginVertical();
                GUILayout.Label(targetThought.ItemName, "BottomShadowInwards", GUILayout.ExpandWidth(true));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                GUILayout.Label($"\"{targetThought.ItemDescription}\"");
                EditorGUILayout.EndVertical();

                if (targetThought.ItemIcon != null)
                {
                    foldoutRect = GUILayoutUtility.GetAspectRect(1.0f, GUILayout.MaxHeight(50.0f), GUILayout.MaxWidth(50.0f));
                    EditorUtilities.DrawBorderedRect(foldoutRect, 1, EditorStyles.label.normal.textColor, targetThought.ItemColor);

                    customGUIStyle = new GUIStyle("sv_iconselector_back");
                    customGUIStyle.padding = new RectOffset(8, 8, 8, 8);
                    customGUIStyle.overflow = new RectOffset(-3, -3, -3, -3);
                    GUI.Box(foldoutRect, targetThought.ItemIcon.texture, customGUIStyle);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);
            }
            EditorGUILayout.EndFadeGroup();


            GUILayout.Label("Instance Values", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Lifespan", GUILayout.Width(EditorGUIUtility.labelWidth));
            GUILayout.Label("Remaining");
            EditorGUILayout.PropertyField(remainingLifespan, GUIContent.none);
            GUILayout.Label("/ Total");
            EditorGUILayout.PropertyField(totalLifespan, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(decayRate);

            serializedObject.ApplyModifiedProperties();
        }
        else
        {
            DrawDefaultInspector();
        }
    }
}
