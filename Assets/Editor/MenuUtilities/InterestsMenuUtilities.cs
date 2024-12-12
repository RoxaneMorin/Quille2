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

    // Base methods.
    public static void RegisterDomainsInInterests(Quille.InterestDomainSO[] interestDomainSOs)
    {
        // Ensure every Interest contained in an InterestDomain also refers to it.
        foreach (Quille.InterestDomainSO interestDomain in interestDomainSOs)
        {
            //Debug.Log(interestDomain.DomainName);

            foreach (Quille.InterestSO interest in interestDomain.InterestInThisDomain)
            {
                //Debug.Log(interest.InterestName);

                if (!interest.InDomains.Contains(interestDomain))
                {
                    interest.InDomains.Add(interestDomain);
                    Debug.Log(string.Format("The Interest '{0}' now considers itself part of the '{1}' InterestDomain.", interest.InterestName, interestDomain.DomainName));
                }
            }
        }
    }

    public static void RegisterDomainsFromInterests(Quille.InterestSO[] interestSOs)
    {
        // Ensure every InterestDomain contains the Interests that refer to it.
        foreach (Quille.InterestSO interest in interestSOs)
        {
            //Debug.Log(interest.InterestName);

            foreach (Quille.InterestDomainSO interestDomain in interest.InDomains)
            {
                //Debug.Log(interestDomain.DomainName);

                if (!interestDomain.InterestInThisDomain.Contains(interest))
                {
                    interestDomain.InterestInThisDomain.Add(interest);
                    Debug.Log(string.Format("The InterestDomain '{0}' now considers the Interest '{1}' one of its members.", interestDomain.DomainName, interest.InterestName));
                }
            }
        }
    }

    public static void ClearInterestsNotInDomains(Quille.InterestDomainSO[] interestDomainSOs)
    {
        // Deletes all Interest references from InterestDomains they do not refer to.
        foreach (Quille.InterestDomainSO interestDomain in interestDomainSOs)
        {
            //Debug.Log(interestDomain.DomainName);

            List<Quille.InterestSO> reciprocatedInterestInThisDomain = new List<Quille.InterestSO>();

            foreach (Quille.InterestSO interest in interestDomain.InterestInThisDomain)
            {
                //Debug.Log(interest.InterestName);

                if (interest.InDomains.Contains(interestDomain))
                {
                    reciprocatedInterestInThisDomain.Add(interest);
                }
                else
                {
                    Debug.Log(string.Format("The Interest '{0}' was delete from the '{1}' InterestDomain's children.", interest.InterestName, interestDomain.DomainName));
                }
            }

            interestDomain.InterestInThisDomain = reciprocatedInterestInThisDomain;
        }
    }

    public static void ClearDomainsNotInInterests(Quille.InterestSO[] interestSOs)
    {
        // Deletes all InterestDomain references from Interests they do not contain.
        foreach (Quille.InterestSO interest in interestSOs)
        {
            //Debug.Log(interest.InterestName);

            List<Quille.InterestDomainSO> reciprocatedInDomains = new List<Quille.InterestDomainSO>();

            foreach (Quille.InterestDomainSO interestDomain in interest.InDomains)
            {
                //Debug.Log(interestDomain.DomainName);

                if (interestDomain.InterestInThisDomain.Contains(interest))
                {
                    reciprocatedInDomains.Add(interestDomain);
                }
                else
                {
                    Debug.Log(string.Format("The InterestDomain '{0}' was delete from the '{1}' Interest's parents.", interestDomain.DomainName, interest.InterestName));
                }
            }

            interest.InDomains = reciprocatedInDomains;
        }
    }

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
                    Debug.Log(string.Format("The Interest '{0}' now considers the Interest '{1}' to be related.", interest.InterestName, relatedInterest.InterestName));
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
                    Debug.Log(string.Format("The Interest '{0}' no longer considers the Interest '{1}' to be related.", interest.InterestName, relatedInterest.InterestName));
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
                Debug.Log(string.Format("A self-refering RelatedInterest was removed from '{0}.'", interest.InterestName));
            }

            // Remove duplicates in RelatedInterests and InDomains.
            interest.RelatedInterests = interest.RelatedInterests.Distinct().ToList();
            interest.InDomains = interest.InDomains.Distinct().ToList();
        }
    }

    public static void DeleteBadInterestDomainReferences(Quille.InterestDomainSO[] interestDomainSOs)
    {
        foreach (Quille.InterestDomainSO interestDomain in interestDomainSOs)
        {
            // Remove duplicates in InterestInThisDomain.
            interestDomain.InterestInThisDomain = interestDomain.InterestInThisDomain.Distinct().ToList();
        }
    }


     // Menu methods.
    [MenuItem("Quille/Person/Interests/Fix InterestDomains' one-sided references to child Interests.")]
    static void RegisterDomainsInInterests()
    {
        Quille.InterestDomainSO[] interestDomainSOs = Resources.LoadAll<Quille.InterestDomainSO>(Constants_PathResources.SO_PATH_INTERESTDOMAINS);
        RegisterDomainsInInterests(interestDomainSOs);
    }

    [MenuItem("Quille/Person/Interests/Fix Interests' one-sided references to parent InterestDomains.")]
    static void RegisterDomainsFromInterests()
    {
        Quille.InterestSO[] interestSOs = Resources.LoadAll<Quille.InterestSO>(Constants_PathResources.SO_PATH_INTERESTS);
        RegisterDomainsFromInterests(interestSOs);
    }

    [MenuItem("Quille/Person/Interests/Delete InterestDomains' one-sided references to child Interests.")]
    static void ClearInterestsNotInDomains()
    {
        Quille.InterestDomainSO[] interestDomainSOs = Resources.LoadAll<Quille.InterestDomainSO>(Constants_PathResources.SO_PATH_INTERESTDOMAINS);
        ClearInterestsNotInDomains(interestDomainSOs);
    }

    [MenuItem("Quille/Person/Interests/Delete Interests' one-sided references to parent InterestDomains.")]
    static void ClearDomainsNotInInterests()
    {
        Quille.InterestSO[] interestSOs = Resources.LoadAll<Quille.InterestSO>(Constants_PathResources.SO_PATH_INTERESTS);
        ClearDomainsNotInInterests(interestSOs);
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
        DeleteBadInterestDomainReferences(interestDomainSOs);
    }
}
