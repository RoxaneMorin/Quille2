using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Should it be within the Quille namespace?
namespace Quille
{
    [CreateAssetMenu(fileName = "DoI_", menuName = "Quille/Interests/Domain of Interest", order = 10)]
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