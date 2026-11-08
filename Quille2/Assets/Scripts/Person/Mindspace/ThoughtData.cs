using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChecksAndMods;

namespace Quille
{
    // TODO: consider whether to name this Basic/BaseThought, Data/SO, and whether to rename other smtSO classes.
    // TODO: review the general structure and organisation of other data SOs.

    // The ScriptableObject template of basic Thoughts, used for the creation of specific thoughts as data assets.
    // These thoughs represent most of what may be on a character's mind, such as an interest, a person or a recent memory.
    [CreateAssetMenu(fileName = "Thought_", menuName = "Quille/Mindspace/Generic Thought", order = 0)]
    public class ThoughtData : ScriptableObject
    {
        // VARIABLES   
        [SerializeField] protected string itemName = "Undefined";
        [SerializeField] protected string itemDescription;
        [SerializeField] protected Sprite itemIcon;
        [SerializeField] protected Color itemColor;

        [SerializeField] protected float defaultLifespan;
        [SerializeField, SerializeReference] protected List<Modulator> lifespanModifiers;

        // Default decay speed + modifier?

        // Modifiers set OnAcquire
        // Other stuff OnAcquire?

        // Modifiers to check while active

        // Whatever stuff used OnExpire?


        // PROPERTIES
        public string ItemName { get { return itemName; } }
        public string ItemDescription { get { return itemDescription; } }
        public Sprite ItemIcon { get { return itemIcon; } }
        public Color ItemColor { get { return itemColor; } }

        public float DefaultLifespan { get { return defaultLifespan; } }
        public IEnumerable<Modulator> LifespanModifiers
        {
            get
            {
                foreach (var modifier in lifespanModifiers)
                {
                    yield return modifier;
                }
            }
        }
    }
}

