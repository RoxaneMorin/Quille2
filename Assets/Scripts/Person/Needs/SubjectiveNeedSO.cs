using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of SubjectiveNeeds, used for the creation of specific needs as assets.
    // These needs represent higher level psychological drives such as the desires for social interactions, comfort or novelty in one's life.
    // Two (somewhat) opposing drives are contained within the same SubjectiveNeed.


    [CreateAssetMenu(fileName = "SubjectiveNeed", menuName = "Quille/Needs/Subjective Need", order = 1)]
    public class SubjectiveNeedSO : ScriptableObject
    {
        // VARIABLES/PARAMS
        [SerializeField]
        private string needName = "Undefined";
        public string NeedName { get { return needName; } }

        [SerializeField]
        private string needNameLeft = "Undefined (Left)";
        public string NeedNameLeft { get { return needNameLeft; } }

        [SerializeField]
        private string needNameRight = "Undefined (Right)";
        public string NeedNameRight { get { return needNameRight; } }

        // Description.

        // NEED GRAPHICS
        public Sprite needIconLeft;
        public Sprite needIconRight;

        // NEED'S DEFAULT VALUES
        [SerializeField, Range(Constants_Quille.MIN_PRIORITY, Constants_Quille.MAX_PRIORITY)] // Priorize larger values? 
        private float aiPriorityWeighting = 1; // Should this be static, since it'll likely by the same for all characters?
        public float AiPriorityWeighting { get { return aiPriorityWeighting; } }
        // Are separate priorities needed for the two sides? Only bother on a per character basis?

        [SerializeField] // not sure about the nomenclature for these. level, range, gauge, etc? 
        private float levelFullLeft = Constants_Quille.DEFAULT_LEVEL_FULL;
        public float LevelFullLeft { get { return levelFullLeft; } }
        public float LevelEmptyLeft { get { return Constants_Quille.DEFAULT_LEVEL_EMPTY; } }// Will always be 0?
        [SerializeField]
        private float levelFullRight = Constants_Quille.DEFAULT_LEVEL_FULL;
        public float LevelFullRight { get { return levelFullRight; } }
        public float LevelEmptyRight { get { return Constants_Quille.DEFAULT_LEVEL_EMPTY; } }

        [SerializeField]
        private float defaultChangeRateLeft = 0;
        public float DefaultChangeRateLeft { get { return defaultChangeRateLeft; } }
        [SerializeField]
        private float defaultChangeRateRight = 0;
        public float DefaultChangeRateRight { get { return defaultChangeRateRight; } }

        [SerializeField]
        private float thresholdElatedLeft = Constants_Quille.DEFAULT_THRESHOLD_ELATED;
        public float ThresholdElatedLeft { get { return thresholdElatedLeft; } }
        [SerializeField]
        private float thresholdWarningLeft = Constants_Quille.DEFAULT_THRESHOLD_WARNING;
        public float ThresholdWarningLeft { get { return thresholdWarningLeft; } }
        [SerializeField]
        private float thresholdCriticalLeft = Constants_Quille.DEFAULT_THRESHOLD_CRITICAL;
        public float ThresholdCriticalLeft { get { return thresholdCriticalLeft; } }
        [SerializeField]
        private float thresholdElatedRight = Constants_Quille.DEFAULT_THRESHOLD_ELATED;
        public float ThresholdElatedRight { get { return thresholdElatedRight; } }
        [SerializeField]
        private float thresholdWarningRight = Constants_Quille.DEFAULT_THRESHOLD_WARNING;
        public float ThresholdWarningRight { get { return thresholdWarningRight; } }
        [SerializeField]
        private float thresholdCriticalRight = Constants_Quille.DEFAULT_THRESHOLD_CRITICAL;
        public float ThresholdCriticalRight { get { return thresholdCriticalRight; } }


        // MODULATORS
        //Default values modulated by ? (List of functions/references)
        // Left
        [SerializeField] private ChecksAndMods.ModulatorArithmeticFromFloat[] baseAIWeightingModulatedByLeft;
        public ChecksAndMods.ModulatorArithmeticFromFloat[] BaseAIWeightingModulatedByLeft { get { return baseAIWeightingModulatedByLeft; } }

        [SerializeField] private ChecksAndMods.ModulatorArithmeticFromFloat[] baseChangeRateModulatedByLeft;
        public ChecksAndMods.ModulatorArithmeticFromFloat[] BaseChangeRateModulatedByLeft { get { return baseChangeRateModulatedByLeft; } }

        [SerializeField] private ChecksAndMods.ModulatorArithmeticFromFloat[] thresholdsModulatedByLeft;
        public ChecksAndMods.ModulatorArithmeticFromFloat[] ThresholdsModulatedByLeft { get { return thresholdsModulatedByLeft; } }
        // Right
        [SerializeField] private ChecksAndMods.ModulatorArithmeticFromFloat[] baseAIWeightingModulatedByRight;
        public ChecksAndMods.ModulatorArithmeticFromFloat[] BaseAIWeightingModulatedByRight { get { return baseAIWeightingModulatedByRight; } }

        [SerializeField] private ChecksAndMods.ModulatorArithmeticFromFloat[] baseChangeRateModulatedByRight;
        public ChecksAndMods.ModulatorArithmeticFromFloat[] BaseChangeRateModulatedByRight { get { return baseChangeRateModulatedByRight; } }

        [SerializeField] private ChecksAndMods.ModulatorArithmeticFromFloat[] thresholdsModulatedByRight;
        public ChecksAndMods.ModulatorArithmeticFromFloat[] ThresholdsModulatedByRight { get { return thresholdsModulatedByRight; } }

        // TODO: how to handle the multiple thresholds?
    }
}