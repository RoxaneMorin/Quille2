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
        [SerializeField]
        private string needName = "Undefined";
        public string NeedName { get { return needName; } }

        // Description.

        // NEED GRAPHICS
        public Sprite needIcon;

        // NEED'S DEFAULT VALUES
        [SerializeField, Range(Constants.MIN_PRIORITY, Constants.MAX_PRIORITY)] // Priorize larger values? 
        private float aiPriorityWeighting = 1; // Should this be static, since it'll likely by the same for all characters?
        public float AiPriorityWeighting { get { return aiPriorityWeighting; } }

        [SerializeField] // not sure about the nomenclature for these. level, range, gauge, etc? 
        private float levelFull = Constants.DEFAULT_LEVEL_FULL;
        public float LevelFull { get { return levelFull; } }
        public float LevelEmpty { get { return Constants.DEFAULT_LEVEL_EMPTY; } }// Will always be 0?

        [SerializeField]
        private float defaultChangeRate = 0; // the need's universal default decay rate.
        public float DefaultChangeRate { get { return defaultChangeRate; } }

        [SerializeField]
        private float thresholdElated = Constants.DEFAULT_THRESHOLD_ELATED;
        public float ThresholdElated { get { return thresholdElated; } }
        [SerializeField]
        private float thresholdWarning = Constants.DEFAULT_THRESHOLD_WARNING;
        public float ThresholdWarning { get { return thresholdWarning; } }
        [SerializeField]
        private float thresholdCritical = Constants.DEFAULT_THRESHOLD_CRITICAL;
        public float ThresholdCritical { get { return thresholdCritical; } }


        // MODULATORS
        //Default values modulated by ? (List of functions/references)
        [SerializeField] private ChecksAndMods.ModulatorArithmeticFromFloat[] baseAIWeightingModulatedBy;
        public ChecksAndMods.ModulatorArithmeticFromFloat[] BaseAIWeightingModulatedBy  { get { return baseAIWeightingModulatedBy; } }

        [SerializeField] private ChecksAndMods.ModulatorArithmeticFromFloat[] baseChangeRateModulatedBy;
        public ChecksAndMods.ModulatorArithmeticFromFloat[] BaseChangeRateModulatedBy { get { return baseChangeRateModulatedBy; } }

        [SerializeField] private ChecksAndMods.ModulatorArithmeticFromFloat[] thresholdsModulatedBy;
        public ChecksAndMods.ModulatorArithmeticFromFloat[] ThresholdsModulatedBy { get { return thresholdsModulatedBy; } }
        // TODO: how are individual thresholds modulated?
    }
}