using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace QuilleUI
{
    public class SliderRadial : Slider
    {
        // ENUMS
        public enum Shape
        {
            Quarter90,
            Half180,
            Full360,
        }
        public enum Origin
        {
            Botton,
            Left,
            Top,
            Right
        }
        new public enum Direction
        {
            Clockwise,
            Counterclockwise
        }


        // VARIABLES
        [SerializeField] private Shape m_Shape = Shape.Quarter90;
        [SerializeField] private Origin m_Origin = Origin.Botton;
        [SerializeField] private Direction m_Direction = Direction.Clockwise;


        // PROPERTIES
        new public Direction direction { get { return m_Direction; } }
        bool reverseValue { get { return m_Direction == Direction.Counterclockwise; } }




        //void UpdateDrag(PointerEventData eventData, Camera cam)
        //{
        //     The RectTransform of the radial slider (this could be the base circle or container of the radial slider)
        //    RectTransform radialRect = m_HandleContainerRect;

        //    if (radialRect != null)
        //    {
        //         Get the mouse position
        //        Vector2 position;
        //        if (!MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
        //            return;

        //        Vector2 localCursor;
        //        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(radialRect, position, cam, out localCursor))
        //            return;

        //         Calculate the angle relative to the center of the circle
        //        Vector2 center = radialRect.rect.center;
        //        Vector2 direction = localCursor - center;

        //        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //         Normalize angle to [0, 360] range
        //        if (angle < 0)
        //            angle += 360;

        //         Map angle to a [0, 1] range (assuming the radial slider represents a full circle)
        //        float normalizedAngle = angle / 360f;

        //         Set the normalized value, optionally reversing
        //        normalizedValue = reverseValue ? 1f - normalizedAngle : normalizedAngle;
        //    }
        //}
    }
}
