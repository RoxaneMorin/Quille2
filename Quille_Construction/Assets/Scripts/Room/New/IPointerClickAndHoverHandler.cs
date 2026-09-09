using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// Wrapper interface combining pointer click and hover (enter + exit) handlers.
public interface IPointerClickAndHoverHandler : IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
}
