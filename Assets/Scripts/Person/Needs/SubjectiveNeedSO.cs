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
        [SerializeField] private string needName = "Undefined";
        public string NeedName { get { return needName; } }

        // SUBNEEDS
        [SerializeField] private BasicNeedSO leftNeedSO;
        [SerializeField] private BasicNeedSO rightNeedSO;
        
        // Description.


        // PROPERTIES
        public string NeedNameLeft { get { return leftNeedSO != null ? leftNeedSO.NeedName : "Undefined"; } }
        public string NeedNameRight { get { return rightNeedSO != null ? rightNeedSO.NeedName : "Undefined"; } }

        public Sprite NeedIconLeft { get { return leftNeedSO != null ? leftNeedSO.NeedIcon : null; } }
        public Sprite NeedIconRight { get { return rightNeedSO != null ? rightNeedSO.NeedIcon : null; } }

        public float AIPriorityWeightingLeft { get { return leftNeedSO != null ? leftNeedSO.AiPriorityWeighting : 0f; } }
        public float AIPriorityWeightingRight {get { return rightNeedSO != null ? rightNeedSO.AiPriorityWeighting : 0f; } }

        public float LevelFullLeft { get { return leftNeedSO != null ? leftNeedSO.LevelFull : Constants_Quille.DEFAULT_LEVEL_FULL; } }
        public float LevelEmptyLeft { get { return leftNeedSO != null ? leftNeedSO.LevelEmpty : Constants_Quille.DEFAULT_LEVEL_EMPTY; } }
        public float LevelFullRight { get { return rightNeedSO != null ? rightNeedSO.LevelFull : Constants_Quille.DEFAULT_LEVEL_FULL; } }
        public float LevelEmptyRight { get { return rightNeedSO != null ? rightNeedSO.LevelEmpty : Constants_Quille.DEFAULT_LEVEL_EMPTY; } }

        public float ThresholdElatedLeft { get { return leftNeedSO != null ? leftNeedSO.ThresholdElated : Constants_Quille.DEFAULT_THRESHOLD_ELATED; } }
        public float ThresholdWarningLeft { get { return leftNeedSO != null ? leftNeedSO.ThresholdWarning : Constants_Quille.DEFAULT_THRESHOLD_WARNING; } }
        public float ThresholdCriticalLeft { get { return leftNeedSO != null ? leftNeedSO.ThresholdCritical : Constants_Quille.DEFAULT_THRESHOLD_CRITICAL; } }
        public float ThresholdElatedRight { get { return rightNeedSO != null ? rightNeedSO.ThresholdElated : Constants_Quille.DEFAULT_THRESHOLD_ELATED; } }
        public float ThresholdWarningRight { get { return rightNeedSO != null ? rightNeedSO.ThresholdWarning : Constants_Quille.DEFAULT_THRESHOLD_WARNING; } }
        public float ThresholdCriticalRight { get { return rightNeedSO != null ? rightNeedSO.ThresholdCritical : Constants_Quille.DEFAULT_THRESHOLD_CRITICAL; } }

        public float DefaultChangeRateLeft { get { return leftNeedSO != null ? leftNeedSO.DefaultChangeRate : 0f; } }
        public float DefaultChangeRateRight { get { return rightNeedSO != null ? rightNeedSO.DefaultChangeRate : 0f; } }


        // MODULATORS
        public ChecksAndMods.ModulatorArithmeticFromFloat[] BaseAIWeightingModulatedByLeft
        {
            get { return leftNeedSO != null ? leftNeedSO.BaseAIWeightingModulatedBy : null; }
        }
        public ChecksAndMods.ModulatorArithmeticFromFloat[] BaseChangeRateModulatedByLeft
        {
            get { return leftNeedSO != null ? leftNeedSO.BaseChangeRateModulatedBy : null; }
        }
        public ChecksAndMods.ModulatorArithmeticFromFloat[] ThresholdsModulatedByLeft
        {
            get { return leftNeedSO != null ? leftNeedSO.ThresholdsModulatedBy : null; }
        }

        public ChecksAndMods.ModulatorArithmeticFromFloat[] BaseAIWeightingModulatedByRight
        {
            get { return rightNeedSO != null ? rightNeedSO.BaseAIWeightingModulatedBy : null; }
        }
        public ChecksAndMods.ModulatorArithmeticFromFloat[] BaseChangeRateModulatedByRight
        {
            get { return rightNeedSO != null ? rightNeedSO.BaseChangeRateModulatedBy : null; }
        }
        public ChecksAndMods.ModulatorArithmeticFromFloat[] ThresholdsModulatedByRight
        {
            get { return rightNeedSO != null ? rightNeedSO.ThresholdsModulatedBy : null; }
        }
    }
}