using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Building
{
    // The delegates used by events through the Building namespace.
    // They are regrouped here for ease of editability.


    // WALL EVENTS
    // A wall anchor has been clicked.
    public delegate void WallAnchorClicked(WallAnchor targetAnchor, PointerEventData.InputButton clickType);
    // A new wall anchor has been selected by the wall manager.
    public delegate void WallAnchorSelected(WallAnchor selectedAnchor);
}