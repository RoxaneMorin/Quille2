using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // A base class for specific types of personality elements to inherit from.
    public abstract class PersonalityItemSO : ScriptableObject
    {
        // VARIABLES/PARAMS 
        [SerializeField] protected string itemName = "Undefined";
        [SerializeField] protected Sprite itemIcon;
        [SerializeField] protected int menuSortingIndex;

        // PROPERTIES
        public string ItemName { get { return itemName; } }
        public Sprite ItemIcon { get { return itemIcon; } }
        public int MenuSortingIndex { get { return menuSortingIndex; } }


        // TODO: Add Randomization Weights, what is favorable or defavorable to getting this item.
    }

    // As above, but may also be incompatible with certain checks.
    public abstract class ForbiddablePersonalityItemSO : PersonalityItemSO
    {
        // TODO: transform this into an interface?

        // VARIABLES/PARAMS 
        [SerializeField] protected ChecksAndMods.CheckArithmetic[] incompatiblePersonChecks;



        // METHODS
        public abstract bool IsCompatibleWithPerson(Person targetPerson);
    }


    // Interface to ease the manipulation of items that use domains.
    public interface IUseDomains
    {
        // PROPERTIES
        public string ItemName { get; }
        public PersonalityItemDomainSO[] InDomains { get; set; }



        // METHODS
        public bool InDomain(PersonalityItemDomainSO domain);
        public void AddDomain(PersonalityItemDomainSO newDomain);
    }
}

