using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // Base class for specific personality item domains to inherit from.
    public abstract class PersonalityItemDomainSO : MenuSortableScriptableObject
    {
        // VARIABLES/PARAMS
        [SerializeField] protected string domainName = "Undefined";
        [SerializeField] protected Color domainColour;
        [SerializeField] protected Sprite domainIcon;


        // PROPERTIES
        public string DomainName { get { return domainName; } }
        public Color DomainColour { get { return domainColour; } }
        public Sprite DomainIcon { get { return domainIcon; } }

        public abstract PersonalityItemSO[] ItemsInThisDomain { get; set; }



        // METHODS
        public abstract bool IsItemInDomain(PersonalityItemSO item);
        public abstract void AddToDomain(PersonalityItemSO itemToAdd);
    }
}
