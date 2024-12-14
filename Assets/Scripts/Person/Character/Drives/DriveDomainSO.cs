using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of DriveDomains, used for the creation of specific domains as assets.
    // These are used to group Drives into wider categories, such as "social" and "material".


    [CreateAssetMenu(fileName = "DDom_", menuName = "Quille/Character/Drives/Drive Domain", order = 5)]
    public class DriveDomainSO : PersonalityItemDomainSO
    {
        // VARIABLES
        [SerializeField] protected List<DriveSO> itemsInThisDomain;


        // PROPERTIES
        public List<DriveSO> DrivesInThisDomain
        {
            get { return itemsInThisDomain; }
            set { itemsInThisDomain = value; }
        }
        public override List<PersonalityItemSO> ItemsInThisDomain
        {
            get { return itemsInThisDomain.Cast<PersonalityItemSO>().ToList(); }
            set { itemsInThisDomain = value.Cast<DriveSO>().ToList(); }
        }

        // Anything else?



        // METHODS
        public override void AddToDomain(PersonalityItemSO itemToAdd)
        {
            itemsInThisDomain.Add((DriveSO)itemToAdd);
        }
    }
}
