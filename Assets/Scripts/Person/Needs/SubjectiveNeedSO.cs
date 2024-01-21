using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
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
        [SerializeField, Range(Constants.MIN_PRIORITY, Constants.MAX_PRIORITY)] // Priorize larger values? 
        private float aiPriorityWeighting = 1; // Should this be static, since it'll likely by the same for all characters?
        public float AiPriorityWeighting { get { return aiPriorityWeighting; } }
        // Are separate priorities needed for the two sides? Only bother on a per character basis?

        [SerializeField] // not sure about the nomenclature for these. level, range, gauge, etc? 
        private float levelFullLeft = Constants.DEFAULT_LEVEL_FULL;
        public float LevelFullLeft { get { return levelFullLeft; } }
        public float LevelEmptyLeft { get { return Constants.DEFAULT_LEVEL_EMPTY; } }// Will always be 0?
        [SerializeField]
        private float levelFullRight = Constants.DEFAULT_LEVEL_FULL;
        public float LevelFullRight { get { return levelFullRight; } }
        public float LevelEmptyRight { get { return Constants.DEFAULT_LEVEL_EMPTY; } }

        [SerializeField]
        private float defaultChangeRateLeft = 0;
        public float DefaultChangeRateLeft { get { return defaultChangeRateLeft; } }
        [SerializeField]
        private float defaultChangeRateRight = 0;
        public float DefaultChangeRateRight { get { return defaultChangeRateRight; } }

        [SerializeField]
        private float thresholdWarningLeft = Constants.DEFAULT_THRESHOLD_WARNING;
        public float ThresholdWarningLeft { get { return thresholdWarningLeft; } }
        [SerializeField]
        private float thresholdCriticalLeft = Constants.DEFAULT_THRESHOLD_CRITICAL;
        public float ThresholdCriticalLeft { get { return thresholdCriticalLeft; } }
        [SerializeField]
        private float thresholdWarningRight = Constants.DEFAULT_THRESHOLD_WARNING;
        public float ThresholdWarningRight { get { return thresholdWarningRight; } }
        [SerializeField]
        private float thresholdCriticalRight = Constants.DEFAULT_THRESHOLD_CRITICAL;
        public float ThresholdCriticalRight { get { return thresholdCriticalRight; } }

        //Default values modulated by ? (List of functions/references)
    }
}