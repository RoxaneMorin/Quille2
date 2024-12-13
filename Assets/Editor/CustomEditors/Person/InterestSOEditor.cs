using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quille.InterestSO))]
public class InterestSOEditor : Editor
{
    private SerializedProperty scriptProperty;

    private SerializedProperty itemName;
    private SerializedProperty itemIcon;
    private SerializedProperty interestSpan;

    private SerializedProperty inDomains;
    private SerializedProperty relatedInterests;

    private void OnEnable()
    {
        scriptProperty = serializedObject.FindProperty("m_Script");

        itemName = serializedObject.FindProperty("itemName");
        itemIcon = serializedObject.FindProperty("itemIcon");
        interestSpan = serializedObject.FindProperty("interestSpan");

        inDomains = serializedObject.FindProperty("inDomains");
        relatedInterests = serializedObject.FindProperty("relatedInterests");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw script field.
        using (new EditorGUI.DisabledScope(true))
            EditorGUILayout.PropertyField(scriptProperty);

        // Draw generic properties.
        EditorGUILayout.PropertyField(itemName);
        EditorGUILayout.PropertyField(itemIcon);
        EditorGUILayout.PropertyField(interestSpan);

        // Draw inDomains, and handle potential warning message.
        EditorGUILayout.PropertyField(inDomains);
        Quille.InterestSO interestSO = (Quille.InterestSO)target;
        if (interestSO.InDomains == null || interestSO.InDomains.Count == 0)
        {
            EditorGUILayout.HelpBox("An interest should belong to one or more domains.", MessageType.Error);
        }

        // Draw relatedInterests.
        EditorGUILayout.PropertyField(relatedInterests);

        // Draw clean up button.
        if (GUILayout.Button("Clean Up & Fix Dangling References"))
        {
            FixInterestReferences();
        }
        if (GUILayout.Button("Clean Up & Delete Dangling References"))
        {
            DeleteInterestReferences();
        }

        // Apply modified properties; modified linked objects as needed.
        if (serializedObject.ApplyModifiedProperties())
        {
            Quille.InterestSO targetInterestSO = serializedObject.targetObject as Quille.InterestSO;
            UpdateReferencedDomains(targetInterestSO);
            UpdateReferencedInterests(targetInterestSO);
        }
    }

    private void UpdateReferencedDomains(Quille.InterestSO interestSO)
    {
        foreach (Quille.InterestDomainSO domain in interestSO.InDomains)
        {
            if (!domain.InterestInThisDomain.Contains(interestSO))
            {
                domain.InterestInThisDomain.Add(interestSO);
                EditorUtility.SetDirty(domain);
            }
        }
    }

    private void UpdateReferencedInterests(Quille.InterestSO interestSO)
    {
        foreach (Quille.InterestSO otherInterest in interestSO.RelatedInterests)
        {
            if (!otherInterest.RelatedInterests.Contains(interestSO))
            {
                otherInterest.RelatedInterests.Add(interestSO);
                EditorUtility.SetDirty(otherInterest);
            }
        }
    }

    private void FixInterestReferences()
    {
        Quille.InterestSO[] interestSOs = Resources.LoadAll<Quille.InterestSO>(Constants_PathResources.SO_PATH_INTERESTS);
        Quille.InterestDomainSO[] interestDomainSOs = Resources.LoadAll<Quille.InterestDomainSO>(Constants_PathResources.SO_PATH_INTERESTDOMAINS);

        InterestsMenuUtilities.DeleteBadInterestReferences(interestSOs);
        InterestsMenuUtilities.DeleteBadInterestDomainReferences(interestDomainSOs);

        InterestsMenuUtilities.RegisterDomainsInInterests(interestDomainSOs);
        InterestsMenuUtilities.RegisterDomainsFromInterests(interestSOs);
        InterestsMenuUtilities.RegisterRelatedInterests(interestSOs);
    }

    private void DeleteInterestReferences()
    {
        Quille.InterestSO[] interestSOs = Resources.LoadAll<Quille.InterestSO>(Constants_PathResources.SO_PATH_INTERESTS);
        Quille.InterestDomainSO[] interestDomainSOs = Resources.LoadAll<Quille.InterestDomainSO>(Constants_PathResources.SO_PATH_INTERESTDOMAINS);

        InterestsMenuUtilities.DeleteBadInterestReferences(interestSOs);
        InterestsMenuUtilities.DeleteBadInterestDomainReferences(interestDomainSOs);

        InterestsMenuUtilities.ClearInterestsNotInDomains(interestDomainSOs);
        InterestsMenuUtilities.ClearDomainsNotInInterests(interestSOs);
        InterestsMenuUtilities.DeleteRelatedInterests(interestSOs);
    }
}
