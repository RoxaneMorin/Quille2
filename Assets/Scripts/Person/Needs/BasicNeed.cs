using System;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json;

namespace Quille
{
    // The C# object component of a BasicNeed. Instances derive their specificities from their associated BasicNeedSO.
    // These needs represent basic physiological and psychological drives such as hunger and stress.
    // JSON serializable, though instance serialization is handled by the NeedController containing them.


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

        [JsonIgnore, SerializeField] //, InspectorReadOnly]
        private float currentChangeRateScaled;

        [SerializeField]
        private float thresholdElated,
                      thresholdWarning,
                      thresholdCritical;

        [SerializeField, InspectorReadOnly]
        private NeedStates needState = NeedStates.Normal;

        [InspectorReadOnly, JsonIgnore]
        public string needNameForUI;



        // PROPERTIES
        [JsonIgnore] public BasicNeedSO NeedSO { get { return needSO; } }

        [JsonIgnore] public string NeedName { get { return needSO.NeedName; } }
        [JsonIgnore] public Sprite NeedIcon { get { return needSO.needIcon; } }

        [JsonIgnore] public float AiPriorityWeighting { get { return needSO.AiPriorityWeighting; } }
        [JsonIgnore] public float LocalAiPriorityWeighting
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

        [JsonIgnore] public float LevelEmpty { get { return needSO.LevelEmpty; } }
        [JsonIgnore] public float DefaultLevelFull { get { return needSO.LevelFull; } }
        [JsonIgnore] public float LevelFull
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
        [JsonIgnore] public float LevelCurrent
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
        [JsonIgnore] public float LevelCurrentAsPercentage
        {
            get { return levelCurrent / levelFull; }
        }

        [JsonIgnore] public float DefaultChangeRate
        {
            get { return needSO.DefaultChangeRate; }
        }

        [JsonIgnore] public float BaseChangeRate
        {
            get { return baseChangeRate;  }
            set { baseChangeRate = value; }
        }
        public void ResetBaseChangeRate()
        {
            baseChangeRate = needSO.DefaultChangeRate;
        }

        [JsonIgnore] public float CurrentChangeRate
        {
            get { return currentChangeRate; }
            set 
            { 
                currentChangeRate = value;
                currentChangeRateScaled = currentChangeRate/Constants.NEED_CHANGE_RATE_DIVIDER;
            }
        }
        [JsonIgnore] public float CurrentChangeRateScaled
        {
            get { return currentChangeRateScaled; }
        }
        public void ResetCurrentChangeRate()
        {
            CurrentChangeRate = baseChangeRate;
        }

        [JsonIgnore] public float DefaultThresholdElated { get { return needSO.ThresholdElated; } }
        [JsonIgnore] public float DefaultThresholdWarning { get { return needSO.ThresholdWarning; } }
        [JsonIgnore] public float DefaultThresholdCritical { get { return needSO.ThresholdCritical; } }
        [JsonIgnore]
        public float ThresholdElated
        {
            get { return thresholdElated; }
            set
            {
                if (value > Constants.DEFAULT_LEVEL_FULL)
                {
                    thresholdElated = Constants.DEFAULT_LEVEL_FULL;
                    return;
                }
                else if (value < Constants.MAX_THRESHOLD_NEGATIVE + 0.05f)
                {
                    thresholdElated = Constants.MAX_THRESHOLD_NEGATIVE + 0.05f;
                    return;
                }
                else thresholdElated = value;
            }
        }
        [JsonIgnore] public float ThresholdWarning
        {
            get { return thresholdWarning; }
            set
            {
                if (value > Constants.MAX_THRESHOLD_NEGATIVE)
                {
                    thresholdWarning = Constants.MAX_THRESHOLD_NEGATIVE;
                    return;
                }
                else if (value < Constants.MIN_THRESHOLD_NEGATIVE + 0.05f)
                {
                    thresholdWarning = Constants.MIN_THRESHOLD_NEGATIVE + 0.05f;
                    return;
                }
                else thresholdWarning = value;
            }
        }
        [JsonIgnore] public float ThresholdCritical
        {
            get { return thresholdCritical; }
            set
            {
                if (value > Constants.MAX_THRESHOLD_NEGATIVE - 0.05f)
                {
                    thresholdCritical = Constants.MAX_THRESHOLD_NEGATIVE - 0.05f;
                    return;
                }
                else if (value < Constants.MIN_THRESHOLD_NEGATIVE)
                {
                    thresholdCritical = Constants.MIN_THRESHOLD_NEGATIVE;
                    return;
                }
                else thresholdCritical = value;
            }
        }
        public void ResetThresholds()
        {
            ThresholdElated = DefaultThresholdElated;
            ThresholdWarning = DefaultThresholdWarning;
            ThresholdCritical = DefaultThresholdCritical;
        }

        [JsonIgnore] public NeedStates NeedState { get { return needState; } private set { needState = value; } }



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

            ThresholdElated = DefaultThresholdElated;
            ThresholdWarning = DefaultThresholdWarning;
            ThresholdCritical = DefaultThresholdCritical;

            // Hacky UI shit.
            needNameForUI = NeedSO.NeedName;
        }

        [JsonConstructor]
        public BasicNeed(BasicNeedSO needSO, float localAiPriorityWeighting, float levelFull, float levelCurrent, float baseChangeRate, float currentChangeRate, float currentChangeRateScaled, float thresholdElated, float thresholdWarning, float thresholdCritical, NeedStates needState)
        {
            this.needSO = needSO;

            LocalAiPriorityWeighting = localAiPriorityWeighting;

            LevelFull = levelFull;
            LevelCurrent = levelCurrent;

            BaseChangeRate = baseChangeRate;
            CurrentChangeRate = currentChangeRate;
            //this.currentChangeRateScaled = currentChangeRateScaled;

            ThresholdElated = thresholdElated;
            ThresholdWarning = thresholdWarning;
            ThresholdCritical = thresholdCritical;

            NeedState = needState;

            // Hacky UI shit.
            needNameForUI = NeedSO.NeedName;
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
        public void ModulateNeed(Person sourceBasePerson)
        {
            // Run the modulators.

            // TODO: Move to their own function?
            // Will we need to add arithmetic from bool?

            // AI weighting.
            foreach (ChecksAndMods.ModulatorArithmeticFromFloat modulator in needSO.BaseAIWeightingModulatedBy)
            {
                LocalAiPriorityWeighting = modulator.Execute(sourceBasePerson, LocalAiPriorityWeighting);
            }

            // Base change rates.
            foreach (ChecksAndMods.ModulatorArithmeticFromFloat modulator in needSO.BaseChangeRateModulatedBy)
            {
                BaseChangeRate = modulator.Execute(sourceBasePerson, BaseChangeRate);
            }

            // TODO: clean this up
            // The BaseChangeRate cannot be higher than -0.0001f
            if (BaseChangeRate > -Constants.MIN_BASE_CHANGE_RATE)
            {
                BaseChangeRate = -Constants.MIN_BASE_CHANGE_RATE;
            }
            // The BaseChangeRate cannot be lower than -0.5f
            if (BaseChangeRate < - Constants.MAX_BASE_CHANGE_RATE)
            {
                BaseChangeRate = -Constants.MAX_BASE_CHANGE_RATE;
            }

            CurrentChangeRate = BaseChangeRate;

            // Thresholds.
            foreach (ChecksAndMods.ModulatorArithmeticFromFloat modulator in needSO.ThresholdsModulatedBy)
            {
                // TODO: how to integrate ThresholdElated?
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
                    if (this.LevelCurrentAsPercentage <= this.ThresholdCritical & this.NeedState > NeedStates.Critical)
                    {
                        this.NeedState = NeedStates.Critical;
                        Debug.Log(string.Format("{0} is critically low ({1:P2})...", this.NeedName, 1 - GetFulfillmentDelta(true)));

                        OnBNReachedThreshold?.Invoke(NeedSO, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Critical);
                    }
                    else if (this.LevelCurrentAsPercentage <= this.ThresholdWarning & this.NeedState > NeedStates.Warning)
                    {
                        this.NeedState = NeedStates.Warning;
                        Debug.Log(string.Format("{0} is a little low ({1:P2})...", this.NeedName, 1 - GetFulfillmentDelta(true)));

                        OnBNReachedThreshold?.Invoke(NeedSO, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Warning);
                    }
                    else if (this.LevelCurrentAsPercentage >= this.ThresholdElated & this.NeedState != NeedStates.Elated)
                    {
                        this.NeedState = NeedStates.Elated;
                        Debug.Log(string.Format("{0} is elated ({1:P2})...", this.NeedName, 1 - GetFulfillmentDelta(true)));

                        OnBNReachedThreshold?.Invoke(NeedSO, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Elated);
                    }
                    else // Unset the Elated, Warning and Critical booleans as needed.
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
                        if (this.NeedState == NeedStates.Elated & this.LevelCurrent < this.ThresholdElated)
                        {
                            this.NeedState = NeedStates.Normal;
                            Debug.Log(string.Format("{0} is no longer elated.", this.NeedName));

                            OnBNLeftThreshold?.Invoke(NeedSO, LevelCurrent, LevelCurrentAsPercentage, NeedStates.Elated);
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
                yield return new WaitForSeconds(Constants.NEED_DECAY_INTERVAL);
            }
        }
    }
}
