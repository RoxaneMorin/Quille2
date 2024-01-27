using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    [System.Serializable]
    public class SubjectiveNeed
    {
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

        [SerializeField, InspectorReadOnly]
        private float currentChangeRateLeftScaled,
                      currentChangeRateRightScaled;

        [SerializeField]
        private float thresholdWarningLeft, thresholdWarningRight,
                      thresholdCriticalLeft, thresholdCriticalRight;

        [SerializeField, InspectorReadOnly]
        private NeedStates needStateLeft = NeedStates.Normal;
        [SerializeField, InspectorReadOnly]
        private NeedStates needStateRight = NeedStates.Normal;



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
            set
            {
                currentChangeRateLeft = value;
                currentChangeRateLeftScaled = currentChangeRateLeft / Constants.NEED_CHANGE_RATE_DIVIDER;
            }
        }
        public float CurrentChangeRateRight
        {
            get { return currentChangeRateRight; }
            set
            {
                currentChangeRateRight = value;
                currentChangeRateRightScaled = currentChangeRateRight / Constants.NEED_CHANGE_RATE_DIVIDER;
            }
        }
        public (float, float) CurrentChangeRate
        {
            get { return (currentChangeRateLeft, currentChangeRateRight); }
            set
            {
                currentChangeRateLeft = value.Item1;
                currentChangeRateLeftScaled = currentChangeRateLeft / Constants.NEED_CHANGE_RATE_DIVIDER;

                currentChangeRateRight = value.Item2;
                currentChangeRateRightScaled = currentChangeRateRight / Constants.NEED_CHANGE_RATE_DIVIDER;
            }
        }

        public float CurrentChangeRateLeftScaled
        {
            get { return currentChangeRateLeftScaled; }
        }
        public float CurrentChangeRateRightScaled
        {
            get { return currentChangeRateRightScaled; }
        }
        public (float, float) CurrentChangeRateScaled
        {
            get { return (currentChangeRateLeftScaled, currentChangeRateLeftScaled); }
        }

        public void ResetCurrentChangeRateLeft()
        {
            CurrentChangeRateLeft = baseChangeRateLeft;
        }
        public void ResetCurrentChangeRateRight()
        {
            CurrentChangeRateRight = baseChangeRateRight;
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
                if (value > Constants.MAX_THRESHOLD)
                {
                    thresholdWarningLeft = Constants.MAX_THRESHOLD;
                    return;
                }
                else if (value < Constants.MIN_THRESHOLD + 0.05f)
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
                if (value > Constants.MAX_THRESHOLD - 0.05f)
                {
                    thresholdCriticalLeft = Constants.MAX_THRESHOLD - 0.05f;
                    return;
                }
                else if (value < Constants.MIN_THRESHOLD)
                {
                    thresholdCriticalLeft = Constants.MIN_THRESHOLD;
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
                if (value > Constants.MAX_THRESHOLD)
                {
                    thresholdWarningRight = Constants.MAX_THRESHOLD;
                    return;
                }
                else if (value < Constants.MIN_THRESHOLD + 0.05f)
                {
                    thresholdWarningRight = Constants.MIN_THRESHOLD + 0.05f;
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
                if (value > Constants.MAX_THRESHOLD - 0.05f)
                {
                    thresholdCriticalRight = Constants.MAX_THRESHOLD - 0.05f;
                    return;
                }
                else if (value < Constants.MIN_THRESHOLD)
                {
                    thresholdCriticalRight = Constants.MIN_THRESHOLD;
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

        public NeedStates NeedStateLeft { get { return needStateLeft; } private set { needStateLeft = value; } }
        public NeedStates NeedStateRight { get { return needStateRight; } private set { needStateRight = value; } }
        public (NeedStates, NeedStates) NeedState { get { return (needStateLeft, needStateRight); } }



        // EVENTS
        public event SubjectiveNeedLevelCurrentUpdate OnSNLevelCurrentUpdate;

        // General update event for all values? Pass a reference to this object itself?

        // The Warning or Critical threshold is reached.
        public event SubjectiveNeedReachedThreshold ONSNReachedThreshold;

        // The need is no longer in Failure, Critical or Warning.
        public event SubjectiveNeedLeftThreshold OnSNLeftThreshold;

        // Need failure is reached.
        public event SubjectiveNeedFailure OnSNFailure;



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

        // Return neediest of the set. The returned bool indicates which side is neediest, where Left = 0, Right = 1.
        public static (SubjectiveNeed, bool) ReturnNeediestbyNeediestDelta(SubjectiveNeed[] subjectiveNeeds, bool usePriorityWeights = true, bool byPercentage = false)
        {
            SortByFulfillmentDeltaofNeediest(subjectiveNeeds, usePriorityWeights, byPercentage);
            return (subjectiveNeeds[0], subjectiveNeeds[0].GetNeediestSide());
        }
        public static (SubjectiveNeed, bool) ReturnNeediestbyTotalDelta(SubjectiveNeed[] subjectiveNeeds, bool usePriorityWeights = true, bool byPercentage = false)
        {
            SortByTotalFulfillmentDelta(subjectiveNeeds, usePriorityWeights, byPercentage);
            return (subjectiveNeeds[0], subjectiveNeeds[0].GetNeediestSide());
        }
        public static (SubjectiveNeed, bool) ReturnNeediestbyAverageDelta(SubjectiveNeed[] subjectiveNeeds, bool usePriorityWeights = true, bool byPercentage = false)
        {
            SortByAverageFulfillmentDelta(subjectiveNeeds, usePriorityWeights, byPercentage);
            return (subjectiveNeeds[0], subjectiveNeeds[0].GetNeediestSide());
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



        // Init.
        public void Init(BasePerson sourceBasePerson)
        {
            // Run the modulators.

            // AI weighting.
            foreach (ChecksAndMods.ModulatorArithmeticFromFloat modulator in needSO.BaseAIWeightingModulatedByLeft)
            {
                LocalAiPriorityWeightingLeft = modulator.Execute(sourceBasePerson, LocalAiPriorityWeightingLeft);
            }
            foreach (ChecksAndMods.ModulatorArithmeticFromFloat modulator in needSO.BaseAIWeightingModulatedByRight)
            {
                LocalAiPriorityWeightingRight = modulator.Execute(sourceBasePerson, LocalAiPriorityWeightingRight);
            }

            // Base change rates.
            // TO DO: disallow static base change rates?
            foreach (ChecksAndMods.ModulatorArithmeticFromFloat modulator in needSO.BaseChangeRateModulatedByLeft)
            {
                BaseChangeRateLeft = modulator.Execute(sourceBasePerson, BaseChangeRateLeft);
            }
            foreach (ChecksAndMods.ModulatorArithmeticFromFloat modulator in needSO.BaseChangeRateModulatedByRight)
            {
                BaseChangeRateRight = modulator.Execute(sourceBasePerson, BaseChangeRateRight);
            }

            CurrentChangeRate = (BaseChangeRateLeft, BaseChangeRateRight);

            // Thresholds.
            foreach (ChecksAndMods.ModulatorArithmeticFromFloat modulator in needSO.ThresholdsModulatedByLeft)
            {
                ThresholdWarningLeft = modulator.Execute(sourceBasePerson, ThresholdWarningLeft);
                ThresholdCriticalLeft = modulator.Execute(sourceBasePerson, ThresholdCriticalLeft);
            }
            foreach (ChecksAndMods.ModulatorArithmeticFromFloat modulator in needSO.ThresholdsModulatedByRight)
            {
                ThresholdWarningRight = modulator.Execute(sourceBasePerson, ThresholdWarningRight);
                ThresholdCriticalRight = modulator.Execute(sourceBasePerson, ThresholdWarningRight);
            }
        }



        // COROUTINES
        // Every second, alter this need's fulfillment level by its current change rate.
        public IEnumerator AlterLevelByChangeRate()
        {
            // Change rate division stuff.

            while (true)
            {
                // Handle LeftSide subneed.
                if (this.LevelCurrentLeft > this.LevelEmptyLeft) // The need is not empty.
                {
                    this.LevelCurrentLeft += this.CurrentChangeRateLeftScaled;
                    // Invoke need change event?

                    // Threshold detection.
                    if (this.NeedStateLeft > NeedStates.Critical & this.LevelCurrentLeftAsPercentage <= this.ThresholdCriticalLeft)
                    {
                        this.NeedStateLeft = NeedStates.Critical;
                        Debug.Log(string.Format("{0} is critically low ({1:P2})...", this.NeedNameLeft, 1 - GetLeftFulfillmentDelta(true)));

                        ONSNReachedThreshold?.Invoke(NeedSO, false, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Critical);
                    }
                    else if (this.NeedStateLeft > NeedStates.Warning & this.LevelCurrentLeftAsPercentage <= this.ThresholdWarningLeft)
                    {
                        this.NeedStateLeft = NeedStates.Warning;
                        Debug.Log(string.Format("{0} is a little low ({1:P2})...", this.NeedNameLeft, 1 - GetLeftFulfillmentDelta(true)));

                        ONSNReachedThreshold?.Invoke(NeedSO, false, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Warning);
                    }
                    else // Unset the Warning and Critical booleans as needed.
                    {
                        if (this.NeedStateLeft == NeedStates.Critical & this.LevelCurrentLeft > this.ThresholdCriticalLeft)
                        {
                            this.NeedStateLeft = NeedStates.Warning;
                            Debug.Log(string.Format("{0} is no longer critically low.", this.NeedNameLeft));

                            OnSNLeftThreshold?.Invoke(NeedSO, false, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Critical);
                        }
                        if (this.NeedStateLeft == NeedStates.Warning & this.LevelCurrentLeft > this.ThresholdWarningLeft)
                        {
                            this.NeedStateLeft = NeedStates.Normal;
                            Debug.Log(string.Format("{0} is no longer low.", this.NeedNameLeft));

                            OnSNLeftThreshold?.Invoke(NeedSO, false, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Warning);
                        }
                    }
                }
                else // The need is currently empty.
                {
                    if (this.NeedStateLeft != NeedStates.Failure) // First detection of the need failure.
                    {
                        this.NeedStateLeft = NeedStates.Failure;
                        Debug.Log(string.Format("{0} is now empty.", this.NeedNameLeft));

                        OnSNFailure?.Invoke(NeedSO, false);
                    }

                    if (this.CurrentChangeRateLeftScaled > 0) // Only apply the change if it would increase it.
                    {
                        this.LevelCurrentLeft += this.CurrentChangeRateLeftScaled;

                        this.NeedStateLeft = NeedStates.Critical; // Undo the need failure.
                        Debug.Log(string.Format("{0} is no longer in need failure.", this.NeedNameLeft));

                        OnSNLeftThreshold?.Invoke(NeedSO, false, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Failure);
                    }
                }

                // Handle RightSide subneed.
                if (this.LevelCurrentRight > this.LevelEmptyRight) // The need is not empty.
                {
                    this.LevelCurrentRight += this.CurrentChangeRateRightScaled;
                    // Invoke need change event?

                    // Threshold detection.
                    if (this.NeedStateRight > NeedStates.Critical & this.LevelCurrentRightAsPercentage <= this.ThresholdCriticalRight)
                    {
                        this.NeedStateRight = NeedStates.Critical;
                        Debug.Log(string.Format("{0} is critically low ({1:P2})...", this.NeedNameRight, 1 - GetRightFulfillmentDelta(true)));

                        ONSNReachedThreshold?.Invoke(NeedSO, true, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Critical);
                    }
                    else if (this.NeedStateRight > NeedStates.Warning & this.LevelCurrentRightAsPercentage <= this.ThresholdWarningRight)
                    {
                        this.NeedStateRight = NeedStates.Warning;
                        Debug.Log(string.Format("{0} is a little low ({1:P2})...", this.NeedNameRight, 1 - GetRightFulfillmentDelta(true)));

                        ONSNReachedThreshold?.Invoke(NeedSO, true, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Warning);
                    }
                    else // Unset the Warning and Critical booleans as needed.
                    {
                        if (this.NeedStateRight == NeedStates.Critical & this.LevelCurrentRight > this.ThresholdCriticalRight)
                        {
                            this.NeedStateRight = NeedStates.Warning;
                            Debug.Log(string.Format("{0} is no longer critically low.", this.NeedNameRight));

                            OnSNLeftThreshold?.Invoke(NeedSO, true, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Critical);
                        }
                        if (this.NeedStateRight == NeedStates.Warning & this.LevelCurrentRight > this.ThresholdWarningRight)
                        {
                            this.NeedStateRight = NeedStates.Normal;
                            Debug.Log(string.Format("{0} is no longer low.", this.NeedNameRight));

                            OnSNLeftThreshold?.Invoke(NeedSO, true, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Warning);
                        }
                    }
                }
                else // The need is currently empty.
                {
                    if (this.NeedStateRight != NeedStates.Failure) // First detection of the need failure.
                    {
                        this.NeedStateRight = NeedStates.Failure;
                        Debug.Log(string.Format("{0} is now empty.", this.NeedNameRight));

                        OnSNFailure?.Invoke(NeedSO, true);
                    }

                    if (this.CurrentChangeRateRightScaled > 0) // Only apply the change if it would increase it.
                    {
                        this.LevelCurrentRight += this.CurrentChangeRateRightScaled;

                        this.NeedStateRight = NeedStates.Critical; // Undo the need failure.
                        Debug.Log(string.Format("{0} is no longer in need failure.", this.NeedNameRight));

                        OnSNLeftThreshold?.Invoke(NeedSO, true, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Failure);
                    }
                }

                yield return new WaitForSeconds(1);
            }
        }
    }
}
