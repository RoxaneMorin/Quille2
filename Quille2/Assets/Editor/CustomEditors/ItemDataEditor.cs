using Quille;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// 
public class ItemDataEditor : Editor
{
    // VARIABLES
    // Object References
    protected ItemData targetItemData;

    // Serialized Properties
    protected SerializedProperty scriptProperty;

    protected SerializedProperty itemName;
    protected SerializedProperty itemDescription;
    protected SerializedProperty itemIcon;
    //protected SerializedProperty itemColour;
    protected SerializedProperty sortingIndex;

    // Misc
    protected GUIStyle iconGUIStyle;
    protected float halfLine;

    protected string cachedDescription;



    // METHODS
    protected void OnEnable()
    {
        // Fetch the target ItemData proper.
        targetItemData = serializedObject.targetObject as ItemData;

        // Fetch serialized properties
        scriptProperty = serializedObject.FindProperty("m_Script");

        itemName = serializedObject.FindProperty("itemName");
        itemDescription = serializedObject.FindProperty("itemDescription");
        itemIcon = serializedObject.FindProperty("itemIcon");
        //itemColour = serializedObject.FindProperty("itemColour");

        sortingIndex = serializedObject.FindProperty("sortingIndex");

        // Misc
        halfLine = EditorGUIUtility.singleLineHeight / 2f;
    }

    protected void DrawItemDataProperties(bool includeScriptProperty = false)
    {
        if (includeScriptProperty)
        {
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(scriptProperty);
            GUILayout.Space(halfLine);
        }

        // The properties proper
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(itemName, new GUIContent("Name"));
        EditorGUILayout.PropertyField(sortingIndex, new GUIContent("Index"));
        EditorGUILayout.PropertyField(itemIcon, new GUIContent("Icon"));
        //EditorGUILayout.PropertyField(itemColour, new GUIContent("Colour"));

        EditorGUILayout.EndVertical();
        if (targetItemData.ItemIcon != null)
        {
            float iconDim = EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing * 2f;
            Rect iconRect = GUILayoutUtility.GetAspectRect(1.0f, GUILayout.MaxHeight(iconDim), GUILayout.MaxWidth(iconDim));

            iconGUIStyle = new GUIStyle("sv_iconselector_back");
            iconGUIStyle.alignment = TextAnchor.MiddleCenter;
            iconGUIStyle.overflow = new RectOffset(0, 0, -2, 2);
            iconGUIStyle.padding = new RectOffset(4, 4, 2, -2);

            GUI.Box(iconRect, targetItemData.ItemIcon.texture, iconGUIStyle);
        
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
        }
        else
        {
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Description");
        GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
        EditorGUI.BeginChangeCheck();
        cachedDescription = EditorGUILayout.TextArea(targetItemData.ItemDescription, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2f));
        if (EditorGUI.EndChangeCheck())
        {
            targetItemData.ItemDescription = cachedDescription;
        }
        EditorGUILayout.EndHorizontal();
    }
}
