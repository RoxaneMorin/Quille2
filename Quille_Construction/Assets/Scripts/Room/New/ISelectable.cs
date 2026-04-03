using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    // PROPERTIES
    public bool IsSelected { get; }

    // METHODS
    public void Select();
    public void Unselect();
}
