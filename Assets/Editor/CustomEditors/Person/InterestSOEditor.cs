using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quille.InterestSO))]
public class InterestSOEditor : Editor
{
    private List<Quille.InterestSO> previousRelatedInterests;
    
    private SerializedProperty scriptProperty;

    private SerializedProperty interestName;
    private SerializedProperty interestIcon;
    private SerializedProperty interestSpan;

    private SerializedProperty inDomains;
    private SerializedProperty relatedInterests;

    private void OnEnable()
    {
        scriptProperty = serializedObject.FindProperty("m_Script");

        interestName = serializedObject.FindProperty("interestName");
        interestIcon = serializedObject.FindProperty("interestIcon");
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
        EditorGUILayout.PropertyField(interestName);
        EditorGUILayout.PropertyField(interestIcon);
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
        if (GUILayout.Button("Clean Up References"))
        {
            CleanUpInterestReferences();
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

    private void CleanUpInterestReferences()
    {
        // Clean up InterestSOs.
        Quille.InterestSO[] interests = Resources.LoadAll<Quille.InterestSO>("ScriptableObjects/Interests/Fields");
        foreach (Quille.InterestSO interest in interests)
        {
            // Remove self references.
            if (interest.RelatedInterests.Contains(interest))
                interest.RelatedInterests.Remove(interest);

            // Remove duplicates.
            interest.RelatedInterests = interest.RelatedInterests.Distinct().ToList();
            interest.InDomains = interest.InDomains.Distinct().ToList();

            // Remove one-sided InterestSO references.
            List<Quille.InterestSO> referencesToRemove = new List<Quille.InterestSO>();
            foreach (Quille.InterestSO otherInterest in interest.RelatedInterests)
            {
                if (!otherInterest.RelatedInterests.Contains(interest))
                    referencesToRemove.Add(otherInterest);
            }
            foreach (Quille.InterestSO toRemove in referencesToRemove)
            {
                interest.RelatedInterests.Remove(toRemove);
            }

            // Remvoe one-sided InterestDomainSO references.
            List<Quille.InterestDomainSO> domainsToRemove = new List<Quille.InterestDomainSO>();
            foreach (Quille.InterestDomainSO domain in interest.InDomains)
            {
                if (!domain.InterestInThisDomain.Contains(interest))
                    domainsToRemove.Add(domain);
            }
            foreach (Quille.InterestDomainSO toRemove in domainsToRemove)
            {
                interest.InDomains.Remove(toRemove);
            }

            EditorUtility.SetDirty(interest);
        }

        // Clean up InterestDomainSOs.
        Quille.InterestDomainSO[] domains = Resources.LoadAll<Quille.InterestDomainSO>("ScriptableObjects/Interests/Domains");
        foreach (Quille.InterestDomainSO domain in domains)
        {
            // Remove duplicates.
            domain.InterestInThisDomain = domain.InterestInThisDomain.Distinct().ToList();

            // Remove one-sided InterestSO references.
            List<Quille.InterestSO> referencesToRemove = new List<Quille.InterestSO>();
            foreach (Quille.InterestSO interest in domain.InterestInThisDomain)
            {
                if (!interest.InDomains.Contains(domain))
                    referencesToRemove.Add(interest);
            }
            foreach (Quille.InterestSO toRemove in referencesToRemove)
            {
                domain.InterestInThisDomain.Remove(toRemove);
            }

            EditorUtility.SetDirty(domain);
        }
    }
}
