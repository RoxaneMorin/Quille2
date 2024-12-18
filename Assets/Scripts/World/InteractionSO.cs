using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChecksAndMods;
using Quille;

namespace World
{
    // The ScriptableObject template of Interactions, use to create specific ones as assets.
    // Subclasses, such as social interactions, may also derive from it.

    [CreateAssetMenu(fileName = "Interaction", menuName = "World/Interactions/Interaction", order = 0)]
    public class InteractionSO : ScriptableObject
    {
        // VARIABLES/PARAMS

        // Need(s) advertised to.
        [SerializeField] protected BasicNeedSO[] advertisedNeeds;
        // TODO: display a warning box in editor when the list is empty.

        // Other tags, such as advertised drives.

        // Viability checks.
        [SerializeField] protected CheckBoolean viabilityChecks;

        // Scoring weights.
        [SerializeField] protected ModulatorArithmeticFromFloat scoringModulators;

        // Fancy scoring weights for stuff involving morality, conflicting drives, etc?

        // Average/base need change effect.
        // TODO: should this be a dict of the needs affected?
        [SerializeField] protected float defaultNeedChangeRate;
        [SerializeField] protected float defaultMaxNeedChange;

        [SerializeField] protected ModulatorArithmeticFromFloat needChangeRateModulatedBy;
        [SerializeField] protected ModulatorArithmeticFromFloat maxNeedChangeModulatedBy;



        // METHODS

        // → Score for person
        // → Calculate situational need change rate
        // → Calculate situational max need change.
    }
}