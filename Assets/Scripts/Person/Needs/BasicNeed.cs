using System;
using System.Collections;
using UnityEngine;

namespace Quille
{
    [System.Serializable]
    public class BasicNeed
    {
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

        [SerializeField, InspectorReadOnly]
        private float currentChangeRateScaled;

        [SerializeField]
        private float thresholdWarning,
                      thresholdCritical;

        [SerializeField, InspectorReadOnly]
        private NeedStates needState = NeedStates.Normal;



        // PROPERTIES
        public BasicNeedSO NeedSO { get { return needSO; } }

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
            set 
            { 
                currentChangeRate = value;
                currentChangeRateScaled = currentChangeRate/Constants.NEED_CHANGE_RATE_DIVIDER;
            }
        }
        public float CurrentChangeRateScaled
        {
            get { return currentChangeRateScaled; }
        }
        public void ResetCurrentChangeRate()
        {
            CurrentChangeRate = baseChangeRate;
        }

        public float DefaultThresholdWarning { get { return needSO.ThresholdWarning; } }
        public float DefaultThresholdCritical { get { return needSO.ThresholdCritical; } }
        public float ThresholdWarning
        {
            get { return thresholdWarning; }
            set
            {
                if (value > Constants.MAX_THRESHOLD)
                {
                    thresholdWarning = Constants.MAX_THRESHOLD;
                    return;
                }
                else if (value < Constants.MIN_THRESHOLD + 0.05f)
                {
                    thresholdWarning = Constants.MIN_THRESHOLD + 0.05f;
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
                if (value > Constants.MAX_THRESHOLD - 0.05f)
                {
                    thresholdCritical = Constants.MAX_THRESHOLD - 0.05f;
                    return;
                }
                else if (value < Constants.MIN_THRESHOLD)
                {
                    thresholdCritical = Constants.MIN_THRESHOLD;
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

        public NeedStates NeedState { get { return needState; } private set { needState = value; } }



        // EVENTS
        // The need's level has been updated.
        public event BasicNeedLevelCurrentUpdate OnBNLevelCurrentUpdate;

        // General update event for all values? Pass a reference to this object itself?

        // The Warning or Critical threshold is reached.
        public event BasicNeedReachedThreshold OnBNReachedThreshold;

        // The need is no longer in Failure, Critical or Warning.
        public event BasicNeedLeftThreshold OnBNLeftThreshold;

        // Need failure is reached.
        public event BasicNeedFailure OnBNFailure;



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

        // Modulate default values.
        // Clamp the result


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
        
        // Calculates and returns the neediest need from an array of them.
        public static BasicNeed ReturnNeediest(BasicNeed[] basicNeeds, bool usePriorityWeights = true, bool byPercentage = false)
        {
            SortByFulfillmentDelta(basicNeeds, usePriorityWeights, byPercentage);
            return basicNeeds[0];
        }

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



        // Init.
        public void Init(BasePerson sourceBasePerson)
        {
            // Run the modulators.

            // AI weighting.
            foreach (ChecksAndMods.ModulatorArithmeticFromFloat modulator in needSO.BaseAIWeightingModulatedBy)
            {
                LocalAiPriorityWeighting = modulator.Execute(sourceBasePerson, LocalAiPriorityWeighting);
            }

            // Base change rates.
            // TO DO: disallow static base change rates?
            foreach (ChecksAndMods.ModulatorArithmeticFromFloat modulator in needSO.BaseChangeRateModulatedBy)
            {
                BaseChangeRate = modulator.Execute(sourceBasePerson, BaseChangeRate);
            }

            // Thresholds.
            foreach (ChecksAndMods.ModulatorArithmeticFromFloat modulator in needSO.ThresholdsModulatedBy)
            {
                ThresholdWarning = modulator.Execute(sourceBasePerson, ThresholdWarning);
                ThresholdCritical = modulator.Execute(sourceBasePerson, ThresholdCritical);
            }
        }



        // COROUTINES
        // Every second, alter this need's fulfillment level by its current change rate.
        public IEnumerator AlterLevelByChangeRate()
        {
            // Change rate division stuff.

            while (true)
            {
                if (this.LevelCurrent > this.LevelEmpty) // The need is not empty.
                {
                    this.LevelCurrent += this.CurrentChangeRateScaled;
                    // Invoke need change event?

                    // Threshold detection.
                    if (this.NeedState > NeedStates.Critical & this.LevelCurrentAsPercentage <= this.ThresholdCritical)
                    {
                        this.NeedState = NeedStates.Critical;
                        Debug.Log(string.Format("{0} is critically low ({1:P2})...", this.NeedName, 1 - GetFulfillmentDelta(true)));

                        OnBNReachedThreshold?.Invoke(NeedSO, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Critical);
                    }
                    else if (this.NeedState > NeedStates.Warning & this.LevelCurrentAsPercentage <= this.ThresholdWarning)
                    {
                        this.NeedState = NeedStates.Warning;
                        Debug.Log(string.Format("{0} is a little low ({1:P2})...", this.NeedName, 1 - GetFulfillmentDelta(true)));

                        OnBNReachedThreshold?.Invoke(NeedSO, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Warning);
                    }
                    else // Unset the Warning and Critical booleans as needed.
                    {
                        if (this.NeedState == NeedStates.Critical & this.LevelCurrent > this.ThresholdCritical)
                        {
                            this.NeedState = NeedStates.Warning;
                            Debug.Log(string.Format("{0} is no longer critically low.", this.NeedName));

                            OnBNLeftThreshold?.Invoke(NeedSO, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Critical);
                        }
                        if (this.NeedState == NeedStates.Warning & this.LevelCurrent > this.ThresholdWarning)
                        {
                            this.NeedState = NeedStates.Normal;
                            Debug.Log(string.Format("{0} is no longer low.", this.NeedName));

                            OnBNLeftThreshold?.Invoke(NeedSO, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Warning);
                        }
                    }
                }
                else // The need is currently empty.
                {
                    if (this.NeedState != NeedStates.Failure) // First detection of the need failure.
                    {
                        this.NeedState = NeedStates.Failure;
                        Debug.Log(string.Format("{0} is now empty.", this.NeedName));

                        OnBNFailure?.Invoke(NeedSO);
                    }

                    if (this.CurrentChangeRateScaled > 0) // Only apply the change if it would increase it.
                    {
                        this.LevelCurrent += this.CurrentChangeRateScaled;

                        this.NeedState = NeedStates.Critical; // Undo the need failure.
                        Debug.Log(string.Format("{0} is no longer in need failure.", this.NeedName));

                        OnBNLeftThreshold?.Invoke(NeedSO, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Failure);
                    }
                }
                yield return new WaitForSeconds(1);
            }
        }
    }
}
