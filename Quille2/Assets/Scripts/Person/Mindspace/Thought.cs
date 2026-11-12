using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChecksAndMods;

namespace Quille
{
    // The runtime component of a Thought. Instances derive their specificities from their associated ThoughtData instance.
    // These thoughs represent most of what may be on a character's mind, such as an interest, a person, an event or a memory.
    [System.Serializable]
    public class Thought : ScriptableObject
    {
        // VARIABLES
        [SerializeField] protected ThoughtData data;

        [SerializeField] protected float totalLifespan;
        [SerializeField] protected float remainingLifespan;
        [SerializeField] protected float decayRate;

        // Target?


        // PROPERTIES
        public ThoughtData Data { get { return data; } } // Do we need this since everything else is accessible?
        public string ItemName { get { return data.ItemName; } }
        public string ItemDescription { get { return data.ItemDescription; } }
        public Sprite ItemIcon { get { return data.ItemIcon; } }
        public Color ItemColor { get { return data.ItemColor; } }

        public float TotalLifespan { get { return totalLifespan; } }
        public float CurrentLifespan { get { return remainingLifespan; } }
        public float CurrentAge { get { return totalLifespan - remainingLifespan; } }
        public float DecayRate { get { return decayRate; } }
        public bool IsTemporary { get { return data.IsTemporary; } }



        // EVENTS
        public event ThoughtAcquired OnThoughtAcquired;
        public event ThoughtExpired OnThoughtExpired;



        // METHODS
        // Initialization
        public void Init(ThoughtData data, float lifespan, float decayRate)
        {
            this.data = data;
            this.name = $"Thought_{ItemName}";

            this.totalLifespan = this.remainingLifespan = lifespan;
            this.decayRate = decayRate;

            // Etc.

            OnThoughtAcquired?.Invoke(data, this);
        }
        public void InitFor(Person thinker, ThoughtData data, float lifespan)
        {
            Init(data, lifespan, data.CalculateDecayRateFor(thinker));
        }
        public void InitFor(Person thinker, ThoughtData data)
        {
            float calculatedLifespan;
            if (!data.CheckValidityFor(thinker, out calculatedLifespan))
            {
                Debug.LogWarning($"The thought '{ItemName}' is  being initialized for '{thinker.CharIDAndCharacterName}', but is not valid for them.");
            }
            InitFor(thinker, data, calculatedLifespan);
        }


        // Management
        public void ResetLifespan()
        {
            remainingLifespan = totalLifespan;
        }

        // Alter or recalculate rate and lifespan


        // Lifecycle
        protected void Expire()
        {
            OnThoughtExpired?.Invoke(data, this);
        }

        public IEnumerator AgeByDecayRate()
        {
            if (decayRate > 0.0f)
            {
                remainingLifespan -= decayRate;
                if (remainingLifespan <= 0)
                {
                    Expire();
                }

                yield return new WaitForSeconds(Constants_Quille.NEED_DECAY_INTERVAL);
            }
        }
    }

}
