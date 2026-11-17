using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// 
public interface IMenuSortable : IComparable<IMenuSortable>
{
    public int SortingIndex { get; set; }

    int IComparable<IMenuSortable>.CompareTo(IMenuSortable? other)
    {
        if (other == null)
        {
            return 1;
        }

        // TODO: verify they are of the same actual type?
        //Debug.Log(this);
        //Debug.Log(other);

        return this.SortingIndex.CompareTo(other.SortingIndex);
    }
}

public abstract class MenuSortableScriptableObject : ScriptableObject, IMenuSortable
{
    // VARIALBE & PROPERTY
    [SerializeField][FormerlySerializedAs("menuSortingIndex")] protected int sortingIndex;
    public int SortingIndex { get { return sortingIndex; } set { sortingIndex = value; } }
}


