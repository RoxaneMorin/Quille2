using System;
using System.Linq;
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

        [SerializeField] private BasicNeed subneedLeft;
        [SerializeField] private BasicNeed subneedRight;

        private Dictionary<BasicNeedSO, BasicNeed> subneedsBySOs;

        [SerializeField] private float localAiPriorityWeighting;

        [InspectorReadOnly] public string needNameForUI;
        [InspectorReadOnly] public string needNameLeftForUI;
        [InspectorReadOnly] public string needNameRightForUI;



        // PROPERTIES
        public SubjectiveNeedSO NeedSO { get { return needSO; } }
        public BasicNeedSO SubneedSOLeft { get { return needSO.NeedSORight; } }
        public BasicNeedSO SubneedSORight { get { return needSO.NeedSORight; } }

        public BasicNeed SubneedLeft { get { return subneedLeft; } }
        public BasicNeed SubneedRight { get { return subneedRight; } }
        public Dictionary<BasicNeedSO, BasicNeed> SubneedsDict { get { return subneedsBySOs; } }

        public string NeedName { get { return needSO.NeedName; } }
        public string NeedNameLeft { get { return subneedLeft.NeedName; } }
        public string NeedNameRight { get { return subneedRight.NeedName; } }
        public Sprite NeedIconLeft { get { return subneedLeft.NeedIcon; } }
        public Sprite NeedIconRight { get { return subneedRight.NeedIcon; } }

        public float AiPriorityWeightingLeft { get { return subneedLeft.AiPriorityWeighting; } }
        public float AiPriorityWeightingRight { get { return subneedRight.AiPriorityWeighting; } }
        public float LocalAiPriorityWeighting
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
        public float LocalAiPriorityWeightingLeft
        {
            get { return subneedLeft.LocalAiPriorityWeighting; }
            set
            {
                if (value < Constants_Quille.MIN_PRIORITY)
                {
                    subneedLeft.LocalAiPriorityWeighting = Constants_Quille.MIN_PRIORITY;
                    return;
                }
                else if (value > Constants_Quille.MAX_PRIORITY)
                {
                    subneedLeft.LocalAiPriorityWeighting = Constants_Quille.MAX_PRIORITY;
                    return;
                }
                else subneedLeft.LocalAiPriorityWeighting = value;
            }
        }
        public float LocalAiPriorityWeightingRight
        {
            get { return subneedRight.LocalAiPriorityWeighting; }
            set
            {
                if (value < Constants_Quille.MIN_PRIORITY)
                {
                    subneedRight.LocalAiPriorityWeighting = Constants_Quille.MIN_PRIORITY;
                    return;
                }
                else if (value > Constants_Quille.MAX_PRIORITY)
                {
                    subneedRight.LocalAiPriorityWeighting = Constants_Quille.MAX_PRIORITY;
                    return;
                }
                else subneedRight.LocalAiPriorityWeighting = value;
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
            get { return subneedLeft.LevelFull; }
            set
            {
                if (value > LevelEmptyLeft)
                {
                    subneedLeft.LevelFull = value;
                }
            }
        }
        public float LevelFullRight
        {
            get { return subneedRight.LevelFull; }
            set
            {
                if (value > LevelEmptyRight)
                {
                    subneedRight.LevelFull = value;
                }
            }
        }
        public (float, float) LevelFull
        {
            get { return (LevelFullLeft, LevelFullRight); }
            set
            {
                LevelFullLeft = value.Item1;
                LevelFullRight = value.Item2;
            }
        }
        public float LevelFullFor(BasicNeedSO subNeed)
        {
            try
            {
                return subneedsBySOs[subNeed].LevelFull;
            }
            catch(Exception e)
            {
                Debug.LogError(string.Format("{0} is not a valid subneed for {1}.", subNeed.NeedName, NeedSO.NeedName));
                return 0;
            }
        }

        public float LevelCurrentLeft
        {
            get { return subneedLeft.LevelCurrent; }
            set
            {
                if (value >= DefaultLevelFullLeft)
                {
                    subneedLeft.LevelCurrent = DefaultLevelFullLeft;
                    return;
                }
                else if (value <= LevelEmptyLeft)
                {
                    subneedLeft.LevelCurrent = LevelEmptyLeft;
                    // Throw need failure event here?
                    return;
                }
                else subneedLeft.LevelCurrent = value;
            }
        }
        public float LevelCurrentLeftAsPercentage
        {
            get { return LevelCurrentLeft / LevelFullLeft; }
        }
        public float LevelCurrentRight
        {
            get { return subneedRight.LevelCurrent; }
            set
            {
                if (value >= DefaultLevelFullRight)
                {
                    subneedRight.LevelCurrent = DefaultLevelFullRight;
                    return;
                }
                else if (value <= LevelEmptyRight)
                {
                    subneedRight.LevelCurrent = LevelEmptyRight;
                    // Throw need failure event here?
                    return;
                }
                else subneedRight.LevelCurrent = value;
            }
        }
        public float LevelCurrentRightAsPercentage
        {
            get { return LevelCurrentRight / LevelFullRight; }
        }
        public (float, float) LevelCurrent
        {
            get { return (LevelCurrentLeft, LevelCurrentRight); }
            set
            {
                LevelCurrentLeft = value.Item1;
                LevelCurrentRight = value.Item2;
            }
        }
        public (float, float) LevelCurrentAsPercentage
        {
            get { return (LevelCurrentLeft / LevelFullLeft, LevelCurrentRight / LevelFullRight); }
        }
        public float LevelCurrentFor(BasicNeedSO subNeed)
        {
            try
            {
                return subneedsBySOs[subNeed].LevelCurrent;
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("{0} is not a valid subneed for {1}.", subNeed.NeedName, NeedSO.NeedName));
                return 0;
            }
        }
        public float LevelCurrentAsPercentageFor(BasicNeedSO subNeed)
        {
            try
            {
                return subneedsBySOs[subNeed].LevelCurrent / subneedsBySOs[subNeed].LevelFull;
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("{0} is not a valid subneed for {1}.", subNeed.NeedName, NeedSO.NeedName));
                return 0;
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
        public float DefaultChangeRateFor(BasicNeedSO subNeed)
        {
            try
            {
                return subneedsBySOs[subNeed].DefaultChangeRate;
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("{0} is not a valid subneed for {1}.", subNeed.NeedName, NeedSO.NeedName));
                return 0;
            }
        }

        public float BaseChangeRateLeft
        {
            get { return subneedLeft.BaseChangeRate; }
            set { subneedLeft.BaseChangeRate = value; }
        }
        public float BaseChangeRateRight
        {
            get { return subneedRight.BaseChangeRate; }
            set { subneedRight.BaseChangeRate = value; }
        }
        public (float, float) BaseChangeRate
        {
            get { return (BaseChangeRateLeft, BaseChangeRateRight); }
            set
            {
                BaseChangeRateLeft = value.Item1;
                BaseChangeRateRight = value.Item2;
            }
        }
        public float BaseChangeRateFor(BasicNeedSO subNeed)
        {
            try
            {
                return subneedsBySOs[subNeed].BaseChangeRate;
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("{0} is not a valid subneed for {1}.", subNeed.NeedName, NeedSO.NeedName));
                return 0;
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

        public float CurrentChangeRateLeft
        {
            get { return subneedLeft.CurrentChangeRate; }
            set
            {
                subneedLeft.CurrentChangeRate = value;
            }
        }
        public float CurrentChangeRateRight
        {
            get { return subneedRight.CurrentChangeRate; }
            set
            {
                subneedRight.CurrentChangeRate = value;
            }
        }
        public (float, float) CurrentChangeRate
        {
            get { return (CurrentChangeRateLeft, CurrentChangeRateRight); }
            set
            {
                CurrentChangeRateLeft = value.Item1;
                CurrentChangeRateRight = value.Item2;
            }
        }
        public float CurrentChangeRateFor(BasicNeedSO subNeed)
        {
            try
            {
                return subneedsBySOs[subNeed].CurrentChangeRate;
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("{0} is not a valid subneed for {1}.", subNeed.NeedName, NeedSO.NeedName));
                return 0;
            }
        }

        public float CurrentChangeRateLeftScaled
        {
            get { return subneedLeft.CurrentChangeRateScaled; }
        }
        public float CurrentChangeRateRightScaled
        {
            get { return subneedRight.CurrentChangeRateScaled; }
        }
        public (float, float) CurrentChangeRateScaled
        {
            get { return (CurrentChangeRateLeftScaled, CurrentChangeRateLeftScaled); }
        }
        public float CurrentChangeRateScaledFor(BasicNeedSO subNeed)
        {
            try
            {
                return subneedsBySOs[subNeed].CurrentChangeRateScaled;
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("{0} is not a valid subneed for {1}.", subNeed.NeedName, NeedSO.NeedName));
                return 0;
            }
        }

        public void ResetCurrentChangeRateLeft()
        {
            CurrentChangeRateLeft = BaseChangeRateLeft;
        }
        public void ResetCurrentChangeRateRight()
        {
            CurrentChangeRateRight = BaseChangeRateRight;
        }
        public void ResetCurrentChangeRate()
        {
            ResetCurrentChangeRateLeft();
            ResetCurrentChangeRateRight();
        }

        public float DefaultThresholdElatedLeft { get { return needSO.ThresholdElatedLeft; } }
        public float DefaultThresholdWarningLeft { get { return needSO.ThresholdWarningLeft; } }
        public float DefaultThresholdCriticalLeft { get { return needSO.ThresholdCriticalLeft; } }
        public float DefaultThresholdElatedRight { get { return needSO.ThresholdElatedRight; } }
        public float DefaultThresholdWarningRight { get { return needSO.ThresholdWarningRight; } }
        public float DefaultThresholdCriticalRight { get { return needSO.ThresholdCriticalRight; } }

        public float ThresholdElatedLeft
        {
            get { return subneedLeft.ThresholdElated; }
            set
            {
                if (value > Constants_Quille.DEFAULT_LEVEL_FULL)
                {
                    subneedLeft.ThresholdElated = Constants_Quille.DEFAULT_LEVEL_FULL;
                    return;
                }
                else if (value < Constants_Quille.MAX_THRESHOLD_NEGATIVE + 0.05f)
                {
                    subneedLeft.ThresholdElated = Constants_Quille.MAX_THRESHOLD_NEGATIVE + 0.05f;
                    return;
                }
                else subneedLeft.ThresholdElated = value;
            }
        }
        public float ThresholdWarningLeft
        {
            get { return subneedLeft.ThresholdWarning; }
            set
            {
                if (value > Constants_Quille.MAX_THRESHOLD_NEGATIVE)
                {
                    subneedLeft.ThresholdWarning = Constants_Quille.MAX_THRESHOLD_NEGATIVE;
                    return;
                }
                else if (value < Constants_Quille.MIN_THRESHOLD_NEGATIVE + 0.05f)
                {
                    subneedLeft.ThresholdWarning = 0;
                    return;
                }
                else subneedLeft.ThresholdWarning = value;
            }
        }
        public float ThresholdCriticalLeft
        {
            get { return subneedLeft.ThresholdCritical; }
            set
            {
                if (value > Constants_Quille.MAX_THRESHOLD_NEGATIVE - 0.05f)
                {
                    subneedLeft.ThresholdCritical = Constants_Quille.MAX_THRESHOLD_NEGATIVE - 0.05f;
                    return;
                }
                else if (value < Constants_Quille.MIN_THRESHOLD_NEGATIVE)
                {
                    subneedLeft.ThresholdCritical = Constants_Quille.MIN_THRESHOLD_NEGATIVE;
                    return;
                }
                else subneedLeft.ThresholdCritical = value;
            }
        }
        public float ThresholdElatedRight
        {
            get { return subneedRight.ThresholdElated; }
            set
            {
                if (value > Constants_Quille.DEFAULT_LEVEL_FULL)
                {
                    subneedRight.ThresholdElated = Constants_Quille.DEFAULT_LEVEL_FULL;
                    return;
                }
                else if (value < Constants_Quille.MAX_THRESHOLD_NEGATIVE + 0.05f)
                {
                    subneedRight.ThresholdElated = Constants_Quille.MAX_THRESHOLD_NEGATIVE + 0.05f;
                    return;
                }
                else subneedRight.ThresholdElated = value;
            }
        }
        public float ThresholdWarningRight
        {
            get { return subneedRight.ThresholdWarning; }
            set
            {
                if (value > Constants_Quille.MAX_THRESHOLD_NEGATIVE)
                {
                    subneedRight.ThresholdWarning = Constants_Quille.MAX_THRESHOLD_NEGATIVE;
                    return;
                }
                else if (value < Constants_Quille.MIN_THRESHOLD_NEGATIVE + 0.05f)
                {
                    subneedRight.ThresholdWarning = Constants_Quille.MIN_THRESHOLD_NEGATIVE + 0.05f;
                    return;
                }
                else subneedRight.ThresholdWarning = value;
            }
        }
        public float ThresholdCriticalRight
        {
            get { return subneedRight.ThresholdCritical; }
            set
            {
                if (value > Constants_Quille.MAX_THRESHOLD_NEGATIVE - 0.05f)
                {
                    subneedRight.ThresholdCritical = Constants_Quille.MAX_THRESHOLD_NEGATIVE - 0.05f;
                    return;
                }
                else if (value < Constants_Quille.MIN_THRESHOLD_NEGATIVE)
                {
                    subneedRight.ThresholdCritical = Constants_Quille.MIN_THRESHOLD_NEGATIVE;
                    return;
                }
                else subneedRight.ThresholdCritical = value;
            }
        }
        public (float, float) ThresholdElated
        {
            get { return (ThresholdElatedLeft, ThresholdElatedRight); }
            set
            {
                ThresholdElatedLeft = value.Item1;
                ThresholdElatedRight = value.Item2;
            }
        }
        public (float, float) ThresholdWarning
        {
            get { return (ThresholdWarningLeft, ThresholdWarningRight); }
            set
            {
                ThresholdWarningLeft = value.Item1;
                ThresholdWarningRight = value.Item2;
            }
        }
        public (float, float) ThresholdCritical
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

        public NeedStates NeedStateLeft { get { return subneedLeft.NeedState; } private set { subneedLeft.NeedState = value; } }
        public NeedStates NeedStateRight { get { return subneedRight.NeedState; } private set { subneedRight.NeedState = value; } }
        public (NeedStates, NeedStates) NeedState { get { return (NeedStateLeft, NeedStateRight); } }

        public IEnumerator[] AlterSubneedLevelsByChangeRate { get { return new IEnumerator[] { subneedLeft.AlterLevelByChangeRate(), subneedRight.AlterLevelByChangeRate() }; } }



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
            subneedLeft = new BasicNeed(needSO.NeedSOLeft);
            subneedRight = new BasicNeed(needSO.NeedSORight);

            subneedsBySOs = new Dictionary<BasicNeedSO, BasicNeed>
            {
                { needSO.NeedSOLeft, subneedLeft},
                { needSO.NeedSORight, subneedRight }
            };

            LocalAiPriorityWeighting = AiPriorityWeightingLeft;
            //LocalAiPriorityWeightingLeft = AiPriorityWeightingLeft;
            //LocalAiPriorityWeightingRight = AiPriorityWeightingLeft;

            //LevelFull = (DefaultLevelFullLeft, DefaultLevelFullRight);
            //LevelCurrent = (DefaultLevelFullLeft, DefaultLevelFullRight);

            //BaseChangeRate = (DefaultChangeRateLeft, DefaultChangeRateRight);
            //CurrentChangeRate = (DefaultChangeRateLeft, DefaultChangeRateRight);

            //ThresholdElatedLeft = DefaultThresholdElatedLeft;
            //ThresholdWarningLeft = DefaultThresholdWarningLeft;
            //ThresholdCriticalLeft = DefaultThresholdCriticalLeft;
            //ThresholdElatedRight = DefaultThresholdElatedRight;
            //ThresholdWarningRight = DefaultThresholdWarningRight;
            //ThresholdCriticalRight = DefaultThresholdCriticalRight;

            // Hacky UI shit.
            needNameForUI = NeedSO.NeedName;
            needNameLeftForUI = NeedSO.NeedNameLeft;
            needNameRightForUI = NeedSO.NeedNameRight;
        }


        [JsonConstructor]
        public SubjectiveNeed(SubjectiveNeedSO needSO, float localAiPriorityWeighting, BasicNeed subneedLeft = null, BasicNeed subneedRight = null)
        {
            this.needSO = needSO;
 
            if (subneedLeft == null)
                this.subneedLeft = new BasicNeed(this.needSO.NeedSOLeft);
            else
                this.subneedLeft = subneedLeft;

            if (subneedRight == null)
                this.subneedRight = new BasicNeed(this.needSO.NeedSORight);
            else
                this.subneedRight = subneedRight;

            subneedsBySOs = new Dictionary<BasicNeedSO, BasicNeed>
            {
                { needSO.NeedSOLeft, subneedLeft},
                { needSO.NeedSORight, subneedRight }
            };

            LocalAiPriorityWeighting = localAiPriorityWeighting;

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
            return (GetLeftFulfillmentDelta(byPercentage) <= GetRightFulfillmentDelta(byPercentage)) ? SubneedSORight : SubneedSOLeft ;
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

        // Returns a sorted array of all needs below a certain threshold.
        public static BasicNeedSO[] ReturnNeedy(SubjectiveNeed[] subjectiveNeeds, float needinessThreshold, bool usePriorityWeights = true, bool byPercentage = false)
        {
            // TODO: can I avoid creating two enumerables?
            IEnumerable<BasicNeed> needyNeedsLeft = subjectiveNeeds.Where(need => (byPercentage ? need.LevelCurrentLeftAsPercentage : need.LevelCurrentLeft) <= needinessThreshold).Select(need => need.subneedLeft);
            IEnumerable<BasicNeed> needyNeedsRight = subjectiveNeeds.Where(need => (byPercentage ? need.LevelCurrentRightAsPercentage : need.LevelCurrentRight) <= needinessThreshold).Select(need => need.subneedRight);

            BasicNeed[] needyNeeds = needyNeedsLeft.Concat(needyNeedsRight).ToArray();
            BasicNeed.SortByFulfillmentDelta(needyNeeds, usePriorityWeights, byPercentage);

            return needyNeeds.Select(need => need.NeedSO).ToArray();
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
                    needDeltaA *= isNeediestSideRightA ? needA.LocalAiPriorityWeightingRight : needA.LocalAiPriorityWeightingLeft;
                    needDeltaB *= isNeediestSideRightB ? needB.LocalAiPriorityWeightingRight : needB.LocalAiPriorityWeightingLeft;
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
            // TODO: use the basicNeeds' coroutines instead?

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

                        ONSNReachedThreshold?.Invoke(NeedSO, SubneedSOLeft, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Critical);
                    }
                    else if (this.LevelCurrentLeftAsPercentage <= this.ThresholdWarningLeft & this.NeedStateLeft > NeedStates.Warning)
                    {
                        this.NeedStateLeft = NeedStates.Warning;
                        Debug.Log(string.Format("{0} ({1}) is a little low ({2:P2})...", this.NeedNameLeft, this.NeedName, 1 - GetLeftFulfillmentDelta(true)));

                        ONSNReachedThreshold?.Invoke(NeedSO, SubneedSOLeft, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Warning);
                    }
                    else if (this.LevelCurrentLeftAsPercentage >= this.ThresholdElatedLeft & this.NeedStateLeft != NeedStates.Elated)
                    {
                        this.NeedStateLeft = NeedStates.Elated;
                        Debug.Log(string.Format("{0} ({1}) is elated ({2:P2})...", this.NeedNameLeft, this.NeedName, 1 - GetLeftFulfillmentDelta(true)));

                        ONSNReachedThreshold?.Invoke(NeedSO, SubneedSOLeft, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Elated);
                    }
                    else // Unset the Warning and Critical booleans as needed.
                    {
                        if (this.NeedStateLeft == NeedStates.Critical & this.LevelCurrentLeft > this.ThresholdCriticalLeft)
                        {
                            this.NeedStateLeft = NeedStates.Warning;
                            Debug.Log(string.Format("{0} ({1}) is no longer critically low.", this.NeedNameLeft, this.NeedName));

                            OnSNLeftThreshold?.Invoke(NeedSO, SubneedSOLeft, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Critical);
                        }
                        if (this.NeedStateLeft == NeedStates.Warning & this.LevelCurrentLeft > this.ThresholdWarningLeft)
                        {
                            this.NeedStateLeft = NeedStates.Normal;
                            Debug.Log(string.Format("{0} ({1}) is no longer low.", this.NeedNameLeft, this.NeedName));

                            OnSNLeftThreshold?.Invoke(NeedSO, SubneedSOLeft, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Warning);
                        }
                        if (this.NeedStateLeft == NeedStates.Elated & this.LevelCurrentLeft < this.ThresholdElatedLeft)
                        {
                            this.NeedStateLeft = NeedStates.Normal;
                            Debug.Log(string.Format("{0} ({1}) is no longer elated.", this.NeedNameLeft, this.NeedName));

                            OnSNLeftThreshold?.Invoke(NeedSO, SubneedSOLeft, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Elated);
                        }
                    }
                }
                else // The need is currently empty.
                {
                    if (this.NeedStateLeft != NeedStates.Failure) // First detection of the need failure.
                    {
                        this.NeedStateLeft = NeedStates.Failure;
                        Debug.Log(string.Format("{0} ({1}) is now empty.", this.NeedNameLeft, this.NeedName));

                        OnSNFailure?.Invoke(NeedSO, SubneedSOLeft);
                    }

                    if (this.CurrentChangeRateLeftScaled > 0) // Only apply the change if it would increase it.
                    {
                        this.LevelCurrentLeft += this.CurrentChangeRateLeftScaled;

                        this.NeedStateLeft = NeedStates.Critical; // Undo the need failure.
                        Debug.Log(string.Format("{0} ({1}) is no longer in need failure.", this.NeedNameLeft, this.NeedName));

                        OnSNLeftThreshold?.Invoke(NeedSO, SubneedSOLeft, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Failure);
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

                        ONSNReachedThreshold?.Invoke(NeedSO, SubneedSORight, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Elated);
                    }
                    else if (this.NeedStateRight > NeedStates.Critical & this.LevelCurrentRightAsPercentage <= this.ThresholdCriticalRight)
                    {
                        this.NeedStateRight = NeedStates.Critical;
                        Debug.Log(string.Format("{0} ({1}) is critically low ({2:P2})...", this.NeedNameRight, this.NeedName, 1 - GetRightFulfillmentDelta(true)));

                        ONSNReachedThreshold?.Invoke(NeedSO, SubneedSORight, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Critical);
                    }
                    else if (this.NeedStateRight > NeedStates.Warning & this.LevelCurrentRightAsPercentage <= this.ThresholdWarningRight)
                    {
                        this.NeedStateRight = NeedStates.Warning;
                        Debug.Log(string.Format("{0} ({1}) is a little low ({2:P2})...", this.NeedNameRight, this.NeedName, 1 - GetRightFulfillmentDelta(true)));

                        ONSNReachedThreshold?.Invoke(NeedSO, SubneedSORight, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Warning);
                    }
                    else // Unset the Warning and Critical booleans as needed.
                    {
                        if (this.NeedStateRight == NeedStates.Critical & this.LevelCurrentRight > this.ThresholdCriticalRight)
                        {
                            this.NeedStateRight = NeedStates.Warning;
                            Debug.Log(string.Format("{0} ({1}) is no longer critically low.", this.NeedNameRight, this.NeedName));

                            OnSNLeftThreshold?.Invoke(NeedSO, SubneedSORight, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Critical);
                        }
                        if (this.NeedStateRight == NeedStates.Warning & this.LevelCurrentRight > this.ThresholdWarningRight)
                        {
                            this.NeedStateRight = NeedStates.Normal;
                            Debug.Log(string.Format("{0} ({1}) is no longer low.", this.NeedNameRight, this.NeedName));

                            OnSNLeftThreshold?.Invoke(NeedSO, SubneedSORight, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Warning);
                        }
                        if (this.NeedStateRight == NeedStates.Elated & this.LevelCurrentRight < this.ThresholdElatedRight)
                        {
                            this.NeedStateRight = NeedStates.Normal;
                            Debug.Log(string.Format("{0} ({1}) is no longer elated.", this.NeedNameRight, this.NeedName));

                            OnSNLeftThreshold?.Invoke(NeedSO, SubneedSORight, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Elated);
                        }
                    }
                }
                else // The need is currently empty.
                {
                    if (this.NeedStateRight != NeedStates.Failure) // First detection of the need failure.
                    {
                        this.NeedStateRight = NeedStates.Failure;
                        Debug.Log(string.Format("{0} ({1}) is now empty.", this.NeedNameRight, this.NeedName));

                        OnSNFailure?.Invoke(NeedSO, SubneedSORight);
                    }

                    if (this.CurrentChangeRateRightScaled > 0) // Only apply the change if it would increase it.
                    {
                        this.LevelCurrentRight += this.CurrentChangeRateRightScaled;

                        this.NeedStateRight = NeedStates.Critical; // Undo the need failure.
                        Debug.Log(string.Format("{0} ({1}) is no longer in need failure.", this.NeedNameRight, this.NeedName));

                        OnSNLeftThreshold?.Invoke(NeedSO, SubneedSORight, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Failure);
                    }
                }

                yield return new WaitForSeconds(Constants_Quille.NEED_DECAY_INTERVAL);
            }
        }
    }
}
