using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of Interests, used for the creation of specific interests as assets.
    // These interests represent topics a character may drawn to (or repulsed by), such as art, science or the economy.


    [CreateAssetMenu(fileName = "Interest_", menuName = "Quille/Character/Interests/Interest", order = 1)]
    public class InterestSO : ScriptableObject
    {
        // VARIABLES/PARAMS
        [SerializeField]
        private string interestName = "Undefined";
        public string InterestName { get { return interestName; } }

        // Description.

        // GRAPHICS
        public Sprite interestIcon;

        // OTHER VALUES
        [BeginInspectorReadOnlyGroup]
        [SerializeField]
        private floatPair interestSpan = (-Constants.INTEREST_HALF_SPAN, Constants.INTEREST_HALF_SPAN); // The span from hatred to passion for this interest.
        public floatPair AxeSpan { get { return interestSpan; } }

        [EndInspectorReadOnlyGroup]

        // IN DOMAINS
        [SerializeField]
        private List<InterestDomainSO> inDomains;
        public List<InterestDomainSO> InDomains { get { return inDomains; } set { inDomains = value; } }
        // To do: how to ensure every interest is in at least one domain?
        // Add UI warning if empty?

        // RELATED INTERESTS
        [SerializeField]
        private List<InterestSO> relatedInterests;
        public List<InterestSO> RelatedInterests { get { return relatedInterests; } set { relatedInterests = value; } }

        // MISC
        // Should we have related personality axes or traits?
        // Where should related actions/interactions be handled?
    }
}