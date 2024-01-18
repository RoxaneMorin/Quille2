using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    [System.Serializable]
    public class SubjectiveNeed
    {
        // TEMP

        // VARIABLES
        [SerializeField] private SubjectiveNeedSO needSO;

        [SerializeField]
        private int localAiPriorityWeighting; // ?
        // Are separate priorities needed for the two sides?

        [SerializeField]
        private float localLevelFullLeft,
                      levelCurrentLeft;
        [SerializeField]
        private float localLevelFullRight,
                      levelCurrentRight;

        [SerializeField]
        private float baseChangeRateLeft,
                      currentChangeRateLeft;
        [SerializeField]
        private float baseChangeRateRight,
                      currentChangeRateRight;

        //Default values modulated by ? (List of functions/references)


        // PROPERTIES
        public string NeedName { get { return needSO.NeedName; } }
        public string NeedNameLeft { get { return needSO.NeedNameLeft; } }
        public string NeedNameRight { get { return needSO.NeedNameRight; } }
        public Sprite NeedIconLeft { get { return needSO.needIconLeft; } }
        public Sprite NeedIconRight { get { return needSO.needIconRight; } }

        public int AiPriorityWeighting { get { return needSO.AiPriorityWeighting; } }
        public int LocalAiPriorityWeighting
        {
            get { return localAiPriorityWeighting; }
            set
            {
                if (value < 0)
                {
                    localAiPriorityWeighting = 0;
                    return;
                }
                else if (value > Constants.MAX_PRIORITY)
                {
                    localAiPriorityWeighting = Constants.MAX_PRIORITY;
                    return;
                }
                else localAiPriorityWeighting = value;
            }
        }

        public float LevelEmptyLeft { get { return needSO.LevelEmptyLeft; } }
        public float LevelEmptyRight { get { return needSO.LevelEmptyRight; } }
        public float LevelFullLeft { get { return needSO.LevelFullLeft; } }
        public float LevelFullRight { get { return needSO.LevelFullRight; } }
        public float LocalLevelFullLeft
        {
            get { return localLevelFullLeft; }
            set
            {
                if (value > LocalLevelFullLeft)
                {
                    localLevelFullLeft = value;
                }
            }
        }
        public float LocalLevelFullRight
        {
            get { return localLevelFullRight; }
            set
            {
                if (value > LocalLevelFullRight)
                {
                    localLevelFullRight = value;
                }
            }
        }
        public float LevelCurrentLeft
        {
            get { return levelCurrentLeft; }
            set
            {
                if (value >= LevelFullLeft)
                {
                    levelCurrentLeft = LevelFullLeft;
                    return;
                }
                else if (value <= LevelEmptyLeft)
                {
                    levelCurrentLeft = LevelEmptyLeft;
                    // Throw need failure event here?
                    return;
                }
                else levelCurrentLeft = value;
            }
        }
        public float LevelCurrentRight
        {
            get { return levelCurrentRight; }
            set
            {
                if (value >= LevelFullRight)
                {
                    levelCurrentRight = LevelFullRight;
                    return;
                }
                else if (value <= LevelEmptyRight)
                {
                    levelCurrentRight = LevelEmptyRight;
                    // Throw need failure event here?
                    return;
                }
                else levelCurrentRight = value;
            }
        }

        virtual public float DefaultChangeRateLeft
        {
            get { return needSO.DefaultChangeRateLeft; }
        }
        virtual public float DefaultChangeRateRight
        {
            get { return needSO.DefaultChangeRateRight; }
        }

        public float BaseChangeRateLeft
        {
            get { return baseChangeRateLeft; }
            set { baseChangeRateLeft = value; }
        }
        public float BaseChangeRateRight
        {
            get { return baseChangeRateRight; }
            set { baseChangeRateRight = value; }
        }

        public void ResetBaseChangeRateLeft()
        {
            baseChangeRateLeft = needSO.DefaultChangeRateLeft;
        }
        public void ResetBaseChangeRateRight()
        {
            baseChangeRateRight = needSO.DefaultChangeRateRight;
        }
        public void ResetBaseChangeRate()
        {
            ResetBaseChangeRateLeft();
            ResetBaseChangeRateRight();
        }

        public float CurrentChangeRateLeft
        {
            get { return currentChangeRateLeft; }
            set { currentChangeRateLeft = value; }
        }
        public float CurrentChangeRateRight
        {
            get { return currentChangeRateRight; }
            set { currentChangeRateRight = value; }
        }

        public void ResetCurrentChangeRateLeft()
        {
            currentChangeRateLeft = baseChangeRateLeft;
        }
        public void ResetCurrentChangeRateRight()
        {
            currentChangeRateRight = baseChangeRateRight;
        }
        public void ResetCurrentChangeRate()
        {
            ResetCurrentChangeRateLeft();
            ResetCurrentChangeRateRight();
        }



        /// CONSTRUCTORS
        public SubjectiveNeed(SubjectiveNeedSO needSO)
        {
            this.needSO = needSO;
            SetParametersFromSO();
        }

        private void SetParametersFromSO()
        {
            LocalAiPriorityWeighting = AiPriorityWeighting;

            LocalLevelFullLeft = LevelFullLeft;
            LocalLevelFullRight = LevelFullRight;
            LevelCurrentLeft = LevelFullLeft;
            LevelCurrentRight = LevelFullRight;

            BaseChangeRateLeft = DefaultChangeRateLeft;
            BaseChangeRateRight = DefaultChangeRateRight;
            CurrentChangeRateLeft = DefaultChangeRateLeft;
            CurrentChangeRateRight = DefaultChangeRateRight;
        }



        // OVERRIDES
        // ToString



        // METHODS
    }
}
