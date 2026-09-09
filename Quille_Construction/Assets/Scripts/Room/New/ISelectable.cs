using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Interfaces for objects selectable in communication with managers and the like.
public interface ISelectable
{
    // PROPERTIES
    public bool IsSelected { get; }

    // METHODS
    public void Select();
    public void Unselect();
}
