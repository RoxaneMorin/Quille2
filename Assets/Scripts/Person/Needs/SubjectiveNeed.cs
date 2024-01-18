using System;
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

        public float LevelEmptyLeft { get { return needSO.LevelEmptyLeft; } }
        public float LevelEmptyRight { get { return needSO.LevelEmptyRight; } }
        public (float, float) LevelEmpty { get { return (needSO.LevelEmptyLeft, needSO.LevelEmptyRight); } }
        public float LevelFullLeft { get { return needSO.LevelFullLeft; } }
        public float LevelFullRight { get { return needSO.LevelFullRight; } }
        public (float, float) LevelFull { get { return (needSO.LevelFullLeft, needSO.LevelFullRight); } }
        public float LocalLevelFullLeft
        {
            get { return localLevelFullLeft; }
            set
            {
                if (value > LevelEmptyLeft)
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
                if (value > LevelEmptyRight)
                {
                    localLevelFullRight = value;
                }
            }
        }
        public (float, float) LocalLevelFull
        {
            get { return (localLevelFullLeft, localLevelFullRight); }
            set
            {
                LocalLevelFullLeft = value.Item1;
                LocalLevelFullRight = value.Item1;
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
        public (float, float) LevelCurrent
        {
            get { return (levelCurrentLeft, levelCurrentRight); }
            set
            {
                LevelCurrentLeft = value.Item1;
                LevelCurrentRight = value.Item2;
            }
        }

        public float DefaultChangeRateLeft
        {
            get { return needSO.DefaultChangeRateLeft; }
        }
        public float DefaultChangeRateRight
        {
            get { return needSO.DefaultChangeRateRight; }
        }
        public (float, float) DefaultChangeRate
        {
            get { return (needSO.DefaultChangeRateLeft, needSO.DefaultChangeRateRight); }
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
        public (float, float) BaseChangeRate
        {
            get { return (baseChangeRateLeft, baseChangeRateRight); }
            set
            {
                BaseChangeRateLeft = value.Item1;
                BaseChangeRateRight = value.Item2;
            }
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
        public (float, float) CurrentChangeRate
        {
            get { return (currentChangeRateLeft, currentChangeRateRight); }
            set
            {
                CurrentChangeRateLeft = value.Item1;
                CurrentChangeRateRight = value.Item2;
            }
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

            LocalLevelFull = (LevelFullLeft, LevelFullRight);
            LevelCurrent = (LevelFullLeft, LevelFullRight);

            BaseChangeRate = (DefaultChangeRateLeft, DefaultChangeRateRight);
            CurrentChangeRate = (DefaultChangeRateLeft, DefaultChangeRateRight);
        }



        // OVERRIDES
        // ToString
        public override string ToString()
        {
            return string.Format("{0}:\n\n" +
                "{1}: {2:P2} fulfilled.\nBase decay rate: {3}.\nCurrent decay rate: {4}.\n\n" +
                "{5}: {6:P2} fulfilled.\nBase decay rate: {7}.\nCurrent decay rate: {8}.\n\n" +
                "The neediest subneed is {9}.\n" +
                "The total need fulfilment is {10:P2}, the average is {11:P2}.",
                NeedName, 
                NeedNameLeft, 1 - GetLeftFulfillmentDelta(true), BaseChangeRateLeft, CurrentChangeRateLeft,
                NeedNameRight, 1 - GetRightFulfillmentDelta(true), BaseChangeRateRight, CurrentChangeRateRight,
                (GetNeediestSide() ? NeedNameRight : NeedNameLeft),
                1-GetTotalFulfillmentDelta(), 1-GetAverageFulfillmentDelta()
                );
        }



        // METHODS

        // GetFullfilment*
        public float GetLeftFulfillmentDelta(bool asPercentage = false) // How 'far' are we from a fully fulfilled need? The higher the value, the needier the need.
        {
            float delta = LevelFullLeft - LevelCurrentLeft;
            return asPercentage ? delta : delta / LevelFullLeft;
        }
        public float GetRightFulfillmentDelta(bool asPercentage = false)
        {
            float delta = LevelFullRight - LevelCurrentRight;
            return asPercentage ? delta : delta / LevelFullRight;
        }
        public float GetTotalFulfillmentDelta(bool byPercentage = false) // Return the sum of both subneeds' fulfillment deltas.
        {
            float delta = GetLeftFulfillmentDelta() + GetRightFulfillmentDelta();
            return byPercentage ? delta / (LevelFullLeft + LevelFullRight) : delta;
        }
        public float GetAverageFulfillmentDelta(bool byPercentage = false) // Return the average fullfilment delta of the two subneeds.
        {
            return GetTotalFulfillmentDelta(byPercentage) / 2;
        }

        // Get other relevant information.
        public bool GetNeediestSide(bool byPercentage = false) // Returns which side of the need is the least fulfilled, where Left = 0, Right = 1.
        {
            return GetLeftFulfillmentDelta(byPercentage) <= GetRightFulfillmentDelta(byPercentage);
        }
        public float GetFulfillmentDifference(bool byPercentage = false) // Get the absolute difference between the two subneeds' levels of fulfillment.
        {
            return Mathf.Abs(GetLeftFulfillmentDelta(byPercentage) - GetRightFulfillmentDelta(byPercentage));
        }

        // SortByFullfilment*
        // Sorts an array of subjective needs by the difference between the fulfillment deltas of their neediest subneed. The most drastic difference comes first.
        public static void SortByFulfillmentDeltaofNeediest(SubjectiveNeed[] subjectiveNeeds, bool byPercentage = false)
        {
            SortHelper_SubjectiveNeedsbyDeltaOfNeediest sortHelper = new SortHelper_SubjectiveNeedsbyDeltaOfNeediest(byPercentage);
            Array.Sort(subjectiveNeeds, sortHelper);
            //Array.Reverse(subjectiveNeeds);
        }
        // Sorts an array of subjective needs by the difference between the total deltas of their subneeds. The most drastic difference comes first.
        public static void SortByTotalFulfillmentDelta(SubjectiveNeed[] subjectiveNeeds, bool byPercentage = false)
        {
            SortHelper_SubjectiveNeedsbyTotalDelta sortHelper = new SortHelper_SubjectiveNeedsbyTotalDelta(byPercentage);
            Array.Sort(subjectiveNeeds, sortHelper);
            //Array.Reverse(subjectiveNeeds);
        }
        // Sorts an array of subjective needs by the difference between the average deltas of their subneeds. The most drastic difference comes first.
        public static void SortByAverageFulfillmentDelta(SubjectiveNeed[] subjectiveNeeds, bool byPercentage = false)
        {
            SortHelper_SubjectiveNeedsbyAverageDelta sortHelper = new SortHelper_SubjectiveNeedsbyAverageDelta(byPercentage);
            Array.Sort(subjectiveNeeds, sortHelper);
            //Array.Reverse(subjectiveNeeds);
        }

        // COMPARISON HELPERS
        class SortHelper_SubjectiveNeedsbyDeltaOfNeediest : IComparer
        {
            bool byPercentage;
            public SortHelper_SubjectiveNeedsbyDeltaOfNeediest(bool byPercentage)
            {
                this.byPercentage = byPercentage;
            }

            // Needs will be ordered from largest to smallest fulfillment delta of their most deficient subneed.
            int IComparer.Compare(object a, object b)
            {
                SubjectiveNeed needA = (SubjectiveNeed)a;
                SubjectiveNeed needB = (SubjectiveNeed)b;

                float needDeltaA = needA.GetNeediestSide(byPercentage) ? needA.GetRightFulfillmentDelta(byPercentage) : needA.GetLeftFulfillmentDelta(byPercentage);
                float needDeltaB = needB.GetNeediestSide(byPercentage) ? needB.GetRightFulfillmentDelta(byPercentage) : needB.GetLeftFulfillmentDelta(byPercentage);

                if (needDeltaA < needDeltaB)
                    return 1;
                if (needDeltaA > needDeltaB)
                    return -1;
                else
                    return 0;
            }
        }
        class SortHelper_SubjectiveNeedsbyTotalDelta : IComparer
        {
            bool byPercentage;
            public SortHelper_SubjectiveNeedsbyTotalDelta(bool byPercentage)
            {
                this.byPercentage = byPercentage;
            }

            // Needs will be ordered from largest to smallest total fulfillment delta.
            int IComparer.Compare(object a, object b)
            {
                SubjectiveNeed needA = (SubjectiveNeed)a;
                SubjectiveNeed needB = (SubjectiveNeed)b;

                float needDeltaA = needA.GetTotalFulfillmentDelta(byPercentage);
                float needDeltaB = needB.GetTotalFulfillmentDelta(byPercentage);

                if (needDeltaA < needDeltaB)
                    return 1;
                if (needDeltaA > needDeltaB)
                    return -1;
                else
                    return 0;
            }
        }
        class SortHelper_SubjectiveNeedsbyAverageDelta : IComparer
        {
            bool byPercentage;
            public SortHelper_SubjectiveNeedsbyAverageDelta(bool byPercentage)
            {
                this.byPercentage = byPercentage;
            }

            // Needs will be ordered from largest to smallest average fulfillment delta.
            int IComparer.Compare(object a, object b)
            {
                SubjectiveNeed needA = (SubjectiveNeed)a;
                SubjectiveNeed needB = (SubjectiveNeed)b;

                float needDeltaA = needA.GetAverageFulfillmentDelta(byPercentage);
                float needDeltaB = needB.GetAverageFulfillmentDelta(byPercentage);

                if (needDeltaA < needDeltaB)
                    return 1;
                if (needDeltaA > needDeltaB)
                    return -1;
                else
                    return 0;
            }
        }
        // TO DO: implement facultative use of the AI weight.


        // Runtime
        // Every second, alter this need's fulfillment level by its current change rate.
        public IEnumerator AlterLevelByChangeRate()
        {
            while (this.LevelCurrentLeft > this.LevelEmptyLeft | this.LevelCurrentRight > this.LevelEmptyRight)
            {
                this.LevelCurrentLeft += this.CurrentChangeRateLeft;
                this.LevelCurrentRight += this.CurrentChangeRateRight;

                yield return new WaitForSeconds(1);
            }

            // Do need failure here?
            Debug.Log("The need is now fully empty.");
        }
    }
}
