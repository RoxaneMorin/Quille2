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
        // 


        // PROPERTIES
        public string InteractionName { get { return myInteractionSO.InteractionName; } }

        public BasicNeedSO[] AdvertisedNeeds { get { return myInteractionSO.AdvertisedNeeds; } }
        public InteractionNeedEffectSettings[] EffectedNeeds { get { return myInteractionSO.EffectedNeeds; } }
        public SerializedDictionary<BasicNeedSO, float> LocalNeedChangeRates { get { return localNeedChangeRates; } }
        public SerializedDictionary<BasicNeedSO, float> LocalMaxNeedChanges { get { return localMaxNeedChanges; } }

        public Check[] ViabilityChecks { get { return myInteractionSO.ViabilityChecks; } }
        public ModulatorArithmetic[] ScoringModulators { get { return myInteractionSO.ScoringModulators; } }


        // EVENTS
        public static event InteractionNeedAdvertisement SendInteractionNeedAdvertisement;



        // METHODS

        // - Score for person?
        // - Calculate situational need change rate?
        // - Calculate situational max need change?

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


        // TODO: Unregister upon deletion.


        // UTILITY
        public float ScoreFor(Person thisPerson)
        {
            float score = 0; // TODO: determine the base score?

            foreach (ModulatorArithmetic modulator in ScoringModulators)
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

            foreach (InteractionNeedEffectSettings needEffect in EffectedNeeds)
            {
                float currentNeedChangeRate = needEffect.DefaultNeedChangeRate;

                if (localNeedChangeRates.ContainsKey(needEffect.TargetNeed))
                {
                    currentNeedChangeRate *= localNeedChangeRates[needEffect.TargetNeed];
                }

                foreach (ModulatorArithmetic modulator in needEffect.NeedChangeRateModulatedBy)
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

            foreach (InteractionNeedEffectSettings needEffect in EffectedNeeds)
            {
                float currentMaxNeedChange = needEffect.DefaultMaxNeedChange;

                if (localMaxNeedChanges.ContainsKey(needEffect.TargetNeed))
                {
                    currentMaxNeedChange *= localNeedChangeRates[needEffect.TargetNeed];
                }

                foreach (ModulatorArithmetic modulator in needEffect.MaxNeedChangeModulatedBy)
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


        public override string ToString()
        {
            return string.Format("Interaction '{0}' on {1}.", myInteractionSO.InteractionName, gameObject.name);
        }
    }
}
