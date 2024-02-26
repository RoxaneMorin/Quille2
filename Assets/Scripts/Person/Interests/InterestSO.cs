using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    [CreateAssetMenu(fileName = "Interest_", menuName = "Quille/Interests/Field of Interest", order = 1)]
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
        private InterestDomainSO[] inDomains;
        // To do: how to ensure every interest is in at least one domain?
        // Add UI warning if empty?

        // RELATED INTERESTS
        [SerializeField]
        private InterestSO[] relatedInterests;

        // MISC
        // Should we have related personality axes or traits?
        // Where should related actions/interactions be handled?
    }
}