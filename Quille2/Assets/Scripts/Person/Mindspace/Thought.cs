using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChecksAndMods;

namespace Quille
{
    // TODO: decide whether this should be a C# object or a scriptable object.

    // The C# object component of a Thought. Instances derive their specificities from their associated ThoughtData instance.
    // These thoughs represent most of what may be on a character's mind, such as an interest, a person, an event or a memory.
    [System.Serializable]
    public class Thought
    {
        // VARIABLES
        [SerializeField] protected ThoughtData data;

        [SerializeField] protected float lifespan;
        [SerializeField] protected float currentAge;

        // Decay speed?

        // Target?


        // PROPERTIES
        public ThoughtData Data { get { return data; } } // Do we need this since everything else is accessible?
        public string ItemName { get { return data.ItemName; } }
        public string ItemDescription { get { return data.ItemDescription; } }
        public Sprite ItemIcon { get { return data.ItemIcon; } }
        public Color ItemColor { get { return data.ItemColor; } }

        public float Lifespan { get { return lifespan; } }
        public float CurrentAge { get { return currentAge; } }

        // Decay rate?



        // METHODS

        //protected void OnAcquire() { } :
        // Init for the given person, calculate thought lifespan, other stuff if needed. Send up an event?


        //protected void OnExpire() { } 
        // Probably just send up an event?


        protected void CalculateLifespan(Person thoughtOwner)
        {
            lifespan = data.DefaultLifespan;
            foreach (Modulator modulator in data.LifespanModifiers)
            {
                lifespan = modulator.Execute(thoughtOwner, lifespan);
            }

            // If lifespan <= 0, kill the thought.
        }


        // Some way to tick up its current age?
        
    }

}
