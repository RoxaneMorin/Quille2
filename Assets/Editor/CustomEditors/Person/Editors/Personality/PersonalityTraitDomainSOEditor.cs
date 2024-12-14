using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quille.PersonalityTraitDomainSO))]
public class PersonalityTraitDomainSOEditor : Editor
{
    private SerializedProperty scriptProperty;

    private SerializedProperty domainName;
    private SerializedProperty domainIcon;
    private SerializedProperty domainColour;
    private SerializedProperty menuSortingIndex;

    private SerializedProperty itemsInThisDomain;

    private void OnEnable()
    {
        scriptProperty = serializedObject.FindProperty("m_Script");

        domainName = serializedObject.FindProperty("domainName");
        domainIcon = serializedObject.FindProperty("domainIcon");
        domainColour = serializedObject.FindProperty("domainColour");
        menuSortingIndex = serializedObject.FindProperty("menuSortingIndex");

        itemsInThisDomain = serializedObject.FindProperty("itemsInThisDomain");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw script field.
        using (new EditorGUI.DisabledScope(true))
            EditorGUILayout.PropertyField(scriptProperty);
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw generic properties.
        EditorGUILayout.PropertyField(domainName);
        EditorGUILayout.PropertyField(domainIcon);
        EditorGUILayout.PropertyField(domainColour);
        EditorGUILayout.PropertyField(menuSortingIndex);

        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw itemsInThisDomain.
        EditorGUILayout.PropertyField(itemsInThisDomain);
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw clean up buttons.
        if (GUILayout.Button("Clean Up & Fix Dangling References"))
        {
            TraitsMenuUtilities.FixPersonalityTraitReferences();
        }
        if (GUILayout.Button("Clean Up & Delete Dangling References"))
        {
            TraitsMenuUtilities.DeletePersonalityTraitReferences();
        }

        // Apply modified properties; modified linked objects as needed.
        if (serializedObject.ApplyModifiedProperties())
        {
            Quille.PersonalityTraitDomainSO targetTraitDomainSO = serializedObject.targetObject as Quille.PersonalityTraitDomainSO;
            UpdateReferencedDrives(targetTraitDomainSO);
        }
    }

    private void UpdateReferencedDrives(Quille.PersonalityTraitDomainSO domain)
    {
        foreach (Quille.PersonalityTraitSO trait in domain.TraitsInThisDomain)
        {
            if (trait && !trait.InTraitDomains.Contains(domain))
            {
                trait.AddDomain(domain);
                EditorUtility.SetDirty(trait);
            }
        }
    }
}
