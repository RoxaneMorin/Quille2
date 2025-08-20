using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of BasicNeeds, used for the creation of specific needs as assets.
    // These needs represent basic physiological and psychological drives such as hunger and stress.


    [CreateAssetMenu(fileName = "BasicNeed", menuName = "Quille/Needs/Basic Need", order = 0)]
    public class BasicNeedSO : ScriptableObject
    {
        // VARIABLES/PARAMS   
        [SerializeField] private string needName = "Undefined";
        public string NeedName { get { return needName; } }

        [SerializeField] private bool isHalfNeed = false;
        public bool IsHalfNeed { get { return isHalfNeed; } }


        // NEED GRAPHICS
        [SerializeField] private Sprite needIcon;
        public Sprite NeedIcon { get { return needIcon; } }


        // NEED'S DEFAULT VALUES
        [SerializeField, Range(Constants_Quille.MINIMUM_NEED_PRIORITY, Constants_Quille.MAXIMUM_NEED_PRIORITY)] // Priorize larger values? 
        private float aiPriorityWeighting = 1; // Should this be static, since it'll likely by the same for all characters?
        public float AiPriorityWeighting { get { return aiPriorityWeighting; } }

        [SerializeField] private float defaultChangeRate = 0; // the need's universal default decay rate.
        public float DefaultChangeRate { get { return defaultChangeRate; } }

        [SerializeField] private float thresholdElated = Constants_Quille.DEFAULT_NEED_THRESHOLD_ELATED;
        public float ThresholdElated { get { return thresholdElated; } }
        [SerializeField] private float thresholdWarning = Constants_Quille.DEFAULT_NEED_THRESHOLD_WARNING;
        public float ThresholdWarning { get { return thresholdWarning; } }
        [SerializeField] private float thresholdCritical = Constants_Quille.DEFAULT_NEED_THRESHOLD_CRITICAL;
        public float ThresholdCritical { get { return thresholdCritical; } }


        // MODULATORS
        //Default values modulated by ? (List of functions/references)
        [SerializeReference][PopulateModulatorSubtypes] private ChecksAndMods.Modulator[] baseAIWeightingModulatedBy;
        // TODO: should AI weighting be done on the basis of general need type instead? Should it be "sacrified" in favor of the generic notice levels?
        public ChecksAndMods.Modulator[] BaseAIWeightingModulatedBy  { get { return baseAIWeightingModulatedBy; } }

        [SerializeReference][PopulateModulatorSubtypes] private ChecksAndMods.Modulator[] baseChangeRateModulatedBy;
        public ChecksAndMods.Modulator[] BaseChangeRateModulatedBy { get { return baseChangeRateModulatedBy; } }

        [SerializeReference][PopulateModulatorSubtypes] private ChecksAndMods.Modulator[] thresholdsModulatedBy;
        public ChecksAndMods.Modulator[] ThresholdsModulatedBy { get { return thresholdsModulatedBy; } }
        // TODO: how are individual thresholds modulated?
    }
}