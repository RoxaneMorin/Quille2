using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSortableScriptableObject : ScriptableObject, IComparable
{
    // VARIALBE & PROPERTY
    [SerializeField] protected int menuSortingIndex;
    public int MenuSortingIndex { get { return menuSortingIndex; } }


    // METHODS
    public int CompareTo(object obj)
    {
        if (obj == null)
        {
            return 1;
        }

        MenuSortableScriptableObject otherSO = obj as MenuSortableScriptableObject;
        if (otherSO != null)
        {
            return this.MenuSortingIndex.CompareTo(otherSO.MenuSortingIndex);
        }
        else
        {
            throw new ArgumentException(string.Format("{0} is not a MenuSortableScriptableObject.", obj));
        }
    }
}

// Interface doing the same thing just in case.
public interface IMenuSortable : IComparable
{
    public int MenuSortingIndex { get; set; }
}
