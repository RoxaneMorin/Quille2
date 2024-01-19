using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    [System.Serializable]
    public class BasicNeed
    {
        // TEMP
        public NeedBar myNeedBar; // comment faire pour que le need n'ai pas besoin de connaitre sa bar?


        // VARIABLES
        [SerializeField] private BasicNeedSO needSO;

        [SerializeField]
        private float localAiPriorityWeighting;

        [SerializeField]
        private float levelFull,
                      levelCurrent; // init this?

        [SerializeField]
        private float baseChangeRate, // this need's base decay rate for its owner character.
                      currentChangeRate; // rename variable to 'change rate'?

        [SerializeField]
        private float thresholdWarning,
                      thresholdCritical;
        
        private bool isWarning;
        private bool isCritical;

        //Default values modulated by ? (List of functions/references)


        // PROPERTIES
        public string NeedName { get { return needSO.NeedName; } }
        public Sprite NeedIcon { get { return needSO.needIcon; } }

        public float AiPriorityWeighting { get { return needSO.AiPriorityWeighting; } }
        public float LocalAiPriorityWeighting
        {
            get { return localAiPriorityWeighting; }
            set
            {
                if (value < Constants.MIN_PRIORITY)
                {
                    localAiPriorityWeighting = Constants.MIN_PRIORITY;
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

        public float LevelEmpty { get { return needSO.LevelEmpty; } }
        public float DefaultLevelFull { get { return needSO.LevelFull; } }
        public float LevelFull
        {
            get { return levelFull; }
            set
            {
                if (value > LevelEmpty)
                {
                    levelFull = value;
                }
            }
        }
        public float LevelCurrent
        {
            get { return levelCurrent; }
            set
            {
                if (value >= DefaultLevelFull)
                {
                    levelCurrent = DefaultLevelFull;
                    return;
                }
                else if (value <= LevelEmpty)
                {
                    levelCurrent = LevelEmpty;
                    // Throw need failure event here?
                    return;
                }
                else levelCurrent = value;
            }
        }
        public float LevelCurrentAsPercentage
        {
            get { return levelCurrent / levelFull; }
        }

        public float DefaultChangeRate
        {
            get { return needSO.DefaultChangeRate; }
        }

        public float BaseChangeRate
        {
            get { return baseChangeRate;  }
            set { baseChangeRate = value; }
        }
        public void ResetBaseChangeRate()
        {
            baseChangeRate = needSO.DefaultChangeRate;
        }

        public float CurrentChangeRate
        {
            get { return currentChangeRate; }
            set { currentChangeRate = value; }
        }
        public void ResetCurrentChangeRate()
        {
            currentChangeRate = baseChangeRate;
        }

        public float DefaultThresholdWarning { get { return needSO.ThresholdWarning; } }
        public float DefaultThresholdCritical { get { return needSO.ThresholdCritical; } }
        public float ThresholdWarning
        {
            get { return thresholdWarning; }
            set
            {
                if (value > 1)
                {
                    thresholdWarning = 1;
                    return;
                }
                else if (value < 0)
                {
                    thresholdWarning = 0;
                    return;
                }
                else thresholdWarning = value;
            }
        }
        public float ThresholdCritical
        {
            get { return thresholdCritical; }
            set
            {
                if (value > 1)
                {
                    thresholdCritical = 1;
                    return;
                }
                else if (value < 0)
                {
                    thresholdCritical = 0;
                    return;
                }
                else thresholdCritical = value;
            }
        }
        public void ResetThresholds()
        {
            ThresholdWarning = DefaultThresholdWarning;
            thresholdCritical = DefaultThresholdCritical;
        }



        // CONSTRUCTORS

        public BasicNeed(BasicNeedSO needSO) 
        {
            this.needSO = needSO;
            SetParametersFromSO();
        }

        private void SetParametersFromSO()
        {
            LocalAiPriorityWeighting = AiPriorityWeighting;

            LevelFull = DefaultLevelFull;
            LevelCurrent = DefaultLevelFull;

            BaseChangeRate = DefaultChangeRate;
            CurrentChangeRate = DefaultChangeRate;

            ThresholdWarning = DefaultThresholdWarning;
            ThresholdCritical = DefaultThresholdCritical;
        }



        // OVERRIDES
        public override string ToString()
        {
            return string.Format("{0}: {1:P2} fulfilled.\nBase decay rate: {2}.\nCurrent decay rate: {3}.", NeedName, 1 - GetFulfillmentDelta(true), BaseChangeRate, CurrentChangeRate);
        }



        // METHODS

        // GetFullfilment*
        public float GetFulfillmentDelta(bool asPercentage = false) // How 'far' are we from a fully fulfilled need?
        {
            float delta = LevelFull - LevelCurrent;
            return asPercentage ? (delta / LevelFull) : delta;
        }

        // SortByFullfilment*
        // Sorts an array of basic needs by the difference between their maximum and current fulfillment levels. The most drastic difference comes first.
        public static void SortByFulfillmentDelta(BasicNeed[] basicNeeds, bool usePriorityWeights = true, bool byPercentage = false)
        {
            SortHelper_BasicNeedsbyDelta sortHelper = new SortHelper_BasicNeedsbyDelta(usePriorityWeights, byPercentage);
            Array.Sort(basicNeeds, sortHelper);
            //Array.Reverse(basicNeeds);
        }
        
        // Return neediest of the set.

        // COMPARISON HELPERS
        class SortHelper_BasicNeedsbyDelta : IComparer
        {
            bool usePriorityWeights;
            bool byPercentage;
            public SortHelper_BasicNeedsbyDelta(bool usePriorityWeights, bool byPercentage)
            {
                this.usePriorityWeights = usePriorityWeights;
                this.byPercentage = byPercentage;
            }

            // Needs will be ordered from largest to smallest fulfillment delta.
            int IComparer.Compare(object a, object b)
            {
                BasicNeed needA = (BasicNeed)a;
                BasicNeed needB = (BasicNeed)b;

                float needDeltaA = needA.GetFulfillmentDelta(byPercentage);
                float needDeltaB = needB.GetFulfillmentDelta(byPercentage);

                if (usePriorityWeights)
                {
                    needDeltaA *= needA.localAiPriorityWeighting;
                    needDeltaB *= needB.localAiPriorityWeighting;
                }

                if (needDeltaA < needDeltaB)
                    return 1;
                if (needDeltaA > needDeltaB)
                    return -1;
                else
                    return 0;
            }
        }



        // Runtime
        // Every second, alter this need's fulfillment level by its current change rate.
        public IEnumerator AlterLevelByChangeRate()
        {
            while(this.LevelCurrent > this.LevelEmpty )
            {
                this.LevelCurrent += this.CurrentChangeRate;

                // Threshold detection.
                if (!isCritical & this.LevelCurrentAsPercentage <= this.ThresholdCritical)
                {
                    Debug.Log(string.Format("{0} is critically low ({1:P2})...", this.NeedName, 1 - GetFulfillmentDelta(true)));
                    isCritical = true;
                }
                else if (!isWarning & this.LevelCurrentAsPercentage <= this.ThresholdWarning)
                {
                    Debug.Log(string.Format("{0} is a little low ({1:P2})...", this.NeedName, 1 - GetFulfillmentDelta(true)));
                    isWarning = true;
                }

                myNeedBar.UpdateFill(this.LevelCurrent); // TO DO: handle this elsewhere; this object shouldn't know the UI.

                yield return new WaitForSeconds(1);
            }

            // Do need failure here?
            Debug.Log(string.Format("{0} is now empty.", this.NeedName));

            // TO DO: ability to bounce back while the need is empty?
        }
        
    }

}
