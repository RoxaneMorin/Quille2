using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of Drives, used for the creation of specific drives as assets.
    // These drives represent a character's aspirations, desires and values, such as kinship or material wealth.


    [CreateAssetMenu(fileName = "Drive", menuName = "Quille/Character/Drives/Drive", order = 1)]
    public class DriveSO : ForbiddablePersonalityItemSO, IUseDomains
    {
        // VARIABLES/PARAMS 
        // The intensity of drives is either 0.5 or 1.
        [SerializeField] [InspectorReadOnly] private float driveSpan = Constants_Quille.DRIVE_SPAN;
        [SerializeField] [InspectorReadOnly] private float driveMidpoint = Constants_Quille.DRIVE_SPAN / 2;

        [SerializeField] private DriveDomainSO[] inDomains;

        // Favorable and defavorable personaltiy scores?


        // PROPERTIES
        public float DriveSpan { get { return driveSpan; } }
        public float DriveMidpoint { get { return driveSpan; } }

        public DriveDomainSO[] InDriveDomains { get { return inDomains; ; } set { inDomains = value; } }
        public PersonalityItemDomainSO[] InDomains { get { return inDomains.Cast<PersonalityItemDomainSO>().ToArray(); ; } set { inDomains = value.Cast<DriveDomainSO>().ToArray(); } }



        // METHODS
        public bool InDomain(PersonalityItemDomainSO domain)
        {
            return inDomains.Contains(domain);
        }
        public void AddDomain(PersonalityItemDomainSO newDomain)
        {
            inDomains.Append((DriveDomainSO)newDomain);
        }

        public override bool IsCompatibleWithPerson(Person targetPerson)
        {
            // Check whether this SO is incompatible with any relevant aspect of the given person.
            foreach (ChecksAndMods.CheckArithmetic check in incompatiblePersonChecks)
            {
                if (check.Execute(targetPerson))
                {
                    Debug.Log(string.Format("The Drive '{1}' is forbidden to {0}, due to the Check '{2}'.", targetPerson.CharIDAndCharacterName, this.itemName, check.ToString()));
                    return false;
                }
            }
            return true;
        }
    }
}