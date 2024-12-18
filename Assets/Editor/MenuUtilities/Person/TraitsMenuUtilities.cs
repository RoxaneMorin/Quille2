using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TraitsMenuUtilities : MonoBehaviour
{
    // Editor utilities for the management of PersonalityTraits and PersonalityTraitDomains.
    // Some of this logic is also handled by the PersonalityTraitSOEditor.


    // METHODS
    public static void DeleteBadPersonalityTraitReferences(Quille.PersonalityTraitSO[] PersonalityTraitSOs)
    {
        foreach (Quille.PersonalityTraitSO PersonalityTrait in PersonalityTraitSOs)
        {
            // Copy lists and remove invalid values: self references, nulls and duplicates.
            List<Quille.PersonalityTraitDomainSO> validInDomains = PersonalityTrait.InTraitDomains.Where(domain => domain != null).Distinct().ToList();

            // Apply the new lists.
            PersonalityTrait.InTraitDomains = validInDomains.ToArray();

            Debug.Log(string.Format("The PersonalityTrait '{0}' was checked for invalid references. Any found where removed.", PersonalityTrait.ItemName));
        }
    }


    // UI Methods.
    static public void FixPersonalityTraitReferences()
    {
        Quille.PersonalityTraitSO[] PersonalityTraitSOs = Resources.LoadAll<Quille.PersonalityTraitSO>(Constants_PathResources.SO_PATH_PERSONALITYTRAITS);
        Quille.PersonalityTraitDomainSO[] PersonalityTraitDomainSOs = Resources.LoadAll<Quille.PersonalityTraitDomainSO>(Constants_PathResources.SO_PATH_PERSONALITYTRAITDOMAINS);

        DeleteBadPersonalityTraitReferences(PersonalityTraitSOs);
        PersonalityItemsMenuUtilities.DeleteBadItemDomainReferences(PersonalityTraitDomainSOs);

        PersonalityItemsMenuUtilities.RegisterDomainsInItems(PersonalityTraitDomainSOs);
        PersonalityItemsMenuUtilities.RegisterDomainsFromItems(PersonalityTraitSOs);
    }

    static public void DeletePersonalityTraitReferences()
    {
        Quille.PersonalityTraitSO[] PersonalityTraitSOs = Resources.LoadAll<Quille.PersonalityTraitSO>(Constants_PathResources.SO_PATH_PERSONALITYTRAITS);
        Quille.PersonalityTraitDomainSO[] PersonalityTraitDomainSOs = Resources.LoadAll<Quille.PersonalityTraitDomainSO>(Constants_PathResources.SO_PATH_PERSONALITYTRAITDOMAINS);

        DeleteBadPersonalityTraitReferences(PersonalityTraitSOs);
        PersonalityItemsMenuUtilities.DeleteBadItemDomainReferences(PersonalityTraitDomainSOs);

        PersonalityItemsMenuUtilities.ClearItemsNotInDomains(PersonalityTraitDomainSOs);
        PersonalityItemsMenuUtilities.ClearDomainsNotInItems(PersonalityTraitSOs);
    }


    // Menu methods.
    [MenuItem("Quille/Person/PersonalityTraits/Fix PersonalityTraitDomains' one-sided references to child PersonalityTraits.")]
    static void RegisterDomainsInPersonalityTraits()
    {
        Quille.PersonalityTraitDomainSO[] PersonalityTraitDomainSOs = Resources.LoadAll<Quille.PersonalityTraitDomainSO>(Constants_PathResources.SO_PATH_PERSONALITYTRAITDOMAINS);
        PersonalityItemsMenuUtilities.RegisterDomainsInItems(PersonalityTraitDomainSOs);
    }

    [MenuItem("Quille/Person/PersonalityTraits/Fix PersonalityTraits' one-sided references to parent PersonalityTraitDomains.")]
    static void RegisterDomainsFromPersonalityTraits()
    {
        Quille.PersonalityTraitSO[] PersonalityTraitSOs = Resources.LoadAll<Quille.PersonalityTraitSO>(Constants_PathResources.SO_PATH_PERSONALITYTRAITS);
        PersonalityItemsMenuUtilities.RegisterDomainsFromItems(PersonalityTraitSOs);
    }

    [MenuItem("Quille/Person/PersonalityTraits/Delete PersonalityTraitDomains' one-sided references to child PersonalityTraits.")]
    static void ClearPersonalityTraitsNotInDomains()
    {
        Quille.PersonalityTraitDomainSO[] PersonalityTraitDomainSOs = Resources.LoadAll<Quille.PersonalityTraitDomainSO>(Constants_PathResources.SO_PATH_PERSONALITYTRAITDOMAINS);
        PersonalityItemsMenuUtilities.ClearItemsNotInDomains(PersonalityTraitDomainSOs);
    }

    [MenuItem("Quille/Person/PersonalityTraits/Delete PersonalityTraits' one-sided references to parent PersonalityTraitDomains.")]
    static void ClearDomainsNotInPersonalityTraits()
    {
        Quille.PersonalityTraitSO[] PersonalityTraitSOs = Resources.LoadAll<Quille.PersonalityTraitSO>(Constants_PathResources.SO_PATH_PERSONALITYTRAITS);
        PersonalityItemsMenuUtilities.ClearDomainsNotInItems(PersonalityTraitSOs);
    }


    [MenuItem("Quille/Person/PersonalityTraits/Clear other bad references in PersonalityTraits and PersonalityTraitsDomains.")]
    static void DeleteBadReferences()
    {
        Quille.PersonalityTraitSO[] PersonalityTraitSOs = Resources.LoadAll<Quille.PersonalityTraitSO>(Constants_PathResources.SO_PATH_PERSONALITYTRAITS);
        Quille.PersonalityTraitDomainSO[] PersonalityTraitDomainSOs = Resources.LoadAll<Quille.PersonalityTraitDomainSO>(Constants_PathResources.SO_PATH_PERSONALITYTRAITDOMAINS);

        DeleteBadPersonalityTraitReferences(PersonalityTraitSOs);
        PersonalityItemsMenuUtilities.DeleteBadItemDomainReferences(PersonalityTraitDomainSOs);
    }
}
