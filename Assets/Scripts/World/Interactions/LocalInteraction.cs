using AYellowpaper.SerializedCollections;
using ChecksAndMods;
using Quille;
using UnityEngine;

namespace World
{
    // TODO: convert to a purely C# object once the basic testing is done.

    //[System.Serializable]
    public class LocalInteraction : MonoBehaviour
    {
        // The local component of interactions, its instances tracking the data specific to a world object.
        // For example, that of "the cozy bed in Quill's bedroom".


        // VARIABLES/PARAMS
        [SerializeField] protected InteractionSO myInteractionSO;

        [SerializeField] [SerializedDictionary("Target Need", "Ratio (0-1) of Original")] SerializedDictionary<BasicNeedSO, float> localNeedChangeRates;
        [SerializeField] [SerializedDictionary("Target Need", "Ratio (0-1) of Original")] SerializedDictionary<BasicNeedSO, float> localMaxNeedChanges;

        // TODO: how to get/pass on room info when necessary?


        // PROPERTIES
        public string InteractionName { get { return myInteractionSO.InteractionName; } }
        public float InteractionBaseScore { get { return myInteractionSO.DefaultBaseScore; } }

        public BasicNeedSO[] AdvertisedNeeds { get { return myInteractionSO.AdvertisedNeeds; } }
        public InteractionNeedEffect[] EffectedNeeds { get { return myInteractionSO.EffectedNeeds; } }
        public SerializedDictionary<BasicNeedSO, float> LocalNeedChangeRates { get { return localNeedChangeRates; } }
        public SerializedDictionary<BasicNeedSO, float> LocalMaxNeedChanges { get { return localMaxNeedChanges; } }

        public Check[] ViabilityChecks { get { return myInteractionSO.ViabilityChecks; } }
        public Modulator[] ScoringModulators { get { return myInteractionSO.ScoringModulators; } }


        // EVENTS
        public static event InteractionNeedAdvertisement SendInteractionNeedAdvertisement;
        public static event InteractionNeedDeletion SendInteractionNeedDeletion;


        // METHODS

        // INIT
        protected void Init()
        {
            SendOutNeedAdvertisements();
        }

        protected void SendOutNeedAdvertisements()
        {
            foreach (BasicNeedSO advertisedNeed in AdvertisedNeeds)
            {
                SendInteractionNeedAdvertisement?.Invoke(advertisedNeed, this);
            }
        }
        protected void SendOutNeedAdvertisementDeletions()
        {
            foreach (BasicNeedSO advertisedNeed in AdvertisedNeeds)
            {
                SendInteractionNeedDeletion?.Invoke(advertisedNeed, this);
            }
        }


        // UTILITY
        public bool ValidateFor(Person thisPerson)
        {
            foreach (Check check in ViabilityChecks)
            {
                if (check != null)
                {
                    // Return false as soon as a validity check fails.
                    if (!check.Execute(thisPerson))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public float ScoreFor(Person thisPerson)
        {
            float score = InteractionBaseScore;

            foreach (Modulator modulator in ScoringModulators)
            {
                if (modulator != null)
                {
                    score = modulator.Execute(thisPerson, score);
                }
            }

            return score;
        }

        public SerializedDictionary<BasicNeedSO, float> CalculateNeedChangeRatesFor(Person thisPerson)
        {
            SerializedDictionary<BasicNeedSO, float> situationalNeedChangeRates = new SerializedDictionary<BasicNeedSO, float>();

            foreach (InteractionNeedEffect needEffect in EffectedNeeds)
            {
                float currentNeedChangeRate = needEffect.DefaultNeedChangeRate;

                if (localNeedChangeRates.ContainsKey(needEffect.TargetNeed))
                {
                    currentNeedChangeRate *= localNeedChangeRates[needEffect.TargetNeed];
                }

                foreach (Modulator modulator in needEffect.NeedChangeRateModulatedBy)
                {
                    currentNeedChangeRate = modulator.Execute(thisPerson, currentNeedChangeRate);
                }

                situationalNeedChangeRates.Add(needEffect.TargetNeed, currentNeedChangeRate);
            }

            return situationalNeedChangeRates;
        }

        public SerializedDictionary<BasicNeedSO, float> CalculateMaxNeedChangesFor(Person thisPerson)
        {
            SerializedDictionary<BasicNeedSO, float> situationalMaxNeedChanges = new SerializedDictionary<BasicNeedSO, float>();

            foreach (InteractionNeedEffect needEffect in EffectedNeeds)
            {
                float currentMaxNeedChange = needEffect.DefaultMaxNeedChange;

                if (localMaxNeedChanges.ContainsKey(needEffect.TargetNeed))
                {
                    currentMaxNeedChange *= localNeedChangeRates[needEffect.TargetNeed];
                }

                foreach (Modulator modulator in needEffect.MaxNeedChangeModulatedBy)
                {
                    currentMaxNeedChange = modulator.Execute(thisPerson, currentMaxNeedChange);
                }

                situationalMaxNeedChanges.Add(needEffect.TargetNeed, currentMaxNeedChange);
            }

            return situationalMaxNeedChanges;
        }


        // BUILT IN
        private void Start()
        {
            Init();
        }

        private void OnDestroy()
        {
            SendOutNeedAdvertisementDeletions();
        }


        public override string ToString()
        {
            return string.Format("Interaction '{0}' on {1}", myInteractionSO.InteractionName, gameObject.name);
        }
    }
}
