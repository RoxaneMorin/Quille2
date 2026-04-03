using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IArrowControllable
{
    // ControlArrowDragged(Vector2 cursorPosDelta);

    // TODO: interface for using this arrow?
    /* Send out OnDragged, containing the mouse delta
     * Receive movement instructions for the controlled object
     */

    protected void OnControlArrowAdjustment(Vector2 adjustmentDelta);



    // Instructions for the arrow to move
}
