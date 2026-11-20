using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChecksAndMods;
using UnityEngine.UIElements.Experimental;

namespace Quille
{
    // The data component of Thoughts, used for the creation of specific thoughts as assets.
    // These thoughs represent most of what may be on a character's mind, such as an interest, a person or a recent memory.
    [CreateAssetMenu(fileName = "Thought_", menuName = "Quille/Mindspace/Basic Thought", order = 0)]
    public class ThoughtData : ItemData
    {
        // VARIABLES   
        [SerializeField] protected Color itemColour;

        // Vibe, positive vs negative, pleasant or unpleasant thought to have?

        [SerializeReference, PopulateSubtypes] protected List<Check> validityChecks;

        [SerializeField] bool isTemporary = true;

        [SerializeField] protected float defaultLifespan;
        [SerializeReference, PopulateSubtypes] protected List<Modulator> lifespanModifiers;

        // Modifiers set on acquisition?
        // Modifiers to check while active?
        // Whatever may happens on expire?



        // PROPERTIES
        public Color ThoughtColour { get { return itemColour; } }
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
        public bool IsTemporary { get { return isTemporary; } }
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



        // METHODS


        public bool CheckValidityFor(Person potentialThinker)
        {
            if (validityChecks != null)
            {
                foreach(Check check in validityChecks)
                {
                    if (!check.Execute(potentialThinker))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool CheckValidityFor(Person potentialThinker, out float lifespanIfValid)
        {
            lifespanIfValid = 0.0f;

            if (CheckValidityFor(potentialThinker))
            {
                return !isTemporary || (lifespanIfValid = CalculateLifespanFor(potentialThinker)) > 0;
            }
            else
            {
                return false;
            }
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

        public Thought TryCreateThoughtFor(Person potentialThinker)
        {
            float potentialLifespan;
            if (CheckValidityFor(potentialThinker, out potentialLifespan))
            {
                Thought newThought = ScriptableObject.CreateInstance<Thought>();
                newThought.Init(this, potentialLifespan);

                return newThought;
            }
            else
            {
                return null;
            }
        }
    }
}

