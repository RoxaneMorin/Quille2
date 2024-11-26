using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of Drives, used for the creation of specific drives as assets.
    // These drives represent a character's aspirations, desires and values, such as kinship or material wealth.


    [CreateAssetMenu(fileName = "Drive", menuName = "Quille/Character/Drive", order = 5)]
    public class DriveSO : ScriptableObject
    {
        // VARIABLES/PARAMS 
        [SerializeField]
        private string driveName = "Undefined";
        public string DriveName { get { return driveName; } }

        // Description.

        // DRIVE GRAPHICS
        public Sprite driveIcon;

        // OTHER VALUES
        // The intensity of drives is either 0.5 or 1.
        [SerializeField] [InspectorReadOnly] private float driveSpan = Constants.DRIVE_SPAN;
        public float DriveSpan { get { return driveSpan; } }

        [SerializeField] [InspectorReadOnly] private float driveMidpoint = Constants.DRIVE_SPAN / 2;
        public float DriveMidpoint { get { return driveSpan; } }

        // INCOMPATIBILITIES
        [SerializeField]
        private ChecksAndMods.CheckArithmetic[] incompatiblePersonalityScores;

        // FAVORABLE AND DEFAVORABLE PERSONALITY SCORES?

        // CATEGORIES?s
    }
}