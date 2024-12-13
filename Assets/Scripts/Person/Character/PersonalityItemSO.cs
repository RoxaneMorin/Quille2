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
    }

    // As above, but may also be incompatible with certain checks.
    public abstract class ForbiddablePersonalityItemSO : PersonalityItemSO
    {
        // VARIABLES/PARAMS 
        [SerializeField] protected ChecksAndMods.CheckArithmetic[] incompatiblePersonChecks;



        // METHODS
        public abstract bool IsCompatibleWithPerson(Person targetPerson);
    }
}

