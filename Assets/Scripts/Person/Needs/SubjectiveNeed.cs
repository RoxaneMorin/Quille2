using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Quille
{
    // The C# object component of a SubjectiveNeed. Instances derive their specificities from their associated SubjectiveNeedSO.
    // These needs represent higher level psychological drives such as the desires for social interactions, comfort or novelty in one's life.
    // Two (somewhat) opposing drives are contained within the same SubjectiveNeed.
    // JSON serializable, though instance serialization is handled by the NeedController containing them.


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

        [SerializeField] //, InspectorReadOnly]
        private float currentChangeRateLeftScaled,
                      currentChangeRateRightScaled;

        [SerializeField]
        private float thresholdElatedLeft, thresholdElatedRight,
                      thresholdWarningLeft, thresholdWarningRight,
                      thresholdCriticalLeft, thresholdCriticalRight;

        [SerializeField, InspectorReadOnly]
        private NeedStates needStateLeft = NeedStates.Normal;
        [SerializeField, InspectorReadOnly]
        private NeedStates needStateRight = NeedStates.Normal;

        [InspectorReadOnly, JsonIgnore]
        public string needNameForUI;
        [InspectorReadOnly, JsonIgnore]
        public string needNameLeftForUI;
        [InspectorReadOnly, JsonIgnore]
        public string needNameRightForUI;




        // PROPERTIES
        [JsonIgnore] public SubjectiveNeedSO NeedSO { get { return needSO; } }
        [JsonIgnore] public BasicNeedSO NeedSOLeft { get { return needSO.NeedSORight; } }
        [JsonIgnore] public BasicNeedSO NeedSORight { get { return needSO.NeedSORight; } }

        [JsonIgnore] public string NeedName { get { return needSO.NeedName; } }
        [JsonIgnore] public string NeedNameLeft { get { return needSO.NeedNameLeft; } }
        [JsonIgnore] public string NeedNameRight { get { return needSO.NeedNameRight; } }
        [JsonIgnore] public Sprite NeedIconLeft { get { return needSO.NeedIconLeft; } }
        [JsonIgnore] public Sprite NeedIconRight { get { return needSO.NeedIconRight; } }

        [JsonIgnore] public float AiPriorityWeightingLeft { get { return needSO.AIPriorityWeightingLeft; } }
        [JsonIgnore] public float AiPriorityWeightingRight { get { return needSO.AIPriorityWeightingRight; } }
        [JsonIgnore] public float LocalAiPriorityWeighting
        {
            get { return localAiPriorityWeighting; }
            set
            {
                if (value < Constants_Quille.MIN_PRIORITY)
                {
                    localAiPriorityWeighting = Constants_Quille.MIN_PRIORITY;
                    return;
                }
                else if (value > Constants_Quille.MAX_PRIORITY)
                {
                    localAiPriorityWeighting = Constants_Quille.MAX_PRIORITY;
                    return;
                }
                else localAiPriorityWeighting = value;
            }
        }
        [JsonIgnore] public float LocalAiPriorityWeightingLeft
        {
            get { return localAiPriorityWeightingLeft; }
            set
            {
                if (value < Constants_Quille.MIN_PRIORITY)
                {
                    localAiPriorityWeightingLeft = Constants_Quille.MIN_PRIORITY;
                    return;
                }
                else if (value > Constants_Quille.MAX_PRIORITY)
                {
                    localAiPriorityWeightingLeft = Constants_Quille.MAX_PRIORITY;
                    return;
                }
                else localAiPriorityWeightingLeft = value;
            }
        }
        [JsonIgnore] public float LocalAiPriorityWeightingRight
        {
            get { return localAiPriorityWeightingRight; }
            set
            {
                if (value < Constants_Quille.MIN_PRIORITY)
                {
                    localAiPriorityWeightingRight = Constants_Quille.MIN_PRIORITY;
                    return;
                }
                else if (value > Constants_Quille.MAX_PRIORITY)
                {
                    localAiPriorityWeightingRight = Constants_Quille.MAX_PRIORITY;
                    return;
                }
                else localAiPriorityWeightingRight = value;
            }
        }
        public void AverageLocalAiPriorityWeighting()
        {
            LocalAiPriorityWeighting = (LocalAiPriorityWeightingLeft + LocalAiPriorityWeightingRight) / 2;
        }

        [JsonIgnore] public float LevelEmptyLeft { get { return needSO.LevelEmptyLeft; } }
        [JsonIgnore] public float LevelEmptyRight { get { return needSO.LevelEmptyRight; } }
        [JsonIgnore] public (float, float) LevelEmpty { get { return (needSO.LevelEmptyLeft, needSO.LevelEmptyRight); } }
        [JsonIgnore] public float DefaultLevelFullLeft { get { return needSO.LevelFullLeft; } }
        [JsonIgnore] public float DefaultLevelFullRight { get { return needSO.LevelFullRight; } }
        [JsonIgnore] public (float, float) DefaultLevelFull { get { return (needSO.LevelFullLeft, needSO.LevelFullRight); } }
        [JsonIgnore] public float LevelFullLeft
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
        [JsonIgnore] public float LevelFullRight
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
        [JsonIgnore] public (float, float) LevelFull
        {
            get { return (levelFullLeft, levelFullRight); }
            set
            {
                levelFullLeft = value.Item1;
                levelFullRight = value.Item2;
            }
        }
        public float LevelFullFor(BasicNeedSO subNeed)
        {
            if (subNeed == NeedSORight)
            {
                return LevelFullRight;
            }
            else
            {
                return LevelFullLeft;
            }
        }

        [JsonIgnore] public float LevelCurrentLeft
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
        [JsonIgnore] public float LevelCurrentLeftAsPercentage
        {
            get { return levelCurrentLeft/levelFullLeft; }
        }
        [JsonIgnore] public float LevelCurrentRight
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
        [JsonIgnore] public float LevelCurrentRightAsPercentage
        {
            get { return levelCurrentRight / levelFullRight; }
        }
        [JsonIgnore] public (float, float) LevelCurrent
        {
            get { return (levelCurrentLeft, levelCurrentRight); }
            set
            {
                LevelCurrentLeft = value.Item1;
                LevelCurrentRight = value.Item2;
            }
        }
        [JsonIgnore] public (float, float) LevelCurrentAsPercentage
        {
            get { return (levelCurrentLeft/levelFullLeft, levelCurrentRight/levelFullRight); }
        }
        public float LevelCurrentFor(BasicNeedSO subNeed)
        {
            if (subNeed == NeedSORight)
            {
                return LevelCurrentRight;
            }
            else
            {
                return levelCurrentLeft;
            }
        }
        public float LevelCurrentAsPercentageFor(BasicNeedSO subNeed)
        {
            if (subNeed == NeedSORight)
            {
                return LevelCurrentRight/ levelFullRight;
            }
            else
            {
                return levelCurrentLeft/ levelFullLeft;
            }
        }

        [JsonIgnore] public float DefaultChangeRateLeft
        {
            get { return needSO.DefaultChangeRateLeft; }
        }
        [JsonIgnore] public float DefaultChangeRateRight
        {
            get { return needSO.DefaultChangeRateRight; }
        }
        [JsonIgnore] public (float, float) DefaultChangeRate
        {
            get { return (needSO.DefaultChangeRateLeft, needSO.DefaultChangeRateRight); }
        }
        public float DefaultChangeRateFor(BasicNeedSO subNeed)
        {
            if (subNeed == NeedSORight)
            {
                return DefaultChangeRateRight;
            }
            else
            {
                return DefaultChangeRateLeft;
            }
        }

        [JsonIgnore] public float BaseChangeRateLeft
        {
            get { return baseChangeRateLeft; }
            set { baseChangeRateLeft = value; }
        }
        [JsonIgnore] public float BaseChangeRateRight
        {
            get { return baseChangeRateRight; }
            set { baseChangeRateRight = value; }
        }
        [JsonIgnore] public (float, float) BaseChangeRate
        {
            get { return (baseChangeRateLeft, baseChangeRateRight); }
            set
            {
                BaseChangeRateLeft = value.Item1;
                BaseChangeRateRight = value.Item2;
            }
        }
        public float BaseChangeRateFor(BasicNeedSO subNeed)
        {
            if (subNeed == NeedSORight)
            {
                return BaseChangeRateRight;
            }
            else
            {
                return BaseChangeRateLeft;
            }
        }

        public void ResetBaseChangeRateLeft()
        {
            BaseChangeRateLeft = NeedSO.DefaultChangeRateLeft;
        }
        public void ResetBaseChangeRateRight()
        {
            BaseChangeRateRight = NeedSO.DefaultChangeRateRight;
        }
        public void ResetBaseChangeRate()
        {
            ResetBaseChangeRateLeft();
            ResetBaseChangeRateRight();
        }

        [JsonIgnore] public float CurrentChangeRateLeft
        {
            get { return currentChangeRateLeft; }
            set
            {
                currentChangeRateLeft = value;
                currentChangeRateLeftScaled = currentChangeRateLeft / Constants_Quille.NEED_CHANGE_RATE_DIVIDER;
            }
        }
        [JsonIgnore] public float CurrentChangeRateRight
        {
            get { return currentChangeRateRight; }
            set
            {
                currentChangeRateRight = value;
                currentChangeRateRightScaled = currentChangeRateRight / Constants_Quille.NEED_CHANGE_RATE_DIVIDER;
            }
        }
        [JsonIgnore] public (float, float) CurrentChangeRate
        {
            get { return (currentChangeRateLeft, currentChangeRateRight); }
            set
            {
                currentChangeRateLeft = value.Item1;
                currentChangeRateLeftScaled = currentChangeRateLeft / Constants_Quille.NEED_CHANGE_RATE_DIVIDER;

                currentChangeRateRight = value.Item2;
                currentChangeRateRightScaled = currentChangeRateRight / Constants_Quille.NEED_CHANGE_RATE_DIVIDER;
            }
        }
        public float CurrentChangeRateFor(BasicNeedSO subNeed)
        {
            if (subNeed == NeedSORight)
            {
                return CurrentChangeRateRight;
            }
            else
            {
                return CurrentChangeRateLeft;
            }
        }

        [JsonIgnore] public float CurrentChangeRateLeftScaled
        {
            get { return currentChangeRateLeftScaled; }
        }
        [JsonIgnore] public float CurrentChangeRateRightScaled
        {
            get { return currentChangeRateRightScaled; }
        }
        [JsonIgnore] public (float, float) CurrentChangeRateScaled
        {
            get { return (currentChangeRateLeftScaled, currentChangeRateLeftScaled); }
        }
        public float CurrentChangeRateScaledFor(BasicNeedSO subNeed)
        {
            if (subNeed == NeedSORight)
            {
                return CurrentChangeRateRightScaled;
            }
            else
            {
                return CurrentChangeRateLeftScaled;
            }
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

        [JsonIgnore] public float DefaultThresholdElatedLeft { get { return needSO.ThresholdElatedLeft; } }
        [JsonIgnore] public float DefaultThresholdWarningLeft { get { return needSO.ThresholdWarningLeft; } }
        [JsonIgnore] public float DefaultThresholdCriticalLeft { get { return needSO.ThresholdCriticalLeft; } }
        [JsonIgnore] public float DefaultThresholdElatedRight { get { return needSO.ThresholdElatedRight; } }
        [JsonIgnore] public float DefaultThresholdWarningRight { get { return needSO.ThresholdWarningRight; } }
        [JsonIgnore] public float DefaultThresholdCriticalRight { get { return needSO.ThresholdCriticalRight; } }

        [JsonIgnore] public float ThresholdElatedLeft
        {
            get { return thresholdElatedLeft; }
            set
            {
                if (value > Constants_Quille.DEFAULT_LEVEL_FULL)
                {
                    thresholdElatedLeft = Constants_Quille.DEFAULT_LEVEL_FULL;
                    return;
                }
                else if (value < Constants_Quille.MAX_THRESHOLD_NEGATIVE + 0.05f)
                {
                    thresholdElatedLeft = Constants_Quille.MAX_THRESHOLD_NEGATIVE + 0.05f;
                    return;
                }
                else thresholdElatedLeft = value;
            }
        }
        [JsonIgnore] public float ThresholdWarningLeft
        {
            get { return thresholdWarningLeft; }
            set
            {
                if (value > Constants_Quille.MAX_THRESHOLD_NEGATIVE)
                {
                    thresholdWarningLeft = Constants_Quille.MAX_THRESHOLD_NEGATIVE;
                    return;
                }
                else if (value < Constants_Quille.MIN_THRESHOLD_NEGATIVE + 0.05f)
                {
                    thresholdWarningLeft = 0;
                    return;
                }
                else thresholdWarningLeft = value;
            }
        }
        [JsonIgnore] public float ThresholdCriticalLeft
        {
            get { return thresholdCriticalLeft; }
            set
            {
                if (value > Constants_Quille.MAX_THRESHOLD_NEGATIVE - 0.05f)
                {
                    thresholdCriticalLeft = Constants_Quille.MAX_THRESHOLD_NEGATIVE - 0.05f;
                    return;
                }
                else if (value < Constants_Quille.MIN_THRESHOLD_NEGATIVE)
                {
                    thresholdCriticalLeft = Constants_Quille.MIN_THRESHOLD_NEGATIVE;
                    return;
                }
                else thresholdCriticalLeft = value;
            }
        }
        [JsonIgnore] public float ThresholdElatedRight
        {
            get { return thresholdElatedRight; }
            set
            {
                if (value > Constants_Quille.DEFAULT_LEVEL_FULL)
                {
                    thresholdElatedRight = Constants_Quille.DEFAULT_LEVEL_FULL;
                    return;
                }
                else if (value < Constants_Quille.MAX_THRESHOLD_NEGATIVE + 0.05f)
                {
                    thresholdElatedRight = Constants_Quille.MAX_THRESHOLD_NEGATIVE + 0.05f;
                    return;
                }
                else thresholdElatedRight = value;
            }
        }
        [JsonIgnore] public float ThresholdWarningRight
        {
            get { return thresholdWarningRight; }
            set
            {
                if (value > Constants_Quille.MAX_THRESHOLD_NEGATIVE)
                {
                    thresholdWarningRight = Constants_Quille.MAX_THRESHOLD_NEGATIVE;
                    return;
                }
                else if (value < Constants_Quille.MIN_THRESHOLD_NEGATIVE + 0.05f)
                {
                    thresholdWarningRight = Constants_Quille.MIN_THRESHOLD_NEGATIVE + 0.05f;
                    return;
                }
                else thresholdWarningRight = value;
            }
        }
        [JsonIgnore] public float ThresholdCriticalRight
        {
            get { return thresholdCriticalRight; }
            set
            {
                if (value > Constants_Quille.MAX_THRESHOLD_NEGATIVE - 0.05f)
                {
                    thresholdCriticalRight = Constants_Quille.MAX_THRESHOLD_NEGATIVE - 0.05f;
                    return;
                }
                else if (value < Constants_Quille.MIN_THRESHOLD_NEGATIVE)
                {
                    thresholdCriticalRight = Constants_Quille.MIN_THRESHOLD_NEGATIVE;
                    return;
                }
                else thresholdCriticalRight = value;
            }
        }
        [JsonIgnore] public (float, float) ThresholdElated
        {
            get { return (ThresholdElatedLeft, ThresholdElatedRight); }
            set
            {
                ThresholdElatedLeft = value.Item1;
                ThresholdElatedRight = value.Item2;
            }
        }
        [JsonIgnore] public (float, float) ThresholdWarning
        {
            get { return (ThresholdWarningLeft, ThresholdWarningRight); }
            set
            {
                ThresholdWarningLeft = value.Item1;
                ThresholdWarningRight = value.Item2;
            }
        }
        [JsonIgnore] public (float, float) ThresholdCritical
        {
            get { return (ThresholdCriticalLeft, ThresholdCriticalRight); }
            set
            {
                ThresholdCriticalLeft = value.Item1;
                ThresholdCriticalRight = value.Item2;
            }
        }
        public void ResetThresholds()
        {
            ThresholdElatedLeft = DefaultThresholdElatedLeft;
            ThresholdElatedRight = DefaultThresholdElatedRight;
            ThresholdWarningLeft = DefaultThresholdWarningLeft;
            ThresholdWarningRight = DefaultThresholdWarningRight;
            ThresholdCriticalLeft = DefaultThresholdCriticalLeft;
            ThresholdCriticalRight = DefaultThresholdCriticalRight;
        }

        [JsonIgnore] public NeedStates NeedStateLeft { get { return needStateLeft; } private set { needStateLeft = value; } }
        [JsonIgnore] public NeedStates NeedStateRight { get { return needStateRight; } private set { needStateRight = value; } }
        [JsonIgnore] public (NeedStates, NeedStates) NeedState { get { return (needStateLeft, needStateRight); } }



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
            LocalAiPriorityWeighting = AiPriorityWeightingLeft;
            LocalAiPriorityWeightingLeft = AiPriorityWeightingLeft;
            LocalAiPriorityWeightingRight = AiPriorityWeightingLeft;

            LevelFull = (DefaultLevelFullLeft, DefaultLevelFullRight);
            LevelCurrent = (DefaultLevelFullLeft, DefaultLevelFullRight);

            BaseChangeRate = (DefaultChangeRateLeft, DefaultChangeRateRight);
            CurrentChangeRate = (DefaultChangeRateLeft, DefaultChangeRateRight);

            ThresholdElatedLeft = DefaultThresholdElatedLeft;
            ThresholdWarningLeft = DefaultThresholdWarningLeft;
            ThresholdCriticalLeft = DefaultThresholdCriticalLeft;
            ThresholdElatedRight = DefaultThresholdElatedRight;
            ThresholdWarningRight = DefaultThresholdWarningRight;
            ThresholdCriticalRight = DefaultThresholdCriticalRight;

            // Hacky UI shit.
            needNameForUI = NeedSO.NeedName;
            needNameLeftForUI = NeedSO.NeedNameLeft;
            needNameRightForUI = NeedSO.NeedNameRight;
        }


        [JsonConstructor]
        public SubjectiveNeed(SubjectiveNeedSO needSO, float localAiPriorityWeighting, float levelFullLeft, float levelFullRight, float levelCurrentLeft, float levelCurrentRight, float baseChangeRateLeft, float baseChangeRateRight, float currentChangeRateLeft, float currentChangeRateRight, float currentChangeRateLeftScaled, float currentChangeRateRightScaled, float thresholdElatedLeft, float thresholdElatedRight, float thresholdWarningLeft, float thresholdWarningRight, float thresholdCriticalLeft, float thresholdCriticalRight, NeedStates needStateLeft, NeedStates needStateRight)
        {
            this.needSO = needSO;

            LocalAiPriorityWeighting = localAiPriorityWeighting;

            LevelFull = (levelFullLeft, levelFullRight);
            LevelCurrent = (levelCurrentLeft, levelCurrentRight);

            BaseChangeRate = (baseChangeRateLeft, baseChangeRateRight);
            CurrentChangeRate = (currentChangeRateLeft, currentChangeRateRight);
            //this.currentChangeRateScaled = currentChangeRateScaled;

            ThresholdElated = (thresholdElatedLeft, thresholdElatedRight);
            ThresholdWarning = (thresholdWarningLeft, thresholdWarningRight);
            ThresholdCritical = (thresholdCriticalLeft, thresholdCriticalRight);

            NeedStateLeft = needStateLeft;
            NeedStateRight = needStateRight;

            // Hacky UI shit.
            needNameForUI = NeedSO.NeedName;
            needNameLeftForUI = NeedSO.NeedNameLeft;
            needNameRightForUI = NeedSO.NeedNameRight;
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
        public BasicNeedSO GetNeediestSide(bool byPercentage = false) // Returns which side of the need is the least fulfilled.
        {
            return (GetLeftFulfillmentDelta(byPercentage) <= GetRightFulfillmentDelta(byPercentage)) ? NeedSORight : NeedSOLeft ;
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
        public static (SubjectiveNeed, BasicNeedSO) ReturnNeediestbyNeediestDelta(SubjectiveNeed[] subjectiveNeeds, bool usePriorityWeights = true, bool byPercentage = false)
        {
            SortByFulfillmentDeltaofNeediest(subjectiveNeeds, usePriorityWeights, byPercentage);
            return (subjectiveNeeds[0], subjectiveNeeds[0].GetNeediestSide());
        }
        public static (SubjectiveNeed, BasicNeedSO) ReturnNeediestbyTotalDelta(SubjectiveNeed[] subjectiveNeeds, bool usePriorityWeights = true, bool byPercentage = false)
        {
            SortByTotalFulfillmentDelta(subjectiveNeeds, usePriorityWeights, byPercentage);
            return (subjectiveNeeds[0], subjectiveNeeds[0].GetNeediestSide());
        }
        public static (SubjectiveNeed, BasicNeedSO) ReturnNeediestbyAverageDelta(SubjectiveNeed[] subjectiveNeeds, bool usePriorityWeights = true, bool byPercentage = false)
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
        public void ModulateNeed(Person sourceBasePerson)
        {
            // Run the modulators.

            // TODO: Move to their own function?
            // Will we need to add arithmetic from bool?

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

            // TODO: clean this up
            // The BaseChangeRates cannot be higher than 0.5f
            if (BaseChangeRateLeft > Constants_Quille.MAX_BASE_CHANGE_RATE)
            {
                BaseChangeRateLeft = Constants_Quille.MAX_BASE_CHANGE_RATE;
            }
            if (BaseChangeRateRight > Constants_Quille.MAX_BASE_CHANGE_RATE)
            {
                BaseChangeRateRight = Constants_Quille.MAX_BASE_CHANGE_RATE;
            }

            // The BaseChangeRates cannot be lower than -0.5f
            if (BaseChangeRateLeft < -Constants_Quille.MAX_BASE_CHANGE_RATE)
            {
                BaseChangeRateLeft = -Constants_Quille.MAX_BASE_CHANGE_RATE;
            }
            if (BaseChangeRateRight < -Constants_Quille.MAX_BASE_CHANGE_RATE)
            {
                BaseChangeRateRight = -Constants_Quille.MAX_BASE_CHANGE_RATE;
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
                    if (this.LevelCurrentLeftAsPercentage <= this.ThresholdCriticalLeft & this.NeedStateLeft > NeedStates.Critical)
                    {
                        this.NeedStateLeft = NeedStates.Critical;
                        Debug.Log(string.Format("{0} ({1}) is critically low ({1:P2})...", this.NeedNameLeft, this.NeedName, 1 - GetLeftFulfillmentDelta(true)));

                        ONSNReachedThreshold?.Invoke(NeedSO, NeedSOLeft, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Critical);
                    }
                    else if (this.LevelCurrentLeftAsPercentage <= this.ThresholdWarningLeft & this.NeedStateLeft > NeedStates.Warning)
                    {
                        this.NeedStateLeft = NeedStates.Warning;
                        Debug.Log(string.Format("{0} ({1}) is a little low ({2:P2})...", this.NeedNameLeft, this.NeedName, 1 - GetLeftFulfillmentDelta(true)));

                        ONSNReachedThreshold?.Invoke(NeedSO, NeedSOLeft, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Warning);
                    }
                    else if (this.LevelCurrentLeftAsPercentage >= this.ThresholdElatedLeft & this.NeedStateLeft != NeedStates.Elated)
                    {
                        this.NeedStateLeft = NeedStates.Elated;
                        Debug.Log(string.Format("{0} ({1}) is elated ({2:P2})...", this.NeedNameLeft, this.NeedName, 1 - GetLeftFulfillmentDelta(true)));

                        ONSNReachedThreshold?.Invoke(NeedSO, NeedSOLeft, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Elated);
                    }
                    else // Unset the Warning and Critical booleans as needed.
                    {
                        if (this.NeedStateLeft == NeedStates.Critical & this.LevelCurrentLeft > this.ThresholdCriticalLeft)
                        {
                            this.NeedStateLeft = NeedStates.Warning;
                            Debug.Log(string.Format("{0} ({1}) is no longer critically low.", this.NeedNameLeft, this.NeedName));

                            OnSNLeftThreshold?.Invoke(NeedSO, NeedSOLeft, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Critical);
                        }
                        if (this.NeedStateLeft == NeedStates.Warning & this.LevelCurrentLeft > this.ThresholdWarningLeft)
                        {
                            this.NeedStateLeft = NeedStates.Normal;
                            Debug.Log(string.Format("{0} ({1}) is no longer low.", this.NeedNameLeft, this.NeedName));

                            OnSNLeftThreshold?.Invoke(NeedSO, NeedSOLeft, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Warning);
                        }
                        if (this.NeedStateLeft == NeedStates.Elated & this.LevelCurrentLeft < this.ThresholdElatedLeft)
                        {
                            this.NeedStateLeft = NeedStates.Normal;
                            Debug.Log(string.Format("{0} ({1}) is no longer elated.", this.NeedNameLeft, this.NeedName));

                            OnSNLeftThreshold?.Invoke(NeedSO, NeedSOLeft, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Elated);
                        }
                    }
                }
                else // The need is currently empty.
                {
                    if (this.NeedStateLeft != NeedStates.Failure) // First detection of the need failure.
                    {
                        this.NeedStateLeft = NeedStates.Failure;
                        Debug.Log(string.Format("{0} ({1}) is now empty.", this.NeedNameLeft, this.NeedName));

                        OnSNFailure?.Invoke(NeedSO, NeedSOLeft);
                    }

                    if (this.CurrentChangeRateLeftScaled > 0) // Only apply the change if it would increase it.
                    {
                        this.LevelCurrentLeft += this.CurrentChangeRateLeftScaled;

                        this.NeedStateLeft = NeedStates.Critical; // Undo the need failure.
                        Debug.Log(string.Format("{0} ({1}) is no longer in need failure.", this.NeedNameLeft, this.NeedName));

                        OnSNLeftThreshold?.Invoke(NeedSO, NeedSOLeft, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Failure);
                    }
                }

                // Handle RightSide subneed.
                if (this.LevelCurrentRight > this.LevelEmptyRight) // The need is not empty.
                {
                    this.LevelCurrentRight += this.CurrentChangeRateRightScaled;
                    // Invoke need change event?

                    // Threshold detection.
                    if (this.NeedStateRight != NeedStates.Elated & this.LevelCurrentRightAsPercentage >= this.ThresholdElatedRight)
                    {
                        this.NeedStateRight = NeedStates.Elated;
                        Debug.Log(string.Format("{0} ({1}) is elated ({2:P2})...", this.NeedNameRight, this.NeedName, 1 - GetRightFulfillmentDelta(true)));

                        ONSNReachedThreshold?.Invoke(NeedSO, NeedSORight, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Elated);
                    }
                    else if (this.NeedStateRight > NeedStates.Critical & this.LevelCurrentRightAsPercentage <= this.ThresholdCriticalRight)
                    {
                        this.NeedStateRight = NeedStates.Critical;
                        Debug.Log(string.Format("{0} ({1}) is critically low ({2:P2})...", this.NeedNameRight, this.NeedName, 1 - GetRightFulfillmentDelta(true)));

                        ONSNReachedThreshold?.Invoke(NeedSO, NeedSORight, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Critical);
                    }
                    else if (this.NeedStateRight > NeedStates.Warning & this.LevelCurrentRightAsPercentage <= this.ThresholdWarningRight)
                    {
                        this.NeedStateRight = NeedStates.Warning;
                        Debug.Log(string.Format("{0} ({1}) is a little low ({2:P2})...", this.NeedNameRight, this.NeedName, 1 - GetRightFulfillmentDelta(true)));

                        ONSNReachedThreshold?.Invoke(NeedSO, NeedSORight, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Warning);
                    }
                    else // Unset the Warning and Critical booleans as needed.
                    {
                        if (this.NeedStateRight == NeedStates.Critical & this.LevelCurrentRight > this.ThresholdCriticalRight)
                        {
                            this.NeedStateRight = NeedStates.Warning;
                            Debug.Log(string.Format("{0} ({1}) is no longer critically low.", this.NeedNameRight, this.NeedName));

                            OnSNLeftThreshold?.Invoke(NeedSO, NeedSORight, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Critical);
                        }
                        if (this.NeedStateRight == NeedStates.Warning & this.LevelCurrentRight > this.ThresholdWarningRight)
                        {
                            this.NeedStateRight = NeedStates.Normal;
                            Debug.Log(string.Format("{0} ({1}) is no longer low.", this.NeedNameRight, this.NeedName));

                            OnSNLeftThreshold?.Invoke(NeedSO, NeedSORight, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Warning);
                        }
                        if (this.NeedStateRight == NeedStates.Elated & this.LevelCurrentRight < this.ThresholdElatedRight)
                        {
                            this.NeedStateRight = NeedStates.Normal;
                            Debug.Log(string.Format("{0} ({1}) is no longer elated.", this.NeedNameRight, this.NeedName));

                            OnSNLeftThreshold?.Invoke(NeedSO, NeedSORight, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Elated);
                        }
                    }
                }
                else // The need is currently empty.
                {
                    if (this.NeedStateRight != NeedStates.Failure) // First detection of the need failure.
                    {
                        this.NeedStateRight = NeedStates.Failure;
                        Debug.Log(string.Format("{0} ({1}) is now empty.", this.NeedNameRight, this.NeedName));

                        OnSNFailure?.Invoke(NeedSO, NeedSORight);
                    }

                    if (this.CurrentChangeRateRightScaled > 0) // Only apply the change if it would increase it.
                    {
                        this.LevelCurrentRight += this.CurrentChangeRateRightScaled;

                        this.NeedStateRight = NeedStates.Critical; // Undo the need failure.
                        Debug.Log(string.Format("{0} ({1}) is no longer in need failure.", this.NeedNameRight, this.NeedName));

                        OnSNLeftThreshold?.Invoke(NeedSO, NeedSORight, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Failure);
                    }
                }

                yield return new WaitForSeconds(Constants_Quille.NEED_DECAY_INTERVAL);
            }
        }
    }
}
