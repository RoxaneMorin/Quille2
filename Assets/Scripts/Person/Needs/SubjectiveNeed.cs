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


        // GetFullfilment*

        public float GetLeftFulfillmentDelta() // How 'far' are we from a fully fulfilled need?
        {
            return LevelFullLeft - LevelCurrentLeft;
        }
        public float GetRightFulfillmentDelta()
        {
            return LevelFullRight - LevelCurrentRight;
        }
        public float GetTotalFulfillmentDelta() // Return the sum of both subneeds' raw fulfillment deltas.
        {
            return GetLeftFulfillmentDelta() + GetRightFulfillmentDelta();
        }

        public float GetLeftFulfillmentDeltaAsPercentage() // How 'far' are we from a fully fulfilled need, as a percentage?
        {
            return (GetLeftFulfillmentDelta() / LevelFullLeft);
        }
        public float GetRightFulfillmentDeltaAsPercentage()
        {
            return (GetRightFulfillmentDelta() / LevelFullRight);
        }
        public float GetTotalFulfillmentDeltaAsPercentage() // Return the sum of both subneeds' fulfillment deltas as a percentage of their combined potential.
        {
            return ((GetLeftFulfillmentDelta() + GetRightFulfillmentDelta()) / (LevelFullLeft + LevelFullRight));
        }


        // Get other relevant information.

        public bool GetNeediestSide() // Returns which side of the need is the least fulfilled by raw value, where Left = 0, Right = 1.
        {
            return GetLeftFulfillmentDelta() >= GetRightFulfillmentDelta();
        }
        public bool GetNeediestSideFromPercentages() // Returns which side of the need is the least fulfilled by percentage, where Left = 0, Right = 1.
        {
            return GetLeftFulfillmentDeltaAsPercentage() >= GetRightFulfillmentDeltaAsPercentage();
        }

        public float GetFulfillmentDifference() // Get the absolute difference between the two subneeds' raw levels of fulfillment.
        {
            return Mathf.Abs(GetLeftFulfillmentDelta() - GetRightFulfillmentDelta());
        }
        public float GetFulfillmentDifferenceFromPercentages() // Get the absolute difference between the two subneeds' percentile levels of fulfillment.
        {
            return Mathf.Abs(GetLeftFulfillmentDeltaAsPercentage() - GetRightFulfillmentDeltaAsPercentage());
        }

        public float GetAverageFulfillmentDelta() // Return the average raw fullfilment delta of the two subneeds.
        {
            return GetTotalFulfillmentDelta() / 2;
        }
        public float GetAverageFulfillmentDeltaFromPercentages() // Return the average percentile fullfilment delta of the two subneeds.
        {
            return GetTotalFulfillmentDeltaAsPercentage() / 2;
        }


        // SortByFullfilment*

        // Sorts an array of subjective needs by the absolute difference between the maximum and current fulfillment levels of their neediest subneed. The most drastic difference comes first.
        public static void SortByFulfillmentDeltaofNeediest(SubjectiveNeed[] subjectiveNeeds)
        {
            SortHelper_SubjectiveNeedsbyDeltaOfNeediest sortHelper = new SortHelper_SubjectiveNeedsbyDeltaOfNeediest();
            Array.Sort(subjectiveNeeds, sortHelper);
            //Array.Reverse(subjectiveNeeds);
        }

        //Sorts an array of subjective needs by the percentile difference between the maximum and current fulfillment levels of their neediest subneed. The most dramatic difference comes first.
        public static void SortByFulfillmentDeltaofNeediestAsPercentage(SubjectiveNeed[] subjectiveNeeds)
        {
            SortHelper_SubjectiveNeedsbyDeltaOfNeediestPercentage sortHelper = new SortHelper_SubjectiveNeedsbyDeltaOfNeediestPercentage();
            Array.Sort(subjectiveNeeds, sortHelper);
            //Array.Reverse(subjectiveNeeds);
        }

        // Sorts an array of subjective needs by the average absolute difference between the maximum and current fulfillment levels of their subneeds. The most drastic difference comes first.
        public static void SortByAverageFulfillmentDelta(SubjectiveNeed[] subjectiveNeeds)
        {
            SortHelper_SubjectiveNeedsbyAverageDelta sortHelper = new SortHelper_SubjectiveNeedsbyAverageDelta();
            Array.Sort(subjectiveNeeds, sortHelper);
            //Array.Reverse(subjectiveNeeds);
        }

        // Sorts an array of subjective needs by the average percentile difference between the maximum and current fulfillment levels of their subneeds. The most drastic difference comes first.
        public static void SortByAverageFulfillmentDeltaAsPercentage(SubjectiveNeed[] subjectiveNeeds)
        {
            SortHelper_SubjectiveNeedsbyAverageDeltaPercentage sortHelper = new SortHelper_SubjectiveNeedsbyAverageDeltaPercentage();
            Array.Sort(subjectiveNeeds, sortHelper);
            //Array.Reverse(subjectiveNeeds);
        }


        // With the total deficit of both needs









        // COMPARISON HELPERS

        class SortHelper_SubjectiveNeedsbyDeltaOfNeediest : IComparer
        {
            // Needs will be ordered from largest to smallest fulfillment delta of their most deficient subneed.
            int IComparer.Compare(object a, object b)
            {
                SubjectiveNeed needA = (SubjectiveNeed)a;
                SubjectiveNeed needB = (SubjectiveNeed)b;

                float needDeltaA = needA.GetNeediestSide() ? needA.GetRightFulfillmentDelta() : needA.GetLeftFulfillmentDelta();
                float needDeltaB = needB.GetNeediestSide() ? needB.GetRightFulfillmentDelta() : needB.GetLeftFulfillmentDelta();

                if (needDeltaA < needDeltaB)
                    return 1;
                if (needDeltaA > needDeltaB)
                    return -1;
                else
                    return 0;
            }
        }

        class SortHelper_SubjectiveNeedsbyDeltaOfNeediestPercentage : IComparer
        {
            // Needs will be ordered from largest to smallest percentile fulfillment delta of their most deficient subneed.
            int IComparer.Compare(object a, object b)
            {
                SubjectiveNeed needA = (SubjectiveNeed)a;
                SubjectiveNeed needB = (SubjectiveNeed)b;

                float needDeltaA = needA.GetNeediestSideFromPercentages() ? needA.GetRightFulfillmentDeltaAsPercentage() : needA.GetLeftFulfillmentDeltaAsPercentage();
                float needDeltaB = needB.GetNeediestSideFromPercentages() ? needB.GetRightFulfillmentDeltaAsPercentage() : needB.GetLeftFulfillmentDeltaAsPercentage();

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
            // Needs will be ordered from largest to smallest total fulfillment delta.
            int IComparer.Compare(object a, object b)
            {
                SubjectiveNeed needA = (SubjectiveNeed)a;
                SubjectiveNeed needB = (SubjectiveNeed)b;

                float needDeltaA = needA.GetTotalFulfillmentDelta();
                float needDeltaB = needB.GetTotalFulfillmentDelta();

                if (needDeltaA < needDeltaB)
                    return 1;
                if (needDeltaA > needDeltaB)
                    return -1;
                else
                    return 0;
            }
        }

        class SortHelper_SubjectiveNeedsbyTotalDeltaPercentage : IComparer
        {
            // Needs will be ordered from largest to smallest percentile total fulfillment delta.
            int IComparer.Compare(object a, object b)
            {
                SubjectiveNeed needA = (SubjectiveNeed)a;
                SubjectiveNeed needB = (SubjectiveNeed)b;

                float needDeltaA = needA.GetTotalFulfillmentDeltaAsPercentage();
                float needDeltaB = needB.GetTotalFulfillmentDeltaAsPercentage();

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
            // Needs will be ordered from largest to smallest average fulfillment delta.
            int IComparer.Compare(object a, object b)
            {
                SubjectiveNeed needA = (SubjectiveNeed)a;
                SubjectiveNeed needB = (SubjectiveNeed)b;

                float needDeltaA = needA.GetAverageFulfillmentDelta();
                float needDeltaB = needB.GetAverageFulfillmentDelta();

                if (needDeltaA < needDeltaB)
                    return 1;
                if (needDeltaA > needDeltaB)
                    return -1;
                else
                    return 0;
            }
        }

        class SortHelper_SubjectiveNeedsbyAverageDeltaPercentage : IComparer
        {
            // Needs will be ordered from largest to smallest percentile average fulfillment delta.
            int IComparer.Compare(object a, object b)
            {
                SubjectiveNeed needA = (SubjectiveNeed)a;
                SubjectiveNeed needB = (SubjectiveNeed)b;

                float needDeltaA = needA.GetAverageFulfillmentDeltaFromPercentages();
                float needDeltaB = needB.GetAverageFulfillmentDeltaFromPercentages();

                if (needDeltaA < needDeltaB)
                    return 1;
                if (needDeltaA > needDeltaB)
                    return -1;
                else
                    return 0;
            }
        }
    }
}
