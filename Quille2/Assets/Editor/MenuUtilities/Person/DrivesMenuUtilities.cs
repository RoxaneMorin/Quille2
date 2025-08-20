using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DrivesMenuUtilities : MonoBehaviour
{
    // Editor utilities for the management of Drives and DriveDomains.
    // Some of this logic is also handled by the DriveSOEditor.


    // METHODS
    public static void DeleteBadDriveReferences(Quille.DriveSO[] DriveSOs)
    {
        foreach (Quille.DriveSO Drive in DriveSOs)
        {
            // Copy lists and remove invalid values: self references, nulls and duplicates.
            List<Quille.DriveDomainSO> validInDomains = Drive.InDriveDomains.Where(domain => domain != null).Distinct().ToList();

            // Apply the new lists.
            Drive.InDriveDomains = validInDomains.ToArray();

            Debug.Log(string.Format("The Drive '{0}' was checked for invalid references. Any found where removed.", Drive.ItemName));
        }
    }


    // UI Methods.
    static public void FixDriveReferences()
    {
        Quille.DriveSO[] DriveSOs = Resources.LoadAll<Quille.DriveSO>(Constants_PathResources.SO_PATH_DRIVES);
        Quille.DriveDomainSO[] DriveDomainSOs = Resources.LoadAll<Quille.DriveDomainSO>(Constants_PathResources.SO_PATH_DRIVEDOMAINS);

        DeleteBadDriveReferences(DriveSOs);
        PersonalityItemsMenuUtilities.DeleteBadItemDomainReferences(DriveDomainSOs);

        PersonalityItemsMenuUtilities.RegisterDomainsInItems(DriveDomainSOs);
        PersonalityItemsMenuUtilities.RegisterDomainsFromItems(DriveSOs);
    }

    static public void DeleteDriveReferences()
    {
        Quille.DriveSO[] DriveSOs = Resources.LoadAll<Quille.DriveSO>(Constants_PathResources.SO_PATH_DRIVES);
        Quille.DriveDomainSO[] DriveDomainSOs = Resources.LoadAll<Quille.DriveDomainSO>(Constants_PathResources.SO_PATH_DRIVEDOMAINS);

        DeleteBadDriveReferences(DriveSOs);
        PersonalityItemsMenuUtilities.DeleteBadItemDomainReferences(DriveDomainSOs);

        PersonalityItemsMenuUtilities.ClearItemsNotInDomains(DriveDomainSOs);
        PersonalityItemsMenuUtilities.ClearDomainsNotInItems(DriveSOs);
    }


    // Menu methods.
    [MenuItem("Quille/Person/Drives/Fix DriveDomains' one-sided references to child Drives.")]
    static void RegisterDomainsInDrives()
    {
        Quille.DriveDomainSO[] DriveDomainSOs = Resources.LoadAll<Quille.DriveDomainSO>(Constants_PathResources.SO_PATH_DRIVEDOMAINS);
        PersonalityItemsMenuUtilities.RegisterDomainsInItems(DriveDomainSOs);
    }

    [MenuItem("Quille/Person/Drives/Fix Drives' one-sided references to parent DriveDomains.")]
    static void RegisterDomainsFromDrives()
    {
        Quille.DriveSO[] DriveSOs = Resources.LoadAll<Quille.DriveSO>(Constants_PathResources.SO_PATH_DRIVES);
        PersonalityItemsMenuUtilities.RegisterDomainsFromItems(DriveSOs);
    }

    [MenuItem("Quille/Person/Drives/Delete DriveDomains' one-sided references to child Drives.")]
    static void ClearDrivesNotInDomains()
    {
        Quille.DriveDomainSO[] DriveDomainSOs = Resources.LoadAll<Quille.DriveDomainSO>(Constants_PathResources.SO_PATH_DRIVEDOMAINS);
        PersonalityItemsMenuUtilities.ClearItemsNotInDomains(DriveDomainSOs);
    }

    [MenuItem("Quille/Person/Drives/Delete Drives' one-sided references to parent DriveDomains.")]
    static void ClearDomainsNotInDrives()
    {
        Quille.DriveSO[] DriveSOs = Resources.LoadAll<Quille.DriveSO>(Constants_PathResources.SO_PATH_DRIVES);
        PersonalityItemsMenuUtilities.ClearDomainsNotInItems(DriveSOs);
    }


    [MenuItem("Quille/Person/Drives/Clear other bad references in Drives and DrivesDomains.")]
    static void DeleteBadReferences()
    {
        Quille.DriveSO[] DriveSOs = Resources.LoadAll<Quille.DriveSO>(Constants_PathResources.SO_PATH_DRIVES);
        Quille.DriveDomainSO[] DriveDomainSOs = Resources.LoadAll<Quille.DriveDomainSO>(Constants_PathResources.SO_PATH_DRIVEDOMAINS);

        DeleteBadDriveReferences(DriveSOs);
        PersonalityItemsMenuUtilities.DeleteBadItemDomainReferences(DriveDomainSOs);
    }
}
