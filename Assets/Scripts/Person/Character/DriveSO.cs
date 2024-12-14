using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of Drives, used for the creation of specific drives as assets.
    // These drives represent a character's aspirations, desires and values, such as kinship or material wealth.


    [CreateAssetMenu(fileName = "Drive", menuName = "Quille/Character/Drive", order = 5)]
    public class DriveSO : ForbiddablePersonalityItemSO
    {
        // VARIABLES/PARAMS 
        // The intensity of drives is either 0.5 or 1.
        [SerializeField] [InspectorReadOnly] private float driveSpan = Constants_Quille.DRIVE_SPAN;
        [SerializeField] [InspectorReadOnly] private float driveMidpoint = Constants_Quille.DRIVE_SPAN / 2;

        // Favorable and defavorable personaltiy scores?
        // Categories?


        // PROPERTIES
        public float DriveSpan { get { return driveSpan; } }
        public float DriveMidpoint { get { return driveSpan; } }



        // METHODS
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