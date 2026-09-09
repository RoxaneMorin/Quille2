using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface for objects that can be modified using one or more control arrows.
namespace Building
{
    public interface IArrowControllable
    {
        public void OnControlArrowAdjustment(ControlArrow sourceArrow, Vector2 cursorPosDelta);

        // TODO: separate function to return a message to the arrow?
        // Event the arrow listens to when it should change position?
    }
}

