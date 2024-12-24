using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quille.InterestSO)), CanEditMultipleObjects]
public class InterestSOEditor : Editor
{
    private SerializedProperty scriptProperty;

    private SerializedProperty itemName;
    private SerializedProperty itemIcon;
    private SerializedProperty menuSortingIndex;

    private SerializedProperty interestSpan;

    private SerializedProperty inDomains;
    private SerializedProperty relatedInterests;

    private void OnEnable()
    {
        scriptProperty = serializedObject.FindProperty("m_Script");

        itemName = serializedObject.FindProperty("itemName");
        itemIcon = serializedObject.FindProperty("itemIcon");
        menuSortingIndex = serializedObject.FindProperty("menuSortingIndex");

        interestSpan = serializedObject.FindProperty("interestSpan");

        inDomains = serializedObject.FindProperty("inDomains");
        relatedInterests = serializedObject.FindProperty("relatedInterests");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw script field & read only span.
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.PropertyField(scriptProperty);
            EditorGUILayout.PropertyField(interestSpan);
        }
        GUI.enabled = true;
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw generic properties.
        EditorGUILayout.PropertyField(itemName);
        EditorGUILayout.PropertyField(itemIcon);
        EditorGUILayout.PropertyField(menuSortingIndex);

        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw inDomains, and handle potential warning message.
        EditorGUILayout.PropertyField(inDomains);
        Quille.InterestSO interestSO = (Quille.InterestSO)target;
        if (interestSO.InDomains == null || interestSO.InDomains.Length == 0)
        {
            EditorGUILayout.HelpBox("An interest should belong to one or more domains.", MessageType.Error);
        }
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw relatedInterests.
        EditorGUILayout.PropertyField(relatedInterests);
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw clean up button.
        if (GUILayout.Button("Clean Up & Fix Dangling References"))
        {
            InterestsMenuUtilities.FixInterestReferences();
        }
        if (GUILayout.Button("Clean Up & Delete Dangling References"))
        {
            InterestsMenuUtilities.DeleteInterestReferences();
        }

        // Apply modified properties; modified linked objects as needed.
        if (serializedObject.ApplyModifiedProperties())
        {
            Quille.InterestSO targetInterestSO = serializedObject.targetObject as Quille.InterestSO;
            UpdateReferencedDomains(targetInterestSO);
            UpdateReferencedInterests(targetInterestSO);
        }
    }

    private void UpdateReferencedDomains(Quille.InterestSO interest)
    {
        foreach (Quille.InterestDomainSO domain in interest.InDomains)
        {
            if (domain && !domain.InterestInThisDomain.Contains(interest))
            {
                domain.AddToDomain(interest);
                EditorUtility.SetDirty(domain);
            }
        }
    }

    private void UpdateReferencedInterests(Quille.InterestSO interest)
    {
        foreach (Quille.InterestSO otherInterest in interest.RelatedInterests)
        {
            if (otherInterest && !otherInterest.RelatedInterests.Contains(interest))
            {
                otherInterest.AddRelatedInterest(interest);
                EditorUtility.SetDirty(otherInterest);
            }
        }
    }
}
