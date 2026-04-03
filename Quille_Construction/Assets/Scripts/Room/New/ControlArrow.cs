using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Building
{
    public class ControlArrow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        // VARIABLES
        [Header("Runtime")]
        protected bool arrowAvailable;
        protected bool arrowInUse;

        protected Vector3 prevCursorPos;
        protected Vector2 cursorPosDelta;



        // EVENTS
        public event ControlArrowDragged OnDragged;



        // METHODS
        protected void Init()
        {
            arrowAvailable = true;
        }

        // BEHAVIOUR
        protected void BeDragged()
        {
            cursorPosDelta = prevCursorPos - Input.mousePosition;
            prevCursorPos = Input.mousePosition;

            OnDragged?.Invoke(cursorPosDelta);
        }

        public void AdjustPosition(Vector3 positionDelta)
        {
            Vector3 targetPosition = transform.position;
            targetPosition += positionDelta;

            transform.position = targetPosition;
        }


        // BUILT IN
        protected void Start()
        {
            Init();
        }

        protected void Update()
        {
            if (arrowInUse)
            {
                BeDragged();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (arrowAvailable)
            {
                arrowInUse = true;
                prevCursorPos = Input.mousePosition;
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            arrowInUse = false;
        }
    }
}