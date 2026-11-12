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
        [SerializeField, TextArea] protected string itemDescription;
        [SerializeField] protected Sprite itemIcon;
        [SerializeField] protected Color itemColor;

        // Vibe, positive vs negative, pleasant or unpleasant thought to have?

        [SerializeField] bool isTemporary = true;
        [SerializeReference, PopulateSubtypes] protected List<Check> validityChecks;
        [SerializeField] protected float defaultLifespan;
        [SerializeReference, PopulateSubtypes] protected List<Modulator> lifespanModifiers;
        [SerializeField] protected float defaultDecayRate;
        [SerializeReference, PopulateSubtypes] protected Modulator[] decayRateModifiers;

        // Having both a customisable lifespan and decay rate is probably redundant.

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
        public bool IsTemporary { get { return isTemporary && defaultDecayRate > 0.0f; } }
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

            if (isTemporary)
            {
                lifespanIfValid = CalculateLifespanFor(potentialThinker);
                if (lifespanIfValid <= 0)
                {
                    return false;
                }
            }
            else
            {
                lifespanIfValid = 0.0f;
            }

            return true;
        }

        public float CalculateLifespanFor(Person thinker)
        {
            if (isTemporary && defaultLifespan > 0.0f)
            {
                float lifespan = defaultLifespan;
                foreach (Modulator modulator in lifespanModifiers)
                {
                    lifespan = modulator.Execute(thinker, lifespan);
                }
                return lifespan;
            }
            else
            {
                return 0.0f; // Non-decaying thought.
            }
        }

        public float CalculateDecayRateFor(Person thinker)
        {
            if (isTemporary && defaultDecayRate > 0.0f)
            {
                float decayRate = defaultDecayRate;
                foreach (Modulator modulator in decayRateModifiers)
                {
                    decayRate = modulator.Execute(thinker, decayRate);
                }
                return decayRate;
            }
            else
            {
                return 0.0f; // Non-decaying thought.
            }
        }


        public Thought TryCreateThoughtFor(Person potentialThinker)
        {
            float potentialLifespan;
            if (CheckValidityFor(potentialThinker, out potentialLifespan))
            {
                Thought newThought = ScriptableObject.CreateInstance<Thought>();
                newThought.InitFor(potentialThinker, this, potentialLifespan);

                return newThought;
            }
            else
            {
                return null;
            }
        }
    }
}

