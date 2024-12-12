using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
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

        // VARIABLES
        [SerializeField] protected RectTransform m_RootRect;
        [SerializeField] protected Vector2 m_OriginalWidthAndHeight;
        [SerializeField] protected RectTransform m_BackgroundRect;
        [SerializeField] protected Image m_BackgroundImage;

        [SerializeField] protected Sprite m_BackgroundSprite360;
        [SerializeField] protected Sprite m_FillSprite360;
        [SerializeField] protected Sprite[] m_BackgroundSprite180;
        [SerializeField] protected Sprite[] m_FillSprite180;
        [SerializeField] protected Sprite[] m_BackgroundSprite90;
        [SerializeField] protected Sprite[] m_FillSprite90;

        [SerializeField] private Shape m_Shape = Shape.Full360;
        private Image.Origin90 m_Origin90;
        private Image.Origin180 m_Origin180;
        private Image.Origin360 m_Origin360;
        [SerializeField] private int m_Origin = 0;
        [SerializeField] private bool m_Clockwise = true;
        [SerializeField] private float m_HandleShift;
        [SerializeField] private bool m_360CanLoopBack;

        private Vector2 centerPoint;
        private Vector2 initialPoint;
        private float initialAngleOffset;
        private float previousAngleValue;
        private float lockedAngleValue;
        private bool lockLoop;

        //[SerializeField] private RectTransform testCenterPoint;
        //[SerializeField] private RectTransform testInitialPoint;
        //[SerializeField] private RectTransform testHandlePoint;


        // PROPERTIES
        public Shape shape { get { return m_Shape; }}
        public Image.Origin90 origin90 { get { return m_Origin90; }}
        public Image.Origin180 origin180 { get { return m_Origin180; }}
        public Image.Origin360 origin360 { get { return m_Origin360; }}
        public bool Clockwise { get { return m_Clockwise; }}
        public bool reverseClockwise { get { return !m_Clockwise; } }



        // METHODS

        // Complex Setters
        public void SetShape(Shape shape)
        {
            m_Shape = shape;
            SetShape();

            // Secondary data.
            SetOrigin();
            SetClockwise();
            SetSecondaryData();
        }
        public void SetShape()
        {
            if (m_Shape == Shape.Quarter90)
            {
                SetImageShape(Image.FillMethod.Radial90);
            }
            if (m_Shape == Shape.Half180)
            {
                SetImageShape(Image.FillMethod.Radial180);
            }
            if (m_Shape == Shape.Full360)
            {
                SetImageShape(Image.FillMethod.Radial360);
            }
        }
        public void SetOrigin(int originID)
        {
            m_Origin = originID;
            SetOrigin();

            // Secondary data.
            SetShape();
            SetClockwise();
            SetSecondaryData();
        }
        public void SetOrigin()
        {
            m_Origin90 = (Image.Origin90)m_Origin;
            m_Origin180 = (Image.Origin180) m_Origin;
            m_Origin360 = (Image.Origin360)m_Origin;

            if (m_FillRect)
            {
                m_FillImage.type = Image.Type.Filled;
                m_FillImage.fillOrigin = m_Origin;
            }
        }
        public void SetClockwise(bool isClockwise)
        {
            m_Clockwise = isClockwise;
            SetClockwise();

            // Secondary data.
            SetShape();
            SetOrigin();
            SetSecondaryData();
        }
        public void SetClockwise()
        {
            if (m_FillRect)
            {
                m_FillImage.type = Image.Type.Filled;
                m_FillImage.fillClockwise = m_Clockwise;
            }
        }
        protected void SetSecondaryData()
        {
            SetComponentImages();
            SetCenterPoint();
            SetInitialPoint();
        }
        public void SetAllData()
        {
            SetShape();
            SetOrigin();
            SetClockwise();

            SetComponentImages();
            SetCenterPoint();
            SetInitialPoint();
        }

        // Helpers for the above.
        protected void SetImageShape(Image.FillMethod fillMethod)
        {
            if (m_FillRect)
            {
                m_FillImage.type = Image.Type.Filled;
                m_FillImage.fillMethod = fillMethod;
            }
        }

        protected void SetComponentImages()
        {
            if (m_Shape == Shape.Half180)
            {
                Vector2 newSize = new Vector2(m_OriginalWidthAndHeight.x, m_OriginalWidthAndHeight.y);

                if (m_Origin180 == Image.Origin180.Bottom || m_Origin180 == Image.Origin180.Top)
                {
                    newSize.y /= 2;
                    m_RootRect.sizeDelta = newSize;
                }
                else if (m_Origin180 == Image.Origin180.Left || m_Origin180 == Image.Origin180.Right)
                {
                    newSize.x /= 2;
                    m_RootRect.sizeDelta = newSize;
                }
            }
            else
            {
                m_RootRect.sizeDelta = m_OriginalWidthAndHeight;
            }

            SetBackgroundImage();
            SetFillImage();
        }
        protected void SetBackgroundImage()
        {
            if (m_BackgroundRect && m_BackgroundImage)
            {
                if (m_Shape == Shape.Full360)
                {
                    m_BackgroundImage.sprite = m_BackgroundSprite360;
                }
                else if (m_Shape == Shape.Half180)
                {
                    m_BackgroundImage.sprite = m_BackgroundSprite180[(int)m_Origin180];
                }
                else if (m_Shape == Shape.Quarter90)
                {
                    m_BackgroundImage.sprite = m_BackgroundSprite90[(int)m_Origin90];
                }
            }
        }
        protected void SetFillImage()
        {
            if (m_FillRect && m_FillImage)
            {
                if (m_Shape == Shape.Full360)
                {
                    m_FillImage.sprite = m_FillSprite360;
                }
                else if (m_Shape == Shape.Half180)
                {
                    m_FillImage.sprite = m_FillSprite180[(int)m_Origin180];
                }
                else if (m_Shape == Shape.Quarter90)
                {
                    m_FillImage.sprite = m_FillSprite90[(int)m_Origin90];
                }
            }
        }

        protected void SetCenterPoint()
        {
            if (m_Shape == Shape.Quarter90)
            {
                SetCenterPointQuater90();
            }
            if (m_Shape == Shape.Half180)
            {
                SetCenterPointHalf180();
            }
            if (m_Shape == Shape.Full360)
            {
                SetCenterPointFull360();
            }
        }
        protected void SetCenterPointFull360()
        {
            if (m_HandleContainerRect)
            {
                centerPoint = m_HandleContainerRect.anchoredPosition;
            }
            else if (m_FillContainerRect)
            {
                centerPoint = m_FillContainerRect.anchoredPosition;
            }
            else
            {
                centerPoint = m_RootRect.anchoredPosition;
            }

            //if (testCenterPoint)
            //{
            //    testCenterPoint.anchoredPosition = centerPoint;
            //}
        }
        protected void SetCenterPointHalf180()
        {
            (Vector2, Vector2) positionInfo = GetBestRectsInfo();

            if (m_Origin180 == Image.Origin180.Bottom)
            {
                centerPoint = positionInfo.Item1 + new Vector2(0, -positionInfo.Item2.y);
            }
            if (m_Origin180 == Image.Origin180.Left)
            {
                centerPoint = positionInfo.Item1 + new Vector2(-positionInfo.Item2.x, 0);
            }
            if (m_Origin180 == Image.Origin180.Top)
            {
                centerPoint = positionInfo.Item1 + new Vector2(0, positionInfo.Item2.y);
            }
            if (m_Origin180 == Image.Origin180.Right)
            {
                centerPoint = positionInfo.Item1 + new Vector2(positionInfo.Item2.x, 0);
            }

            //if (testCenterPoint)
            //{
            //    testCenterPoint.anchoredPosition = centerPoint;
            //}
        }
        protected void SetCenterPointQuater90()
        {
            (Vector2, Vector2) positionInfo = GetBestRectsInfo();

            if (m_Origin90 == Image.Origin90.BottomLeft)
            {
                centerPoint = positionInfo.Item1 + new Vector2(-positionInfo.Item2.x, -positionInfo.Item2.y);
            }
            if (m_Origin90 == Image.Origin90.TopLeft)
            {
                centerPoint = positionInfo.Item1 + new Vector2(-positionInfo.Item2.x, positionInfo.Item2.y);
            }
            if (m_Origin90 == Image.Origin90.TopRight)
            {
                centerPoint = positionInfo.Item1 + new Vector2(positionInfo.Item2.x, positionInfo.Item2.y);
            }
            if (m_Origin90 == Image.Origin90.BottomRight)
            {
                centerPoint = positionInfo.Item1 + new Vector2(positionInfo.Item2.x, -positionInfo.Item2.y);
            }

            //if (testCenterPoint)
            //{
            //    testCenterPoint.anchoredPosition = centerPoint;
            //}
        }

        protected void SetInitialPoint()
        {
            if (m_Shape == Shape.Quarter90)
            {
                SetInitialPointQuater90();
            }
            if (m_Shape == Shape.Half180)
            {
                SetInitialPointHalf180();
            }
            if (m_Shape == Shape.Full360)
            {
                SetInitialPointFull360();
            }

            Vector2 initialDirection = initialPoint - centerPoint;
            initialAngleOffset = Mathf.Atan2(initialDirection.y, initialDirection.x) * Mathf.Rad2Deg;
            if (initialAngleOffset < 0)
            {
                initialAngleOffset += 360;
            }
        }
        protected void SetInitialPointFull360()
        {
            (Vector2, Vector2) positionInfo = GetBestRectsInfo();

            if (m_Origin360 == Image.Origin360.Bottom)
            {
                initialPoint = positionInfo.Item1 + new Vector2(0, -(positionInfo.Item2.y + m_HandleShift));
            }
            if (m_Origin360 == Image.Origin360.Left)
            {
                initialPoint = positionInfo.Item1 + new Vector2(-(positionInfo.Item2.x + m_HandleShift), 0);
            }
            if (m_Origin360 == Image.Origin360.Top)
            {
                initialPoint = positionInfo.Item1 + new Vector2(0, positionInfo.Item2.y + m_HandleShift);
            }
            if (m_Origin360 == Image.Origin360.Right)
            {
                initialPoint = positionInfo.Item1 + new Vector2(positionInfo.Item2.x + m_HandleShift, 0);
            }

            //if (testInitialPoint)
            //{
            //    testInitialPoint.anchoredPosition = initialPoint;
            //}
        }
        protected void SetInitialPointHalf180()
        {
            (Vector2, Vector2) positionInfo = GetBestRectsInfo();

            if (m_Origin180 == Image.Origin180.Bottom)
            {
                float x = (m_Clockwise ? -(positionInfo.Item2.x + m_HandleShift) : positionInfo.Item2.x + m_HandleShift);
                initialPoint = positionInfo.Item1 + new Vector2(x, -positionInfo.Item2.y);
            }
            if (m_Origin180 == Image.Origin180.Left)
            {
                float y = (m_Clockwise ? positionInfo.Item2.y + m_HandleShift : -(positionInfo.Item2.y + m_HandleShift));
                initialPoint = positionInfo.Item1 + new Vector2(-positionInfo.Item2.x, y);
            }
            if (m_Origin180 == Image.Origin180.Top)
            {
                float x = (m_Clockwise ? positionInfo.Item2.x + m_HandleShift : -(positionInfo.Item2.x + m_HandleShift));
                initialPoint = positionInfo.Item1 + new Vector2(x, positionInfo.Item2.y);
            }
            if (m_Origin180 == Image.Origin180.Right)
            {
                float y = (m_Clockwise ? -(positionInfo.Item2.y + m_HandleShift) : positionInfo.Item2.y + m_HandleShift);
                initialPoint = positionInfo.Item1 + new Vector2(positionInfo.Item2.x, y);
            }

            //if (testInitialPoint)
            //{
            //    testInitialPoint.anchoredPosition = initialPoint;
            //}
        }
        protected void SetInitialPointQuater90()
        {
            (Vector2, Vector2) positionInfo = GetBestRectsInfo();

            if (m_Origin90 == Image.Origin90.BottomLeft)
            {
                float x = (m_Clockwise ? -positionInfo.Item2.x : positionInfo.Item2.x + m_HandleShift);
                float y = (m_Clockwise ? positionInfo.Item2.y + m_HandleShift : -positionInfo.Item2.y);
                initialPoint = positionInfo.Item1 + new Vector2(x, y);
            }
            if (m_Origin90 == Image.Origin90.TopLeft)
            {
                float x = (m_Clockwise ? positionInfo.Item2.x + m_HandleShift : -positionInfo.Item2.x);
                float y = (m_Clockwise ? positionInfo.Item2.y : -(positionInfo.Item2.y + m_HandleShift));
                initialPoint = positionInfo.Item1 + new Vector2(x, y);
            }
            if (m_Origin90 == Image.Origin90.TopRight)
            {
                float x = (m_Clockwise ? positionInfo.Item2.x : -(positionInfo.Item2.x + m_HandleShift));
                float y = (m_Clockwise ? -(positionInfo.Item2.y + m_HandleShift) : positionInfo.Item2.y);
                initialPoint = positionInfo.Item1 + new Vector2(x, y);
            }
            if (m_Origin90 == Image.Origin90.BottomRight)
            {
                float x = (m_Clockwise ? -(positionInfo.Item2.x + m_HandleShift) : positionInfo.Item2.x);
                float y = (m_Clockwise ? -positionInfo.Item2.y : positionInfo.Item2.y + m_HandleShift);
                initialPoint = positionInfo.Item1 + new Vector2(x, y);
            }

            //if (testInitialPoint)
            //{
            //    testInitialPoint.anchoredPosition = initialPoint;
            //}
        }
        protected (Vector2, Vector2) GetBestRectsInfo()
        {
            Vector2 anchoredPosition;
            Vector2 areaSize;

            if (m_HandleContainerRect)
            {
                anchoredPosition = m_HandleContainerRect.anchoredPosition;
                areaSize = m_HandleContainerRect.rect.size;
            }
            else if (m_FillContainerRect)
            {
                anchoredPosition = m_FillContainerRect.anchoredPosition;
                areaSize = m_FillContainerRect.rect.size;
            }
            else
            {
                anchoredPosition = m_FillContainerRect.anchoredPosition;
                areaSize = m_FillContainerRect.rect.size;
            }
            areaSize /= 2;

            return (anchoredPosition, areaSize);
        }


        // UTILITY
        protected Vector2 CalculateHandlePositionFromFill()
        {
            float thetha = CalculateTargetAngleFromFill();

            float xDelta = initialPoint.x - centerPoint.x;
            float yDelta = initialPoint.y - centerPoint.y;

            Vector2 cartesianCoords = new Vector2();

            cartesianCoords.x = xDelta * Mathf.Cos(thetha) - yDelta * Mathf.Sin(thetha) + centerPoint.x;
            cartesianCoords.y = xDelta * Mathf.Sin(thetha) + yDelta * Mathf.Cos(thetha) + centerPoint.y;

            return cartesianCoords;
        }
        protected float CalculateTargetAngleFromFill()
        {
            float thetha = 0;

            if (m_Shape == Shape.Quarter90)
            {
                thetha = normalizedValue * 90;
            }
            if (m_Shape == Shape.Half180)
            {
                thetha = normalizedValue * 180;
            }
            if (m_Shape == Shape.Full360)
            {
                thetha = normalizedValue * 360;
            }

            if (m_Clockwise)
            {
                thetha *= -1;
            }

            thetha *= Mathf.Deg2Rad;
            return thetha;
        }


        // OVERRIDES
        protected override void UpdateCachedReferences()
        {
            base.UpdateCachedReferences();

            if (!m_RootRect)
            {
                m_RootRect = this.gameObject.GetComponent<RectTransform>();
                m_OriginalWidthAndHeight = m_RootRect.sizeDelta;
            }

            if (m_BackgroundRect && !m_BackgroundImage)
            {
                m_BackgroundImage = m_BackgroundRect.gameObject.GetComponent<Image>();
            }

            SetAllData(); // Not sure this is necessary.
        }
        protected override void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif

            m_Tracker.Clear();

            if (m_FillContainerRect != null)
            {
                m_Tracker.Add(this, m_FillRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;

                if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
                {
                    m_FillImage.fillAmount = normalizedValue;
                }
                else
                {
                    if (reverseClockwise)
                        anchorMin[(int)axis] = 1 - normalizedValue;
                    else
                        anchorMax[(int)axis] = normalizedValue;
                }

                m_FillRect.anchorMin = anchorMin;
                m_FillRect.anchorMax = anchorMax;
            }

            if (m_HandleContainerRect != null)
            {
                Vector2 newHandleCoords = CalculateHandlePositionFromFill();
                m_HandleRect.anchoredPosition = newHandleCoords;
            }
        }

        protected override void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = m_HandleContainerRect ?? m_FillContainerRect;
            if (clickRect != null && clickRect.rect.size[(int)axis] > 0)
            {
                // Stuff from the original slider.
                Vector2 relativeMousePosition = Vector2.zero;
                if (!MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref relativeMousePosition))
                {
                    return;
                }

                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, relativeMousePosition, cam, out localCursor))
                {
                    return;
                }

                // Calculate the difference between the zero angle and our own.
                Vector2 dragDirection = localCursor - centerPoint;
                float dragAngle = Mathf.Atan2(dragDirection.y, dragDirection.x) * Mathf.Rad2Deg;
                if (dragAngle < 0)
                {
                    dragAngle += 360f;
                }

                float angleValue = initialAngleOffset - dragAngle;
                if (angleValue < 0)
                {
                    angleValue += 360f;
                }
                if (!m_Clockwise)
                {
                    angleValue = 360f - angleValue;
                }

                // Avoid overflow on a per shape basis.
                if (m_Shape == Shape.Half180)
                {
                    if (angleValue > 180f)
                    {
                        if (angleValue >= 190f && angleValue <= 350f)
                        {
                            return;
                        }
                        angleValue = (angleValue < 190f) ? 180f : 0f;
                    }
                    normalizedValue = angleValue / 180f;
                    return;
                }
                else if (m_Shape == Shape.Quarter90)
                {
                    if (angleValue > 90f)
                    {
                        if (angleValue >= 100f && angleValue <= 350f)
                        {
                            return;
                        }
                        angleValue = (angleValue < 100f) ? 90f : 0f;
                    }
                    normalizedValue = angleValue / 90f;
                    return;
                }

                // Everything 360.
                if (m_360CanLoopBack)
                {
                    if (angleValue <= 5f || angleValue >= 355f)
                    {
                        angleValue = (angleValue >= 355f) ? 360f : 0f;
                    }

                    normalizedValue = angleValue / 360f;
                    return;
                }

                float angleDelta = previousAngleValue - angleValue;

                if (lockLoop)
                {
                    if (angleValue >= 5f && angleValue <= 355f)
                    {
                        if (lockedAngleValue == 360f && angleDelta > 0 && angleDelta < 50f)
                        {
                            normalizedValue = angleValue / 360f;
                            lockLoop = false;
                            return;
                        }
                        else if (lockedAngleValue == 0 && angleDelta < 0 && angleDelta > -50f)
                        {
                            normalizedValue = angleValue / 360f;
                            lockLoop = false;
                            return;
                        }
                    }
                    previousAngleValue = angleValue;
                }
                else
                {
                    if (angleValue >= 5f && angleValue <= 355f)
                    {
                        normalizedValue = angleValue / 360f;
                    }
                    else
                    {
                        angleValue = (angleValue >= 5f) ? 360f : 0f;
                        normalizedValue = angleValue / 360f;

                        lockLoop = true;
                    }

                    previousAngleValue = angleValue;
                    lockedAngleValue = previousAngleValue;
                }
            }
        }


        // BUILT IN
        protected override void Start()
        {
            SetAllData();
            UpdateVisuals();
        }
    }
}
