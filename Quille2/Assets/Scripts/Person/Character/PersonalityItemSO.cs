using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // A base class for specific types of personality elements to inherit from.
    public abstract class PersonalityItemSO : ItemData
    {
        // TODO: determine if this actually needs to exist.
        // TODO: Add Randomization Weights, what is favorable or defavorable to getting this item?
    }


    // As above, but may also be incompatible with certain checks.
    public abstract class ForbiddablePersonalityItemSO : PersonalityItemSO
    {
        // VARIABLES/PARAMS 
        [SerializeReference][PopulateCheckSubtypes] protected ChecksAndMods.Check[] incompatiblePersonChecks;



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

