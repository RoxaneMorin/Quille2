using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille 
{
    [CreateAssetMenu(fileName = "PersonalityTrait", menuName = "Quille/Personality/Personality Trait", order = 1)]
    public class PersonalityTraitSO : ScriptableObject
    {
        // VARIABLES/PARAMS 
        [SerializeField]
        private string traitName = "Undefined";
        public string TraitName { get { return traitName; } }

        // Description.

        // TRAIT GRAPHICS
        public Sprite traitIcon;

        // OTHER VALUES
        [SerializeField, InspectorReadOnly]
        private float traitSpan = Constants.AXE_HALF_SPAN;
        public float TraitSpan { get { return traitSpan; } }

        [SerializeField, InspectorReadOnly]
        private float trainMidpoint = Constants.AXE_HALF_SPAN / 2;
        public float TrainMidpoint { get { return trainMidpoint; } }
        // The intensity of traits is either 0.5 or 1.

        // INCOMPATIBILITIES
        [SerializeField]
        private PersonalityAxeSO[] incompatibleTraits;

        // Array of checks for incompatible personality scores.
        [SerializeField]
        private ChecksAndMods.CheckArithmetic[] incompatiblePersonalityScores;
    }
}

