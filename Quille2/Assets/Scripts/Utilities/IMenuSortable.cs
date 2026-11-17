using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Interfaces marking objects as being sortable in editor and in-game menus.
public interface IMenuSortable : IComparable<IMenuSortable>
{
    public int SortingIndex { get; set; }

    int IComparable<IMenuSortable>.CompareTo(IMenuSortable? other)
    {
        if (other == null || this.GetType() != other.GetType())
        {
            return 1;
        }

        return this.SortingIndex.CompareTo(other.SortingIndex);
    }
}