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
        // The intensity of traits is either 0.5 or 1.
        [BeginInspectorReadOnlyGroup]
        [SerializeField]
        private float traitSpan = Constants.PERSONALITY_HALF_SPAN;
        public float TraitSpan { get { return traitSpan; } }

        [BeginInspectorReadOnlyGroup]
        [SerializeField]
        private float trainMidpoint = Constants.PERSONALITY_HALF_SPAN / 2;
        public float TrainMidpoint { get { return trainMidpoint; } }

        [EndInspectorReadOnlyGroup]
        
        // INCOMPATIBILITIES
        [SerializeField]
        private PersonalityTraitSO[] incompatibleTraits;

        // Array of checks for incompatible personality scores.
        [SerializeField]
        private ChecksAndMods.CheckArithmetic[] incompatiblePersonalityScores;
    }
}

