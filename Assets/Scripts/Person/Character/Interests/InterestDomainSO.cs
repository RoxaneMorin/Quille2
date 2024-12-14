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
        [SerializeField] protected List<InterestSO> itemsInThisDomain;


        // PROPERTIES
        public List<InterestSO> InterestInThisDomain
        {
            get { return itemsInThisDomain; }
            set { itemsInThisDomain = value; }
        }
        public override List<PersonalityItemSO> ItemsInThisDomain
        { 
            get { return itemsInThisDomain.Cast<PersonalityItemSO>().ToList(); } 
            set { itemsInThisDomain = value.Cast<InterestSO>().ToList(); } 
        }

        // Anything else?



        // METHODS
        public override void AddToDomain(PersonalityItemSO itemToAdd)
        {
            itemsInThisDomain.Add((InterestSO)itemToAdd);
        }
    }
}
