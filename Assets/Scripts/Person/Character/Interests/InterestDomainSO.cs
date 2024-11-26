using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Should it be within the Quille namespace?
namespace Quille
{
    // The ScriptableObject template of InterestDomains, used for the creation of specific domains as assets.
    // These are used to group Interests into wider categories, such as technical or creative fields/topics.


    [CreateAssetMenu(fileName = "DoI_", menuName = "Quille/Character/Interests/Domain of Interest", order = 10)]
    public class InterestDomainSO : ScriptableObject
    {
        // VARIABLES/PARAMS
        [SerializeField]
        private string domainName = "Undefined";
        public string DomainName { get { return domainName; } }

        // Description.

        // GRAPHICS
        public Color domainColour;
        public Sprite domainIcon;


        // Anything else?
        // Keep track of the interests in this domain?
        // Automatize through UI script?

        [SerializeField]
        private List<InterestSO> interestsInThisDomain;
        public List<InterestSO> InterestInThisDomain { get { return interestsInThisDomain; } set { interestsInThisDomain = value; } }
    }
}
