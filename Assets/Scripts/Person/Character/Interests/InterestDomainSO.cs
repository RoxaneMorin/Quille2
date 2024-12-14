using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Should it be within the Quille namespace?
namespace Quille
{
    // The ScriptableObject template of InterestDomains, used for the creation of specific domains as assets.
    // These are used to group Interests into wider categories, such as technical or creative fields/topics.


    [CreateAssetMenu(fileName = "DoI_", menuName = "Quille/Character/Interests/Domain of Interest", order = 10)]
    public class InterestDomainSO : PersonalityItemDomainSO
    {
        // PROPERTIES
        public List<InterestSO> InterestInThisDomain 
        { 
            get { return itemsInThisDomain.Cast<InterestSO>().ToList(); } 
            set { itemsInThisDomain = value.Cast<PersonalityItemSO>().ToList(); } 
        }

        // Anything else?
    }
}
