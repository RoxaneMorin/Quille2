using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille 
{
    // The ScriptableObject template of PersonalityTraits, used for the creation of specific traits as assets.
    // These traits represent personality elements a character may or may not possess, such as being anxious or loyal.


    [CreateAssetMenu(fileName = "PersonalityTrait", menuName = "Quille/Character/Personality Trait", order = 1)]
    public class PersonalityTraitSO : ForbiddablePersonalityItemSO
    {
        // VARIABLES/PARAMS 
        [SerializeField] [InspectorReadOnly] private float traitSpan = Constants_Quille.PERSONALITY_HALF_SPAN;
        [SerializeField] [InspectorReadOnly] private float trainMidpoint = Constants_Quille.PERSONALITY_HALF_SPAN / 2;

        // Favorable and defavorable personaltiy scores?
        // Categories?


        // PROPERTIES
        public float TraitSpan { get { return traitSpan; } }
        public float TrainMidpoint { get { return trainMidpoint; } }
        


        // METHODS
        public override bool IsCompatibleWithPerson(Person targetPerson)
        {
            // Check whether this SO is incompatible with any relevant aspect of the given person.
            foreach (ChecksAndMods.CheckArithmetic check in incompatiblePersonChecks)
            {
                if (check.Execute(targetPerson))
                {
                    Debug.Log(string.Format("The Trait '{1}' is forbidden to {0} due to the Check '{2}'.", targetPerson.CharIDAndCharacterName, this.itemName, check.ToString()));
                    return false;
                }
            }
            return true;
        }
    }
}

