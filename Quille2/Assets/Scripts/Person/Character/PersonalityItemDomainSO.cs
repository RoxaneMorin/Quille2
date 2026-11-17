using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Quille
{
    // Base class for specific personality item domains to inherit from.
    public abstract class PersonalityItemDomainSO : ScriptableObject, IMenuSortable
    {
        // VARIABLES/PARAMS
        [SerializeField] protected string domainName = "Undefined";
        [SerializeField] protected Color domainColour;
        [SerializeField] protected Sprite domainIcon;

        [SerializeField][FormerlySerializedAs("menuSortingIndex")] protected int sortingIndex;


        // PROPERTIES
        public string DomainName { get { return domainName; } }
        public Color DomainColour { get { return domainColour; } }
        public Sprite DomainIcon { get { return domainIcon; } }

        public abstract PersonalityItemSO[] ItemsInThisDomain { get; set; }

        public int SortingIndex { get { return sortingIndex; } set { sortingIndex = value; } }



        // METHODS
        public abstract bool IsItemInDomain(PersonalityItemSO item);
        public abstract void AddToDomain(PersonalityItemSO itemToAdd);
    }
}
