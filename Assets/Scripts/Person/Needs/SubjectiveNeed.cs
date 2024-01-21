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
        private float localAiPriorityWeighting,
                      localAiPriorityWeightingLeft, 
                      localAiPriorityWeightingRight;

        [SerializeField]
        private float levelFullLeft,
                      levelCurrentLeft;
        [SerializeField]
        private float levelFullRight,
                      levelCurrentRight;

        [SerializeField]
        private float baseChangeRateLeft,
                      currentChangeRateLeft;
        [SerializeField]
        private float baseChangeRateRight,
                      currentChangeRateRight;

        [SerializeField]
        private float thresholdWarningLeft, thresholdWarningRight,
                      thresholdCriticalLeft, thresholdCriticalRight;

        [BeginInspectorReadOnlyGroup]
        [SerializeField]
        private bool isWarningLeft,
                     isWarningRight;
        [SerializeField]
        private bool isCriticalLeft,
                     isCriticalRight;
        [SerializeField]
        private bool isFailureLeft,
                     isFailureRight;
        //[EndInspectorReadOnlyGroup]



        // PROPERTIES
        public SubjectiveNeedSO NeedSO { get { return needSO; } }
        public string NeedName { get { return needSO.NeedName; } }
        public string NeedNameLeft { get { return needSO.NeedNameLeft; } }
        public string NeedNameRight { get { return needSO.NeedNameRight; } }
        public Sprite NeedIconLeft { get { return needSO.needIconLeft; } }
        public Sprite NeedIconRight { get { return needSO.needIconRight; } }

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
        public float LocalAiPriorityWeightingLeft
        {
            get { return localAiPriorityWeightingLeft; }
            set
            {
                if (value < Constants.MIN_PRIORITY)
                {
                    localAiPriorityWeightingLeft = Constants.MIN_PRIORITY;
                    return;
                }
                else if (value > Constants.MAX_PRIORITY)
                {
                    localAiPriorityWeightingLeft = Constants.MAX_PRIORITY;
                    return;
                }
                else localAiPriorityWeightingLeft = value;
            }
        }
        public float LocalAiPriorityWeightingRight
        {
            get { return localAiPriorityWeightingRight; }
            set
            {
                if (value < Constants.MIN_PRIORITY)
                {
                    localAiPriorityWeightingRight = Constants.MIN_PRIORITY;
                    return;
                }
                else if (value > Constants.MAX_PRIORITY)
                {
                    localAiPriorityWeightingRight = Constants.MAX_PRIORITY;
                    return;
                }
                else localAiPriorityWeightingRight = value;
            }
        }
        public void AverageLocalAiPriorityWeighting()
        {
            LocalAiPriorityWeighting = (LocalAiPriorityWeightingLeft + LocalAiPriorityWeightingRight) / 2;
        }

        public float LevelEmptyLeft { get { return needSO.LevelEmptyLeft; } }
        public float LevelEmptyRight { get { return needSO.LevelEmptyRight; } }
        public (float, float) LevelEmpty { get { return (needSO.LevelEmptyLeft, needSO.LevelEmptyRight); } }
        public float DefaultLevelFullLeft { get { return needSO.LevelFullLeft; } }
        public float DefaultLevelFullRight { get { return needSO.LevelFullRight; } }
        public (float, float) DefaultLevelFull { get { return (needSO.LevelFullLeft, needSO.LevelFullRight); } }
        public float LevelFullLeft
        {
            get { return levelFullLeft; }
            set
            {
                if (value > LevelEmptyLeft)
                {
                    levelFullLeft = value;
                }
            }
        }
        public float LevelFullRight
        {
            get { return levelFullRight; }
            set
            {
                if (value > LevelEmptyRight)
                {
                    levelFullRight = value;
                }
            }
        }
        public (float, float) LevelFull
        {
            get { return (levelFullLeft, levelFullRight); }
            set
            {
                LevelFullLeft = value.Item1;
                LevelFullRight = value.Item2;
            }
        }
        public float LevelCurrentLeft
        {
            get { return levelCurrentLeft; }
            set
            {
                if (value >= DefaultLevelFullLeft)
                {
                    levelCurrentLeft = DefaultLevelFullLeft;
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
        public float LevelCurrentLeftAsPercentage
        {
            get { return levelCurrentLeft/levelFullLeft; }
        }
        public float LevelCurrentRight
        {
            get { return levelCurrentRight; }
            set
            {
                if (value >= DefaultLevelFullRight)
                {
                    levelCurrentRight = DefaultLevelFullRight;
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
        public float LevelCurrentRightAsPercentage
        {
            get { return levelCurrentRight / levelFullRight; }
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
        public (float, float) LevelCurrentAsPercentage
        {
            get { return (levelCurrentLeft/levelFullLeft, levelCurrentRight/levelFullRight); }
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

        public float DefaultThresholdWarningLeft { get { return needSO.ThresholdWarningLeft; } }
        public float DefaultThresholdCriticalLeft { get { return needSO.ThresholdCriticalLeft; } }
        public float DefaultThresholdWarningRight { get { return needSO.ThresholdWarningRight; } }
        public float DefaultThresholdCriticalRight { get { return needSO.ThresholdCriticalRight; } }
        public float ThresholdWarningLeft
        {
            get { return thresholdWarningLeft; }
            set
            {
                if (value > 1)
                {
                    thresholdWarningLeft = 1;
                    return;
                }
                else if (value < 0)
                {
                    thresholdWarningLeft = 0;
                    return;
                }
                else thresholdWarningLeft = value;
            }
        }
        public float ThresholdCriticalLeft
        {
            get { return thresholdCriticalLeft; }
            set
            {
                if (value > 1)
                {
                    thresholdCriticalLeft = 1;
                    return;
                }
                else if (value < 0)
                {
                    thresholdCriticalLeft = 0;
                    return;
                }
                else thresholdCriticalLeft = value;
            }
        }
        public float ThresholdWarningRight
        {
            get { return thresholdWarningRight; }
            set
            {
                if (value > 1)
                {
                    thresholdWarningRight = 1;
                    return;
                }
                else if (value < 0)
                {
                    thresholdWarningRight = 0;
                    return;
                }
                else thresholdWarningRight = value;
            }
        }
        public float ThresholdCriticalRight
        {
            get { return thresholdCriticalRight; }
            set
            {
                if (value > 1)
                {
                    thresholdCriticalRight = 1;
                    return;
                }
                else if (value < 0)
                {
                    thresholdCriticalRight = 0;
                    return;
                }
                else thresholdCriticalRight = value;
            }
        }
        public void ResetThresholds()
        {
            ThresholdWarningLeft = DefaultThresholdWarningLeft;
            ThresholdWarningRight = DefaultThresholdWarningRight;
            ThresholdCriticalLeft = DefaultThresholdCriticalLeft;
            thresholdCriticalRight = DefaultThresholdCriticalRight;
        }

        public bool IsWarningLeft { get { return isWarningLeft; } private set { isWarningLeft = value; } }
        public bool IsWarningRight { get { return isWarningRight; } private set { isWarningRight = value; } }
        public bool IsWarning { get { return (IsWarningLeft || IsWarningRight); } }
        public bool IsCriticalLeft { get { return isCriticalLeft; } private set { isCriticalLeft = value; } }
        public bool IsCriticalRight { get { return isCriticalRight; } private set { isCriticalRight = value; } }
        public bool IsCritical { get { return (IsCriticalLeft || IsCriticalRight); } }
        public bool IsFailureLeft { get { return isFailureLeft; } private set { isFailureLeft = value; } }
        public bool IsFailureRight { get { return isFailureRight; } private set { isFailureRight = value; } }
        public bool IsFailure { get { return (IsFailureLeft || IsFailureRight); } }



        // EVENTS
        // The need's level has been updated.
        public delegate void SubjectiveNeedLevelCurrentUpdate(SubjectiveNeedSO needIdentity, (float, float) needLevelCurrent, (float, float) needLevelCurrentAsPercentage);
        private event SubjectiveNeedLevelCurrentUpdate OnSNLevelCurrentUpdate;

        // General update event for all values? Pass a reference to this object itself?

        // The Warning threshold is reached.
        public delegate void SubjectiveNeedReachedWarning(SubjectiveNeedSO needIdentity, (float, float) needLevelCurrent, (float, float) needLevelCurrentAsPercentage);
        private event SubjectiveNeedReachedWarning OnSNReachedWarning;

        // The Critical threshold is reached.
        public delegate void SubjectiveNeedReachedCritical(SubjectiveNeedSO needIdentity, (float, float) needLevelCurrent, (float, float) needLevelCurrentAsPercentage);
        private event SubjectiveNeedReachedCritical OnSNReachedCritical;

        // Need failure is reached.
        public delegate void SubjectiveNeedFailure(SubjectiveNeedSO needIdentity); // Is the other information needed?
        private event SubjectiveNeedFailure OnSNFailure;



        /// CONSTRUCTORS
        public SubjectiveNeed(SubjectiveNeedSO needSO)
        {
            this.needSO = needSO;
            SetParametersFromSO();
        }

        private void SetParametersFromSO()
        {
            LocalAiPriorityWeighting = AiPriorityWeighting;
            LocalAiPriorityWeightingLeft = AiPriorityWeighting;
            LocalAiPriorityWeightingRight = AiPriorityWeighting;

            LevelFull = (DefaultLevelFullLeft, DefaultLevelFullRight);
            LevelCurrent = (DefaultLevelFullLeft, DefaultLevelFullRight);

            BaseChangeRate = (DefaultChangeRateLeft, DefaultChangeRateRight);
            CurrentChangeRate = (DefaultChangeRateLeft, DefaultChangeRateRight);

            ThresholdWarningLeft = DefaultThresholdWarningLeft;
            ThresholdCriticalLeft = DefaultThresholdCriticalLeft;
            ThresholdWarningRight = DefaultThresholdWarningRight;
            ThresholdCriticalRight = DefaultThresholdCriticalRight;
        }

        // Modulate default values.
        // Should include averaging the left and right AI weighting.



        // OVERRIDES
        // ToString
        public override string ToString()
        {
            return string.Format("{0}:\n\n" +
                "{1}: {2:P2} fulfilled.\nBase decay rate: {3}.\nCurrent decay rate: {4}.\n\n" +
                "{5}: {6:P2} fulfilled.\nBase decay rate: {7}.\nCurrent decay rate: {8}.\n\n" +
                "The neediest subneed is {9}.\n" +
                "The total need fulfillment is {10:P2}, the average fulfillment is {11:P2}.\n" +
                "The raw total delta between subneeds is {12}, the raw average delta is {13}.",
                NeedName, 
                NeedNameLeft, 1 - GetLeftFulfillmentDelta(true), BaseChangeRateLeft, CurrentChangeRateLeft,
                NeedNameRight, 1 - GetRightFulfillmentDelta(true), BaseChangeRateRight, CurrentChangeRateRight,
                (GetNeediestSide() ? NeedNameRight : NeedNameLeft),
                1 - GetTotalFulfillmentDelta(true), 
                1 - GetAverageFulfillmentDelta(true),
                GetTotalFulfillmentDelta(),
                GetAverageFulfillmentDelta()
                );
        }



        // METHODS

        // GetFullfilment*
        public float GetLeftFulfillmentDelta(bool asPercentage = false) // How 'far' are we from a fully fulfilled need? The higher the value, the needier the need.
        {
            float delta = LevelFullLeft - LevelCurrentLeft;
            return asPercentage ? (delta / LevelFullLeft) : delta;
        }
        public float GetRightFulfillmentDelta(bool asPercentage = false)
        {
            float delta = LevelFullRight - LevelCurrentRight;
            return asPercentage ? (delta / LevelFullRight) : delta;
        }
        public float GetTotalFulfillmentDelta(bool byPercentage = false) // Return the sum of both subneeds' fulfillment deltas.
        {
            float delta = GetLeftFulfillmentDelta() + GetRightFulfillmentDelta();
            return byPercentage ? (delta / (LevelFullLeft + LevelFullRight)) : delta;
        }
        public float GetAverageFulfillmentDelta(bool byPercentage = false) // Return the average fullfilment delta of the two subneeds.
        {
            return (GetLeftFulfillmentDelta(byPercentage) + GetRightFulfillmentDelta(byPercentage)) / 2;
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
        public static void SortByFulfillmentDeltaofNeediest(SubjectiveNeed[] subjectiveNeeds, bool usePriorityWeights = true, bool byPercentage = false)
        {
            SortHelper_SubjectiveNeedsbyDeltaOfNeediest sortHelper = new SortHelper_SubjectiveNeedsbyDeltaOfNeediest(usePriorityWeights, byPercentage);
            Array.Sort(subjectiveNeeds, sortHelper);
            //Array.Reverse(subjectiveNeeds);
        }
        // Sorts an array of subjective needs by the difference between the total deltas of their subneeds. The most drastic difference comes first.
        public static void SortByTotalFulfillmentDelta(SubjectiveNeed[] subjectiveNeeds, bool usePriorityWeights = true, bool byPercentage = false)
        {
            SortHelper_SubjectiveNeedsbyTotalDelta sortHelper = new SortHelper_SubjectiveNeedsbyTotalDelta(usePriorityWeights, byPercentage);
            Array.Sort(subjectiveNeeds, sortHelper);
            //Array.Reverse(subjectiveNeeds);
        }
        // Sorts an array of subjective needs by the difference between the average deltas of their subneeds. The most drastic difference comes first.
        public static void SortByAverageFulfillmentDelta(SubjectiveNeed[] subjectiveNeeds, bool usePriorityWeights = true, bool byPercentage = false)
        {
            SortHelper_SubjectiveNeedsbyAverageDelta sortHelper = new SortHelper_SubjectiveNeedsbyAverageDelta(usePriorityWeights, byPercentage);
            Array.Sort(subjectiveNeeds, sortHelper);
            //Array.Reverse(subjectiveNeeds);
        }

        // Return neediest of the set.
        public static SubjectiveNeed ReturnNeediestbyNeediestDelta(SubjectiveNeed[] subjectiveNeeds, bool usePriorityWeights = true, bool byPercentage = false)
        {
            SortByFulfillmentDeltaofNeediest(subjectiveNeeds, usePriorityWeights, byPercentage);
            return subjectiveNeeds[0];
        }
        public static SubjectiveNeed ReturnNeediestbyTotalDelta(SubjectiveNeed[] subjectiveNeeds, bool usePriorityWeights = true, bool byPercentage = false)
        {
            SortByTotalFulfillmentDelta(subjectiveNeeds, usePriorityWeights, byPercentage);
            return subjectiveNeeds[0];
        }
        public static SubjectiveNeed ReturnNeediestbyAverageDelta(SubjectiveNeed[] subjectiveNeeds, bool usePriorityWeights = true, bool byPercentage = false)
        {
            SortByAverageFulfillmentDelta(subjectiveNeeds, usePriorityWeights, byPercentage);
            return subjectiveNeeds[0];
        }

        // COMPARISON HELPERS
        class SortHelper_SubjectiveNeedsbyDeltaOfNeediest : IComparer
        {
            bool usePriorityWeights;
            bool byPercentage;
            public SortHelper_SubjectiveNeedsbyDeltaOfNeediest(bool usePriorityWeights, bool byPercentage)
            {
                this.usePriorityWeights = usePriorityWeights;
                this.byPercentage = byPercentage;
            }

            // Needs will be ordered from largest to smallest fulfillment delta of their most deficient subneed.
            int IComparer.Compare(object a, object b)
            {
                SubjectiveNeed needA = (SubjectiveNeed)a;
                SubjectiveNeed needB = (SubjectiveNeed)b;

                bool isNeediestSideRightA = needA.GetNeediestSide(byPercentage);
                bool isNeediestSideRightB = needB.GetNeediestSide(byPercentage);

                float needDeltaA = isNeediestSideRightA ? needA.GetRightFulfillmentDelta(byPercentage) : needA.GetLeftFulfillmentDelta(byPercentage);
                float needDeltaB = isNeediestSideRightB ? needB.GetRightFulfillmentDelta(byPercentage) : needB.GetLeftFulfillmentDelta(byPercentage);

                if (usePriorityWeights)
                {
                    needDeltaA *= isNeediestSideRightA ? needA.localAiPriorityWeightingRight : needA.localAiPriorityWeightingLeft;
                    needDeltaB *= isNeediestSideRightB ? needB.localAiPriorityWeightingRight : needB.localAiPriorityWeightingLeft;
                }

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
            bool usePriorityWeights;
            bool byPercentage;
            public SortHelper_SubjectiveNeedsbyTotalDelta(bool usePriorityWeights, bool byPercentage)
            {
                this.usePriorityWeights = usePriorityWeights;
                this.byPercentage = byPercentage;
            }

            // Needs will be ordered from largest to smallest total fulfillment delta.
            int IComparer.Compare(object a, object b)
            {
                SubjectiveNeed needA = (SubjectiveNeed)a;
                SubjectiveNeed needB = (SubjectiveNeed)b;

                float needDeltaA = needA.GetTotalFulfillmentDelta(byPercentage);
                float needDeltaB = needB.GetTotalFulfillmentDelta(byPercentage);

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
        class SortHelper_SubjectiveNeedsbyAverageDelta : IComparer
        {
            bool usePriorityWeights;
            bool byPercentage;
            public SortHelper_SubjectiveNeedsbyAverageDelta(bool usePriorityWeights, bool byPercentage)
            {
                this.usePriorityWeights = usePriorityWeights;
                this.byPercentage = byPercentage;
            }

            // Needs will be ordered from largest to smallest average fulfillment delta.
            int IComparer.Compare(object a, object b)
            {
                SubjectiveNeed needA = (SubjectiveNeed)a;
                SubjectiveNeed needB = (SubjectiveNeed)b;

                float needDeltaA = needA.GetAverageFulfillmentDelta(byPercentage);
                float needDeltaB = needB.GetAverageFulfillmentDelta(byPercentage);

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



        // COROUTINES
        // Every second, alter this need's fulfillment level by its current change rate.
        public IEnumerator AlterLevelByChangeRate()
        {
            while (true)
            {
                // Handle LeftSide subneed.
                if (this.LevelCurrentLeft > this.LevelEmptyLeft) // The need is not empty.
                {
                    this.LevelCurrentLeft += this.CurrentChangeRateLeft;
                    // Invoke need change event?

                    // Threshold detection.
                    if (!this.IsCriticalLeft & this.LevelCurrentLeftAsPercentage <= this.ThresholdCriticalLeft)
                    {
                        this.IsCriticalLeft = true;
                        Debug.Log(string.Format("{0} is critically low ({1:P2})...", this.NeedNameLeft, 1 - GetLeftFulfillmentDelta(true)));

                        OnSNReachedCritical?.Invoke(NeedSO, LevelCurrent, LevelCurrentAsPercentage);
                    }
                    else if (!this.IsWarningLeft & this.LevelCurrentLeftAsPercentage <= this.ThresholdWarningLeft)
                    {
                        this.IsWarningLeft = true;
                        Debug.Log(string.Format("{0} is a little low ({1:P2})...", this.NeedNameLeft, 1 - GetLeftFulfillmentDelta(true)));

                        OnSNReachedWarning?.Invoke(NeedSO, LevelCurrent, LevelCurrentAsPercentage);
                    }
                    else // Unset the Warning and Critical booleans as needed.
                    {
                        if (this.IsCriticalLeft & this.LevelCurrentLeft > this.ThresholdCriticalLeft)
                        {
                            this.IsCriticalLeft = false;
                            Debug.Log(string.Format("{0} is no longer critically low.", this.NeedNameLeft));
                        }
                        if (this.IsWarningLeft & this.LevelCurrentLeft > this.ThresholdWarningLeft)
                        {
                            this.IsWarningLeft = false;
                            Debug.Log(string.Format("{0} is no longer low.", this.NeedNameLeft));
                        }
                    }
                }
                else // The need is currently empty.
                {
                    if (!this.IsFailureLeft) // First detection of the need failure.
                    {
                        this.IsFailureLeft = true;
                        Debug.Log(string.Format("{0} is now empty.", this.NeedNameLeft));

                        OnSNFailure?.Invoke(NeedSO);
                    }

                    if (this.CurrentChangeRateLeft > 0) // Only apply the change if it would increase it.
                    {
                        this.LevelCurrentLeft += this.CurrentChangeRateLeft;
                        // Invoke need change event?

                        this.IsFailureLeft = false; // Undo the need failure.
                        Debug.Log(string.Format("{0} is no longer in need failure.", this.NeedNameLeft));
                    }
                }

                // Handle RightSide subneed.
                if (this.LevelCurrentRight > this.LevelEmptyRight) // The need is not empty.
                {
                    this.LevelCurrentRight += this.CurrentChangeRateRight;
                    // Invoke need change event?

                    // Threshold detection.
                    if (!this.IsCriticalRight & this.LevelCurrentRightAsPercentage <= this.ThresholdCriticalRight)
                    {
                        this.IsCriticalRight = true;
                        Debug.Log(string.Format("{0} is critically low ({1:P2})...", this.NeedNameRight, 1 - GetRightFulfillmentDelta(true)));

                        OnSNReachedCritical?.Invoke(NeedSO, LevelCurrent, LevelCurrentAsPercentage);
                    }
                    else if (!this.IsWarningRight & this.LevelCurrentRightAsPercentage <= this.ThresholdWarningRight)
                    {
                        this.IsWarningRight = true;
                        Debug.Log(string.Format("{0} is a little low ({1:P2})...", this.NeedNameRight, 1 - GetRightFulfillmentDelta(true)));

                        OnSNReachedWarning?.Invoke(NeedSO, LevelCurrent, LevelCurrentAsPercentage);
                    }
                    else // Unset the Warning and Critical booleans as needed.
                    {
                        if (this.IsCriticalRight & this.LevelCurrentRight > this.ThresholdCriticalRight)
                        {
                            this.IsCriticalRight = false;
                            Debug.Log(string.Format("{0} is no longer critically low.", this.NeedNameRight));
                        }
                        if (this.IsWarningRight & this.LevelCurrentRight > this.ThresholdWarningRight)
                        {
                            this.IsWarningRight = false;
                            Debug.Log(string.Format("{0} is no longer low.", this.NeedNameRight));
                        }
                    }
                }
                else // The need is currently empty.
                {
                    if (!this.IsFailureRight) // First detection of the need failure.
                    {
                        this.IsFailureRight = true;
                        Debug.Log(string.Format("{0} is now empty.", this.NeedNameRight));

                        OnSNFailure?.Invoke(NeedSO);
                    }

                    if (this.CurrentChangeRateRight > 0) // Only apply the change if it would increase it.
                    {
                        this.LevelCurrentRight += this.CurrentChangeRateRight;
                        // Invoke need change event?

                        this.IsFailureRight = false; // Undo the need failure.
                        Debug.Log(string.Format("{0} is no longer in need failure.", this.NeedNameRight));
                    }
                }

                yield return new WaitForSeconds(1);
            }
        }
    }
}
