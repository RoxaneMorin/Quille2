using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChecksAndMods;
using Quille;

namespace World
{
    // The ScriptableObject template of Interactions, use to create interaction data assets.
    // Subclasses, such as social interactions, may also derive from it.
    // This is the information shared between all interactions of a certain nature; for example, "sleep" as it is refered to by all beds.

    [CreateAssetMenu(fileName = "Interaction", menuName = "World/Interactions/Interaction", order = 0)]
    public class InteractionSO : ScriptableObject
    {
        // VARIABLES/PARAMS

        [SerializeField] protected string interactionName;

        // Targeted needs.
        [SerializeField] protected BasicNeedSO[] advertisedNeeds;
        [SerializeField] protected InteractionNeedEffectSettings[] effectedNeeds;
        // TODO: display a warning box in editor when either are empty.

        // Other tags, such as advertised drives.

        // Viability checks.
        [SerializeReference] [PopulateCheckSubtypes] protected Check[] viabilityChecks;

        // Scoring weights.
        [SerializeReference] [PopulateModulatorSubtypes] protected ModulatorArithmetic[] scoringModulators;

        // Fancy scoring weights for stuff involving morality, conflicting drives, etc?


        // PROPERTIES
        public string InteractionName { get { return interactionName; } }

        public BasicNeedSO[] AdvertisedNeeds { get { return advertisedNeeds; } }
        public InteractionNeedEffectSettings[] EffectedNeeds { get { return effectedNeeds; } }

        public Check[] ViabilityChecks { get { return viabilityChecks; } }
        public ModulatorArithmetic[] ScoringModulators { get { return scoringModulators; } }
    }
}