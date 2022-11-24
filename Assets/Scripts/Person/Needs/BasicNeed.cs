using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    [System.Serializable]
    public class BasicNeed
    {
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
                if (value < 0)
                {
                    localAiPriorityWeighting = 0;
                    return;
                }
                else if (value > 5)
                {
                    localAiPriorityWeighting = 5;
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
                if (value > LocalLevelFull)
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

        virtual public float DefaultChangeRate
        {
            get { return needSO.DefaultChangeRate; }
        }

        public float BaseChangeRate
        {
            get { return baseChangeRate;  }
            set { baseChangeRate = value; }
        }
        public void ResetBaseDecayRate()
        {
            baseChangeRate = needSO.DefaultChangeRate;
        }

        public float CurrentChangeRate
        {
            get { return currentChangeRate; }
            set { currentChangeRate = value; }
        }
        public void ResetCurrentDecayRate()
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
            return string.Format("{0}: {1}% fulfilled.\nBase decay rate: {2}.\nCurrent decay rate: {3}.", NeedName, GetFulfillmentPercentage(), BaseChangeRate, CurrentChangeRate);
        }



        // METHODS




        // GetFullfilment*

        public float GetFulfillmentDelta() // How 'far' are we from a fully fulfilled need?
        {
            return LevelFull - LevelCurrent;
        }

        public float GetFulfillmentDeltaAsPercentage() // How 'far' are we from a fully fulfilled need, as a percentage?
        {
            return (GetFulfillmentDelta() / LevelFull) * 100 ;
        }

        public float GetFulfillmentPercentage() // Currently, how fulfilled is the need, as a percentage?
        {
            return (LevelCurrent / LevelFull) * 100;
        }


        // SortByFullfilment*

        // Sorts an array of basic needs by the absolute difference between their maximum and current fulfillment levels. The most drastic difference comes first.
        public static void SortByFulfillmentDelta(BasicNeed[] basicNeeds)
        {
            SortHelper_BasicNeedsbyDelta sortHelper = new SortHelper_BasicNeedsbyDelta();
            Array.Sort(basicNeeds, sortHelper);
            //Array.Reverse(basicNeeds);
        }

        //Sorts an array of basic needs by the difference between their maximum and current fulfillment levels as percentages. The most dramatic difference comes first.
        public static void SortByFulfillmentDeltaAsPercentage(BasicNeed[] basicNeeds)
        {
            SortHelper_BasicNeedsbyDeltaPercentage sortHelper = new SortHelper_BasicNeedsbyDeltaPercentage();
            Array.Sort(basicNeeds, sortHelper);
            //Array.Reverse(basicNeeds);
        }

        // Sorts an array of basic needs by their level of fulfillment as percentages. The least fulfilled need comes first.
        public static void SortByFulfillmentPercentage(BasicNeed[] basicNeeds)
        {
            SortHelper_BasicNeedsbyFulfillmentPercentage sortHelper = new SortHelper_BasicNeedsbyFulfillmentPercentage();
            Array.Sort(basicNeeds, sortHelper);
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

        
        








        // COMPARISON HELPERS

        // ça ne me laisse pas le mettre private de manière explicite?
        class SortHelper_BasicNeedsbyDelta : IComparer
        {
            // Needs will be ordered from largest to smallest fulfillment delta.
            int IComparer.Compare(object a, object b)
            {
                BasicNeed needA = (BasicNeed)a;
                BasicNeed needB = (BasicNeed)b;

                float needDeltaA = needA.GetFulfillmentDelta();
                float needDeltaB = needB.GetFulfillmentDelta();

                if (needDeltaA < needDeltaB)
                    return 1;
                if (needDeltaA > needDeltaB)
                    return -1;
                else
                    return 0;
            }
        }

        class SortHelper_BasicNeedsbyDeltaPercentage : IComparer 
        {
            // Needs will be ordered from largest to smallest percentile fulfillment delta.
            int IComparer.Compare(object a, object b)
            {
                BasicNeed needA = (BasicNeed)a;
                BasicNeed needB = (BasicNeed)b;

                float needDeltaA = needA.GetFulfillmentDeltaAsPercentage();
                float needDeltaB = needB.GetFulfillmentDeltaAsPercentage();

                if (needDeltaA < needDeltaB)
                    return 1;
                if (needDeltaA > needDeltaB)
                    return -1;
                else
                    return 0;
            }
        }

        class SortHelper_BasicNeedsbyFulfillmentPercentage : IComparer
        {
            // Needs will be ordered from smallest to largest percentile fulfillment value.
            int IComparer.Compare(object a, object b)
            {
                BasicNeed needA = (BasicNeed)a;
                BasicNeed needB = (BasicNeed)b;

                float needFulfillmentA = needA.GetFulfillmentPercentage();
                float needFulfillmentB = needB.GetFulfillmentPercentage();

                if (needFulfillmentA > needFulfillmentB)
                    return 1;
                if (needFulfillmentA < needFulfillmentB)
                    return -1;
                else
                    return 0;
            }
        }
    }

}
