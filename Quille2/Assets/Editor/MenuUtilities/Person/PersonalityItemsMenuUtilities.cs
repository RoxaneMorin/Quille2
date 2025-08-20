using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PersonalityItemsMenuUtilities : MonoBehaviour
{
    // Editor utilities for the management of all PersonalityItemSOs and PersonalityItemDomainSOs.


    // METHODS
    public static void RegisterDomainsInItems(Quille.PersonalityItemDomainSO[] domainSOs)
    {
        // Ensure every Item contained in a Domain also refers to it.
        foreach (Quille.PersonalityItemDomainSO domain in domainSOs)
        {
            foreach (Quille.IUseDomains item in domain.ItemsInThisDomain)
            {
                //Debug.Log(item.ItemName);

                if (!item.InDomains.Contains(domain))
                {
                    item.AddDomain(domain);
                    Debug.Log(string.Format("The Item '{0}' now considers itself part of the '{1}' Domain.", item.ItemName, domain.DomainName));
                }
            }
        }
    }

    public static void RegisterDomainsFromItems(Quille.IUseDomains[] itemSOs)
    {
        // Ensure every Domain contains the Item that refer to it.
        foreach (Quille.IUseDomains item in itemSOs)
        {
            //Debug.Log(item.ItemName);
            Quille.PersonalityItemSO itemAsSO = item as Quille.PersonalityItemSO;

            foreach (Quille.PersonalityItemDomainSO domain in item.InDomains)
            {
                //Debug.Log(domain.DomainName);

                if (!domain.ItemsInThisDomain.Contains(itemAsSO))
                {
                    domain.AddToDomain(itemAsSO);
                    Debug.Log(string.Format("The Domain '{0}' now considers the Item '{1}' one of its members.", domain.DomainName, item.ItemName));
                }
            }
        }
    }

    public static void ClearItemsNotInDomains(Quille.PersonalityItemDomainSO[] domainSOs)
    {
        // Deletes all Item references from Domains they do not refer to.
        foreach (Quille.PersonalityItemDomainSO domain in domainSOs)
        {
            //Debug.Log(domain.DomainName);
            List<Quille.PersonalityItemSO> reciprocatedItemtInThisDomain = new List<Quille.PersonalityItemSO>();

            foreach (Quille.PersonalityItemSO item in domain.ItemsInThisDomain)
            {
                //Debug.Log(item.ItemName);

                if (((Quille.IUseDomains)item).InDomains.Contains(domain))
                {
                    reciprocatedItemtInThisDomain.Add(item);
                }
                else
                {
                    Debug.Log(string.Format("The Item '{0}' was deleted from the '{1}' Domain's children.", item.ItemName, domain.DomainName));
                }
            }

            domain.ItemsInThisDomain = reciprocatedItemtInThisDomain.ToArray();
        }
    }

    public static void ClearDomainsNotInItems(Quille.IUseDomains[] itemSOs)
    {
        // Deletes all Domain references from Items they do not contain.
        foreach (Quille.IUseDomains item in itemSOs)
        {
            //Debug.Log(item.ItemName);
            Quille.PersonalityItemSO itemAsSO = item as Quille.PersonalityItemSO;

            List<Quille.PersonalityItemDomainSO> reciprocatedInDomains = new List<Quille.PersonalityItemDomainSO>();

            foreach (Quille.PersonalityItemDomainSO domain in item.InDomains)
            {
                //Debug.Log(domain.DomainName);

                if (domain.ItemsInThisDomain.Contains(itemAsSO))
                {
                    reciprocatedInDomains.Add(domain);
                }
                else
                {
                    Debug.Log(string.Format("The Domain '{0}' was deleted from the '{1}' Item's parents.", domain.DomainName, item.ItemName));
                }
            }

            item.InDomains = reciprocatedInDomains.ToArray() ;
        }
    }

    public static void DeleteBadItemDomainReferences(Quille.PersonalityItemDomainSO[] domainSOs)
    {
        foreach (Quille.PersonalityItemDomainSO domain in domainSOs)
        {
            // Remove nulls and duplicates in ItemsInThisDomain.
            domain.ItemsInThisDomain = domain.ItemsInThisDomain.Where(item => item).Distinct().ToArray();

            Debug.Log(string.Format("The Domain '{0}' was checked for invalid references. Any found where removed.", domain.DomainName));
        }
    }
}
