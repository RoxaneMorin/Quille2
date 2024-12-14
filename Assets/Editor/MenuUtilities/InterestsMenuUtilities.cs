using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class InterestsMenuUtilities : MonoBehaviour
{
    // Editor utilities for the management of Interests and InterestDomains.
    // Some of this logic is also handled by the InterestSOEditor.


    // METHODS
    public static void RegisterRelatedInterests(Quille.InterestSO[] interestSOs)
    {
        // Ensure that every reference in an Interest' RelatedInterests is reciprocal.
        foreach (Quille.InterestSO interest in interestSOs)
        {
            //Debug.Log(interest.InterestName);

            foreach (Quille.InterestSO relatedInterest in interest.RelatedInterests)
            {
                //Debug.Log(relatedInterest.InterestName);

                if (!relatedInterest.RelatedInterests.Contains(interest))
                {
                    relatedInterest.RelatedInterests.Add(interest);
                    Debug.Log(string.Format("The Interest '{0}' now considers the Interest '{1}' to be related.", interest.ItemName, relatedInterest.ItemName));
                }
            }
        }
    }

    public static void DeleteRelatedInterests(Quille.InterestSO[] interestSOs)
    {
        // Delete every non-reciprocal reference in an Interest' RelatedInterests.
        foreach (Quille.InterestSO interest in interestSOs)
        {
            //Debug.Log(interest.InterestName);

            List<Quille.InterestSO> reciprocatedRelatedInterest = new List<Quille.InterestSO>();

            foreach (Quille.InterestSO relatedInterest in interest.RelatedInterests)
            {
                //Debug.Log(relatedInterest.InterestName);

                if (relatedInterest.RelatedInterests.Contains(interest))
                {
                    reciprocatedRelatedInterest.Add(relatedInterest);
                }
                else
                {
                    Debug.Log(string.Format("The Interest '{0}' no longer considers the Interest '{1}' to be related.", interest.ItemName, relatedInterest.ItemName));
                }
            }

            interest.RelatedInterests = reciprocatedRelatedInterest;
        }
    }

    public static void DeleteBadInterestReferences(Quille.InterestSO[] interestSOs)
    {
        foreach (Quille.InterestSO interest in interestSOs)
        {
            // Remove self-references in RelatedInterests.
            if (interest.RelatedInterests.Contains(interest))
            {
                interest.RelatedInterests.Remove(interest);
                Debug.Log(string.Format("A self-refering RelatedInterest was removed from '{0}.'", interest.ItemName));
            }

            // Remove duplicates in RelatedInterests and InDomains.
            interest.RelatedInterests = interest.RelatedInterests.Distinct().ToList();
            interest.InDomains = interest.InDomains.Distinct().ToList();
        }
    }




     // Menu methods.
    [MenuItem("Quille/Person/Interests/Fix InterestDomains' one-sided references to child Interests.")]
    static void RegisterDomainsInInterests()
    {
        Quille.InterestDomainSO[] interestDomainSOs = Resources.LoadAll<Quille.InterestDomainSO>(Constants_PathResources.SO_PATH_INTERESTDOMAINS);
        PersonalityItemsMenuUtilities.RegisterDomainsInItems(interestDomainSOs);
    }

    [MenuItem("Quille/Person/Interests/Fix Interests' one-sided references to parent InterestDomains.")]
    static void RegisterDomainsFromInterests()
    {
        Quille.InterestSO[] interestSOs = Resources.LoadAll<Quille.InterestSO>(Constants_PathResources.SO_PATH_INTERESTS);
        PersonalityItemsMenuUtilities.RegisterDomainsFromItems(interestSOs);
    }

    [MenuItem("Quille/Person/Interests/Delete InterestDomains' one-sided references to child Interests.")]
    static void ClearInterestsNotInDomains()
    {
        Quille.InterestDomainSO[] interestDomainSOs = Resources.LoadAll<Quille.InterestDomainSO>(Constants_PathResources.SO_PATH_INTERESTDOMAINS);
        PersonalityItemsMenuUtilities.ClearItemsNotInDomains(interestDomainSOs);
    }

    [MenuItem("Quille/Person/Interests/Delete Interests' one-sided references to parent InterestDomains.")]
    static void ClearDomainsNotInInterests()
    {
        Quille.InterestSO[] interestSOs = Resources.LoadAll<Quille.InterestSO>(Constants_PathResources.SO_PATH_INTERESTS);
        PersonalityItemsMenuUtilities.ClearDomainsNotInItems(interestSOs);
    }

    [MenuItem("Quille/Person/Interests/Fix Interests' one-sided references to RelatedInterests.")]
    static void RegisterRelatedInterests()
    {
        Quille.InterestSO[] interestSOs = Resources.LoadAll<Quille.InterestSO>(Constants_PathResources.SO_PATH_INTERESTS);
        RegisterRelatedInterests(interestSOs);
    }

    [MenuItem("Quille/Person/Interests/Delete Interests' one-sided references to RelatedInterests.")]
    static void DeleteRelatedInterests()
    {
        Quille.InterestSO[] interestSOs = Resources.LoadAll<Quille.InterestSO>(Constants_PathResources.SO_PATH_INTERESTS);
        DeleteRelatedInterests(interestSOs);
    }

    [MenuItem("Quille/Person/Interests/Clear other bad references in Interests and InterestsDomains.")]
    static void DeleteBadReferences()
    {
        Quille.InterestSO[] interestSOs = Resources.LoadAll<Quille.InterestSO>(Constants_PathResources.SO_PATH_INTERESTS);
        Quille.InterestDomainSO[] interestDomainSOs = Resources.LoadAll<Quille.InterestDomainSO>(Constants_PathResources.SO_PATH_INTERESTDOMAINS);

        DeleteBadInterestReferences(interestSOs);
        PersonalityItemsMenuUtilities.DeleteBadItemDomainReferences(interestDomainSOs);
    }
}
