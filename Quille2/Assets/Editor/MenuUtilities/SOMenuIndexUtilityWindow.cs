using AmplifyShaderEditor;
using AYellowpaper.SerializedCollections.Editor;
using Quille;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class SOMenuIndexUtilityWindow : EditorWindow
{
    // VARIABLES
    private SerializedObject thisAsSerializedObject;
    private SerializedProperty sortableObjsProperty;
    [SerializeField] private UnityEngine.Object[] sortableObjs;

    private Type[] relevantTypes;
    private string[] typeNames;

    private int sortableTypeIndex = -1;
    private Vector2 scrollPosition = Vector2.zero;



    // METHODS
    [MenuItem("Quille/SO MenuIndex Update Utility")]
    public static void OpenWindow()
    {
        SOMenuIndexUtilityWindow window = GetWindow<SOMenuIndexUtilityWindow>();
        window.titleContent = new GUIContent("SO MenuIndex Update Utility");
    }

    public void OnEnable()
    {
        thisAsSerializedObject = new SerializedObject(this);
        sortableObjsProperty = thisAsSerializedObject.FindProperty("sortableObjs");

        relevantTypes = EditorUtilities.FindValidSubtypes(typeof(IMenuSortable));
        typeNames = EditorUtilities.GetTypeNames(relevantTypes);
    }

    public void OnGUI()
    {
        float padding = 8f ;
        float doublePadding = padding * 2f;
        GUILayout.BeginArea(new Rect(padding, padding, position.width - doublePadding, position.height - doublePadding));

        GUILayout.Label("Target Object Type", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        sortableTypeIndex = EditorGUILayout.Popup(sortableTypeIndex, typeNames);
        if (EditorGUI.EndChangeCheck() && sortableTypeIndex > -1 && sortableTypeIndex < relevantTypes.Length)
        {
            LoadSortables(sortableTypeIndex);
            sortableObjsProperty.isExpanded = true;
        }

        GUILayout.Space(padding);    
        GUI.enabled = (sortableObjs != null && sortableObjs.Length > 0);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(sortableObjsProperty, new GUIContent("Relevant ScriptableObjects"));
        if (EditorGUI.EndChangeCheck())
        {
            thisAsSerializedObject.ApplyModifiedProperties();
        }

        GUILayout.EndScrollView();
        GUILayout.Space(padding);

        if (GUILayout.Button("Apply this Order"))
        {
            UpdateSortables();
        }

        GUILayout.EndArea();
    }

    private void OnDestroy()
    {
        thisAsSerializedObject = null;
        sortableObjsProperty = null;
        sortableObjs = null;
    }


    // Internal
    private void LoadSortables(int typeIndex)
    {
        if (typeIndex > -1)
        {

            sortableObjs = Resources.LoadAll(string.Empty, relevantTypes[typeIndex]).OrderBy(so => (so as IMenuSortable)?.SortingIndex).ToArray();
            thisAsSerializedObject.Update();
        }
        else
        {
            Debug.LogError("Invalid typeIndex.");
        }
    }

    private void UpdateSortables()
    {
        if (sortableObjs != null && sortableObjs.Length > 0)
        {
            int trueCount = 0; 
            for (int i = 0; i < sortableObjs.Length; i++)
            {
                if (sortableObjs[i] != null)
                {
                    //Debug.Log(string.Format("{0}. {1}", i, sortableObjs[i]));
                    (sortableObjs[i] as IMenuSortable).SortingIndex = i;
                    trueCount += 1;
                }
            }
            Debug.Log($"{trueCount} objects of type '{typeNames[sortableTypeIndex]}' have been updated.");
        }
        else
        {
            Debug.LogError("No valid object(s) found.");
        }
    }
}
