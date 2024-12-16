using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of Interests, used for the creation of specific interests as assets.
    // These interests represent topics a character may drawn to (or repulsed by), such as art, science or the economy.


    [CreateAssetMenu(fileName = "Interest_", menuName = "Quille/Character/Interests/Interest", order = 1)]
    public class InterestSO : PersonalityItemSO, IUseDomains
    {
        // VARIABLES/PARAMS
        [BeginInspectorReadOnlyGroup]
        [SerializeField] private floatPair interestSpan = (-Constants_Quille.INTEREST_HALF_SPAN, Constants_Quille.INTEREST_HALF_SPAN); // The span from hatred to passion for this interest.
        [EndInspectorReadOnlyGroup]

        [SerializeField] private List<InterestDomainSO> inDomains; // TODO: how to ensure every interest is in at least one domain? Add UI warning if empty?
        [SerializeField] private List<InterestSO> relatedInterests;

        // Should we have related personality axes or traits?
        // Where should related actions/interactions be handled?


        // PROPERTIES
        public floatPair InterestSpan { get { return interestSpan; } }
        public List<InterestDomainSO> InInterestDomains { get { return inDomains; ; } set { inDomains = value; } }
        public List<PersonalityItemDomainSO> InDomains { get { return inDomains.Cast<PersonalityItemDomainSO>().ToList(); ; } set { inDomains = value.Cast<InterestDomainSO>().ToList(); } }
        public List<InterestSO> RelatedInterests { get { return relatedInterests; } set { relatedInterests = value; } }


        // METHODS
        public bool InDomain(PersonalityItemDomainSO domain)
        {
            return inDomains.Contains(domain);
        }
        public void AddDomain(PersonalityItemDomainSO newDomain)
        {
            inDomains.Add((InterestDomainSO) newDomain);
        }
    }
}