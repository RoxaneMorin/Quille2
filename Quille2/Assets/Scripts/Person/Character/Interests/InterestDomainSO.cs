using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Should it be within the Quille namespace?
namespace Quille
{
    // The ScriptableObject template of InterestDomains, used for the creation of specific domains as assets.
    // These are used to group Interests into wider categories, such as technical or creative fields/topics.


    [CreateAssetMenu(fileName = "IDom_", menuName = "Quille/Character/Interests/Interest Domain", order = 5)]
    public class InterestDomainSO : PersonalityItemDomainSO
    {
        // VARIABLES
        [SerializeField] protected InterestSO[] itemsInThisDomain;


        // PROPERTIES
        public InterestSO[] InterestInThisDomain
        {
            get { return itemsInThisDomain; }
            set { itemsInThisDomain = value; }
        }
        public override PersonalityItemSO[] ItemsInThisDomain
        { 
            get { return itemsInThisDomain.Cast<PersonalityItemSO>().ToArray(); } 
            set { itemsInThisDomain = value.Cast<InterestSO>().ToArray(); } 
        }

        // Anything else?



        // METHODS
        public override bool IsItemInDomain(PersonalityItemSO item)
        {
            return itemsInThisDomain.Contains(item);
        }
        public override void AddToDomain(PersonalityItemSO itemToAdd)
        {
            itemsInThisDomain.Append((InterestSO)itemToAdd);
        }
    }
}
