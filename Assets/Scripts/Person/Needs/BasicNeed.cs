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
        public NeedBar myNeedBar;
        
        
        // VARIABLES
        [SerializeField] private BasicNeedSO needSO;

        [SerializeField]
        private int localAiPriorityWeighting; // ?

        [SerializeField]
        private float localLevelFull,
                      levelCurrent; // init this?

        [SerializeField]
        private float baseChangeRate, // this need's base decay rate for its owner character.
                      currentChangeRate; // rename variable to 'change rate'?

        //Default values modulated by ? (List of functions/references)


        // PROPERTIES
        public string NeedName { get { return needSO.NeedName; } }
        public Sprite NeedIcon { get { return needSO.needIcon; } }

        public int AiPriorityWeighting { get { return needSO.AiPriorityWeighting; } }
        public int LocalAiPriorityWeighting
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
        public float LevelFull { get { return needSO.LevelFull; } }
        public float LocalLevelFull
        {
            get { return localLevelFull; }
            set
            {
                if (value > LevelEmpty)
                {
                    localLevelFull = value;
                }
            }
        }
        public float LevelCurrent
        {
            get { return levelCurrent; }
            set
            {
                if (value >= LevelFull)
                {
                    levelCurrent = LevelFull;
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



        // CONSTRUCTORS

        public BasicNeed(BasicNeedSO needSO) 
        {
            this.needSO = needSO;
            SetParametersFromSO();
        }

        private void SetParametersFromSO()
        {
            LocalAiPriorityWeighting = AiPriorityWeighting;

            LocalLevelFull = LevelFull;
            LevelCurrent = LevelFull;

            BaseChangeRate = DefaultChangeRate;
            CurrentChangeRate = DefaultChangeRate;
        }



        // OVERRIDES

        public override string ToString()
        {
            return string.Format("{0}: {1}% fulfilled.\nBase decay rate: {2}.\nCurrent decay rate: {3}.", NeedName, 1 - GetFulfillmentDelta(true), BaseChangeRate, CurrentChangeRate);
        }



        // METHODS


        // GetFullfilment*
        public float GetFulfillmentDelta(bool asPercentage = false) // How 'far' are we from a fully fulfilled need?
        {
            float delta = LevelFull - LevelCurrent;
            return asPercentage ? delta / LevelFull : delta;
        }

        // SortByFullfilment*

        // Sorts an array of basic needs by the difference between their maximum and current fulfillment levels. The most drastic difference comes first.
        public static void SortByFulfillmentDelta(BasicNeed[] basicNeeds, bool byPercentage = false)
        {
            SortHelper_BasicNeedsbyDelta sortHelper = new SortHelper_BasicNeedsbyDelta(byPercentage);
            Array.Sort(basicNeeds, sortHelper);
            //Array.Reverse(basicNeeds);
        }

        // COMPARISON HELPERS
        class SortHelper_BasicNeedsbyDelta : IComparer
        {
            bool byPercentage;
            public SortHelper_BasicNeedsbyDelta(bool byPercentage)
            {
                this.byPercentage = byPercentage;
            }

            // Needs will be ordered from largest to smallest fulfillment delta.
            int IComparer.Compare(object a, object b)
            {
                BasicNeed needA = (BasicNeed)a;
                BasicNeed needB = (BasicNeed)b;

                float needDeltaA = needA.GetFulfillmentDelta(byPercentage);
                float needDeltaB = needB.GetFulfillmentDelta(byPercentage);

                if (needDeltaA < needDeltaB)
                    return 1;
                if (needDeltaA > needDeltaB)
                    return -1;
                else
                    return 0;
            }
        }





        // 

        // Every second, alter this need's fulfillment level by its current change rate.
        public IEnumerator AlterLevelByChangeRate()
        {
            while(this.LevelCurrent > this.LevelEmpty )
            {
                this.LevelCurrent += this.CurrentChangeRate;

                myNeedBar.UpdateFill(this.LevelCurrent);

                yield return new WaitForSeconds(1);
            }

            // Do need failure here?
            Debug.Log("The need is now empty.");
        }
        
    }

}
