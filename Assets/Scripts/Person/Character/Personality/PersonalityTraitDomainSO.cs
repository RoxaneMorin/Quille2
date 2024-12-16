using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of PersonalityTraitDomains, used for the creation of specific domains as assets.
    // These are used to group PersonalityTraits into wider categories, such as "social" or "temperamental".

    [CreateAssetMenu(fileName = "TDom_", menuName = "Quille/Character/Personality Trait/Personality Trait Domain", order = 5)]
    public class PersonalityTraitDomainSO : PersonalityItemDomainSO
    {
        // VARIABLES
        [SerializeField] protected List<PersonalityTraitSO> itemsInThisDomain;


        // PROPERTIES
        public List<PersonalityTraitSO> TraitsInThisDomain
        {
            get { return itemsInThisDomain; }
            set { itemsInThisDomain = value; }
        }
        public override List<PersonalityItemSO> ItemsInThisDomain
        {
            get { return itemsInThisDomain.Cast<PersonalityItemSO>().ToList(); }
            set { itemsInThisDomain = value.Cast<PersonalityTraitSO>().ToList(); }
        }

        // Anything else?



        // METHODS
        public override bool IsItemInDomain(PersonalityItemSO item)
        {
            return itemsInThisDomain.Contains(item);
        }
        public override void AddToDomain(PersonalityItemSO itemToAdd)
        {
            itemsInThisDomain.Add((PersonalityTraitSO)itemToAdd);
        }
    }

}

