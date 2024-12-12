using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille 
{
    // The ScriptableObject template of PersonalityTraits, used for the creation of specific traits as assets.
    // These traits represent personality elements a character may or may not possess, such as being anxious or loyal.


    [CreateAssetMenu(fileName = "PersonalityTrait", menuName = "Quille/Character/Personality Trait", order = 1)]
    public class PersonalityTraitSO : ScriptableObject
    {
        // VARIABLES/PARAMS 
        [SerializeField]
        private string traitName = "Undefined";
        public string TraitName { get { return traitName; } }

        // Description.

        // TRAIT GRAPHICS
        public Sprite traitIcon;

        // OTHER VALUES
        // The intensity of traits is either 0.5 or 1.
        [SerializeField] [InspectorReadOnly] private float traitSpan = Constants_Quille.PERSONALITY_HALF_SPAN;
        public float TraitSpan { get { return traitSpan; } }

        [SerializeField] [InspectorReadOnly] private float trainMidpoint = Constants_Quille.PERSONALITY_HALF_SPAN / 2;
        public float TrainMidpoint { get { return trainMidpoint; } }

        // INCOMPATIBILITIES
        [SerializeField]private ChecksAndMods.CheckArithmetic[] incompatiblePersonalityScores;
        // Remove the separate incompatible traits category as we can now check for a full or half trait.

        // FAVORABLE AND DEFAVORABLE PERSONALITY SCORES?

        // CATEGORIES?



        // METHODS
        public bool ForbiddenToPerson(Person targetPerson)
        {
            // Check whether this SO is incompatible with any relevant aspect of the given person.

            foreach (ChecksAndMods.CheckArithmetic check in incompatiblePersonalityScores)
            {
                if (check.Execute(targetPerson))
                {
                    Debug.Log(string.Format("{0} cannot have the Trait '{1}' due to the Check '{2}'.", targetPerson.CharIDAndCharacterName, this.traitName, check.ToString()));
                    return true;
                }
            }
            return false;
        }
    }
}

