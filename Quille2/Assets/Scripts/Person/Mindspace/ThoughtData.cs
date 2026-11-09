using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChecksAndMods;
using QuilleUI;

namespace Quille
{
    // TODO: review the general structure, naming scheme and organisation of other data SOs.

    // The data component of Thoughts, used for the creation of specific thoughts as assets.
    // These thoughs represent most of what may be on a character's mind, such as an interest, a person or a recent memory.
    [CreateAssetMenu(fileName = "Thought_", menuName = "Quille/Mindspace/Generic Thought", order = 0)]
    public class ThoughtData : ScriptableObject
    {
        // VARIABLES   
        [SerializeField] protected string itemName = "Undefined";
        [SerializeField] protected string itemDescription;
        [SerializeField] protected Sprite itemIcon;
        [SerializeField] protected Color itemColor;

        // Vibe, positive vs negative, pleasant or unpleasant thought to have?

        [SerializeReference] protected List<Check> validityChecks;

        [SerializeField] protected float defaultLifespan;
        [SerializeReference] protected List<Modulator> lifespanModifiers;

        [SerializeField] protected float defaultDecayRate;
        [SerializeReference] protected List<Modulator> decayRateModifiers;

        // Modifiers set on acquisition?
        // Modifiers to check while active?
        // Whatever may happens on expire?



        // PROPERTIES
        public string ItemName { get { return itemName; } }
        public string ItemDescription { get { return itemDescription; } }
        public Sprite ItemIcon { get { return itemIcon; } }
        public Color ItemColor { get { return itemColor; } }

        public IEnumerable<Check> ValidityChecks
        {
            get
            {
                foreach (Check check in validityChecks)
                {
                    yield return check;
                }
            }
        }
        public float DefaultLifespan { get { return defaultLifespan; } }
        public IEnumerable<Modulator> LifespanModifiers
        {
            get
            {
                foreach (Modulator modulator in lifespanModifiers)
                {
                    yield return modulator;
                }
            }
        }
        public float DefaultDecayRate { get { return defaultDecayRate; } }
        public bool ShouldDecay { get { return defaultDecayRate > 0.0f; } }
        public IEnumerable<Modulator> DecayRateModifiers
        {
            get
            {
                foreach (Modulator modulator in decayRateModifiers)
                {
                    yield return modulator;
                }
            }
        }



        // METHODS
        public bool CheckValidityFor(Person potentialThinker, out float lifespanIfValid)
        {
            foreach (Check check in validityChecks)
            {
                if (!check.Execute(potentialThinker))
                {
                    lifespanIfValid = 0f;
                    return false;
                }
            }

            lifespanIfValid = CalculateLifespanFor(potentialThinker);
            if (lifespanIfValid <= 0)
            {
                return false;
            }

            return true;
        }

        public float CalculateLifespanFor(Person thinker)
        {
            float lifespan = defaultLifespan;
            foreach (Modulator modulator in lifespanModifiers)
            {
                lifespan = modulator.Execute(thinker, lifespan);
            }
            return lifespan;
        }

        public float CalculateDecayRateFor(Person thinker)
        {
            if (defaultDecayRate <= 0.0f)
            {
                return 0.0f; // Non-decaying thought.
            }

            float decayRate = defaultDecayRate;
            foreach (Modulator modulator in decayRateModifiers)
            {
                decayRate = modulator.Execute(thinker, decayRate);
            }
            return decayRate;
        }
    }
}

