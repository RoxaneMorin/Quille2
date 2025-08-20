using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // Additional data container there to track further modulators for a given need.
    // Unsure of whether this will be necessary.
    // TODO: implement their loading into the main need objects.


    [CreateAssetMenu(fileName = "NeedModdulations", menuName = "Quille/Needs/Additional Need Modulations", order = 10)]
    public class AdditionalNeedModulations : ScriptableObject
    {
        // VARIABLES/PARAMS
        [SerializeField] private BasicNeedSO targetNeed;
        public BasicNeedSO BasicNeedSO { get { return targetNeed; } }


        // MODULATORS
        //Default values modulated by ? (List of functions/references)
        [SerializeReference][PopulateModulatorSubtypes] private ChecksAndMods.Modulator[] baseAIWeightingModulatedBy;
        // TODO: should AI weighting be done on the basis of general need type instead? Should it be "sacrified" in favor of the generic notice levels?
        public ChecksAndMods.Modulator[] BaseAIWeightingModulatedBy { get { return baseAIWeightingModulatedBy; } }

        [SerializeReference][PopulateModulatorSubtypes] private ChecksAndMods.Modulator[] baseChangeRateModulatedBy;
        public ChecksAndMods.Modulator[] BaseChangeRateModulatedBy { get { return baseChangeRateModulatedBy; } }

        [SerializeReference][PopulateModulatorSubtypes] private ChecksAndMods.Modulator[] thresholdsModulatedBy;
        public ChecksAndMods.Modulator[] ThresholdsModulatedBy { get { return thresholdsModulatedBy; } }
        // TODO: how are individual thresholds modulated?
    }
}

