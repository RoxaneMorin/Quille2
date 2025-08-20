using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Quille;

public class SOMenuIndexUtilityWindow : EditorWindow
{
    [MenuItem("Quille/SO MenuIndex Update Utility")]
    public static void OpenWindow()
    {
        SOMenuIndexUtilityWindow window = GetWindow<SOMenuIndexUtilityWindow>();
        window.titleContent = new GUIContent("SO MenuIndex Update Utility");
    }


    // THE UI PROPER
    float singleLineHeight = EditorGUIUtility.singleLineHeight;
    float halfLineHeight = EditorGUIUtility.singleLineHeight / 2f;
    float loadButtonWidth = EditorGUIUtility.singleLineHeight * 3f;

    Vector2 scrollPosition = Vector2.zero;

    SerializedObject thisAsSerializedObject;
    SerializedProperty theSOsAsSerializedProperty;
    [SerializeField] MenuSortableScriptableObject[] theSOs;

    string[] itemTypes;
    Dictionary<int, string> itemPathsByIndex;
    int typeIndex = -1;

    public void OnEnable()
    {
        thisAsSerializedObject = new SerializedObject(this);
        theSOsAsSerializedProperty = thisAsSerializedObject.FindProperty("theSOs");

        itemTypes = new string[] 
        {
            "Personality Axes", 
            "Personality Traits", 
            "Personality Trait Domains", 
            "Drives", 
            "Drive Domains", 
            "Interests", 
            "Interest Domains", 
            "Skin Colour Genes",
            "Hair Colour Genes",
            "Eye Colour Genes"
        };

        itemPathsByIndex = new Dictionary<int, string>()
        {
            {0, Constants_PathResources.SO_PATH_PERSONALITYAXES},
            {1, Constants_PathResources.SO_PATH_PERSONALITYTRAITS},
            {2, Constants_PathResources.SO_PATH_PERSONALITYTRAITDOMAINS},
            {3, Constants_PathResources.SO_PATH_DRIVES},
            {4, Constants_PathResources.SO_PATH_DRIVEDOMAINS},
            {5, Constants_PathResources.SO_PATH_INTERESTS},
            {6, Constants_PathResources.SO_PATH_INTERESTDOMAINS},
            {7, Constants_PathResources.SO_PATH_SKINCOLOURS},
            {8, Constants_PathResources.SO_PATH_HAIRCOLOURS},
            {9, Constants_PathResources.SO_PATH_EYECOLOURS}
        };
    }

    public void OnGUI()
    {
        Rect contentRect = new Rect(halfLineHeight, halfLineHeight, position.width - singleLineHeight, singleLineHeight);
        GUI.Label(contentRect, "Select Type to Edit...");

        // Type selection.
        Rect dropDownRect = new Rect(contentRect.x, contentRect.y + singleLineHeight, contentRect.width - loadButtonWidth, singleLineHeight);
        typeIndex = EditorGUI.Popup(dropDownRect, typeIndex, itemTypes);
        dropDownRect.x += dropDownRect.width;
        dropDownRect.width = loadButtonWidth;

        GUI.enabled = (typeIndex != -1);
        if (GUI.Button(dropDownRect, "Load"))
        {
            LoadScriptableObjects(typeIndex);
        }
        //GUI.enabled = true;

        contentRect.y += singleLineHeight * 2.5f;

        // Display list.
        GUI.enabled = (theSOs != null && theSOs.Length > 0);

        Rect scrollviewRect = new Rect(contentRect.x, contentRect.y, contentRect.width, position.height - singleLineHeight * 5f);
        Rect scrollviewContentRect = new Rect(0, 0, contentRect.width - singleLineHeight, EditorGUI.GetPropertyHeight(theSOsAsSerializedProperty));
        scrollPosition = GUI.BeginScrollView(scrollviewRect, scrollPosition, scrollviewContentRect, false, true);

        EditorGUI.BeginProperty(position, GUIContent.none, theSOsAsSerializedProperty);
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(scrollviewContentRect, theSOsAsSerializedProperty, new GUIContent("Relevant ScriptableObjects"));
        if (EditorGUI.EndChangeCheck())
        {
            thisAsSerializedObject.ApplyModifiedProperties();
        }
        EditorGUI.EndProperty();
        GUI.EndScrollView();
        
        contentRect.y += scrollviewRect.height + halfLineHeight;

        // Confirm.
        if (GUI.Button(contentRect, "Apply New Order"))
        {
            UpdateScriptableObjects();
        }
        //GUI.enabled = true;
    }

    private void OnDestroy()
    {
        thisAsSerializedObject = null;
        theSOsAsSerializedProperty = null;
        theSOs = null;
    }

    public void LoadScriptableObjects(int typeIndex)
    {
        if (itemPathsByIndex.ContainsKey(typeIndex))
        {
            theSOs = Resources.LoadAll<MenuSortableScriptableObject>(itemPathsByIndex[typeIndex]).OrderBy(so => so.MenuSortingIndex).ToArray();
            thisAsSerializedObject.Update();
        }
        else
        {
            Debug.LogError("Invalid typeIndex.");
        }
    }

    public void UpdateScriptableObjects()
    {
        if (theSOs != null && theSOs.Length > 0)
        {
            for (int i = 0; i < theSOs.Length; i++)
            {
                //Debug.Log(string.Format("{0}. {1}", i, theSOs[i]));
                if (theSOs[i] != null)
                {
                    theSOs[i].MenuSortingIndex = i;
                }
            }

            Debug.Log("These ScriptableObjects' menuSortingIndices have successfully been updated.");
        }
        else
        {
            Debug.LogError("No valid target(s) found.");
        }
    }
}
