using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Class englobing the base fields founds in data objects of all kinds.
public abstract class ItemData : ScriptableObject, IMenuSortable
{
    // VARIABLES   
    [SerializeField] protected string itemName = "Undefined";
    [SerializeField, TextArea] protected string itemDescription;
    [SerializeField] protected Sprite itemIcon;
    [SerializeField] protected Color itemColour; // do we need this?

    [SerializeField][FormerlySerializedAs("menuSortingIndex")] protected int sortingIndex;
    


    // PROPERTIES
    public string ItemName { get { return itemName; } }
    public string ItemDescription { get { return itemDescription; } }
    public Sprite ItemIcon { get { return itemIcon; } }
    public Color ItemColour { get { return itemColour; } }

    public int SortingIndex { get { return sortingIndex; } set { sortingIndex = value; } }
}
