using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface for objects that can be modified using one or more control arrows.
namespace Building
{
    public interface IArrowControllable
    {
        public void OnControlArrowAdjustment(ControlArrow sourceArrow, Vector2 cursorPosDelta);

        // TODO: function to return the object's bounds / new position for the arrow
        // or having OnControlArrowAdjustment return that

        // TODO: store the desired arrow orientation and delta from target as properties here instead?
    }
}

