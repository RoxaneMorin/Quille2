using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // Base class for specific personality item domains to inherit from.
    public abstract class PersonalityItemDomainSO : ScriptableObject
    {
        // VARIABLES/PARAMS
        [SerializeField] protected string domainName = "Undefined";
        [SerializeField] protected Color domainColour;
        [SerializeField] protected Sprite domainIcon;
        [SerializeField] protected int menuSortingIndex;

        [SerializeField] protected List<PersonalityItemSO> itemsInThisDomain;


        // PROPERTIES
        public string DomainName { get { return domainName; } }
        public Color DomainColour { get { return domainColour; } }
        public Sprite DomainIcon { get { return domainIcon; } }
        public int MenuSortingIndex { get { return menuSortingIndex; } }

        public List<PersonalityItemSO> ItemsInThisDomain { get { return itemsInThisDomain; } set { itemsInThisDomain = value; } }
    }
}
