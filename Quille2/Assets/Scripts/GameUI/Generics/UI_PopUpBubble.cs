using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace QuilleUI
{
    // 
    public class UI_PopUpBubble : MonoBehaviour
    {
        // VARIABLES
        // References
        [SerializeField] protected GameObject myOwner;
        [SerializeField] protected GameObject myTail;
        [SerializeField] protected Image myTailsImage;
        [SerializeField] protected UI_ReflowableTextbox myReflowableTextbox;

        [SerializeField] protected RectTransform myRect;
        [SerializeField] protected RectTransform myOwnersRect;
        [SerializeField] protected RectTransform myTailsRect;
        [SerializeField] protected RectTransform myReflowableTextboxsRect;

        // Resources
        [SerializeField] protected Sprite tailSpritePointingDown;
        [SerializeField] protected Sprite tailSpritePointingUp;
        [SerializeField] protected Sprite tailSpritePointingLeft;
        [SerializeField] protected Sprite tailSpritePointingRight;

        // Layout
        [SerializeField] protected float interiorPadding = 10f;
        [SerializeField] protected float distanceFromOwner = 40.0f;
        [SerializeField] protected float tailDistance = 16.0f;
        // Tail size?

        // Runtime
        [Header("Runtime")]
        protected bool isDirty;

        [SerializeField] protected float ownerScreenPosXRightOfPivot;
        [SerializeField] protected float ownerScreenPosXLeftOfPivot;
        [SerializeField] protected float ownerScreenPosYTopOfPivot;
        [SerializeField] protected float ownerScreenPosYBottomOfPivot;

        [SerializeField] protected FourDirections currentDirection;



        // PROPERTIES
        public GameObject Owner
        {
            get { return myOwner; }
            set
            {
                myOwner = value;
                UpdateOwnerData();

                // TODO: protect against invalid owners.
            }
        }

        public Sprite IconSprite
        {
            get
            {
                if (myReflowableTextbox != null)
                {
                    return myReflowableTextbox.IconSprite;
                }
                else
                { 
                    return null; 
                }
            }
            set
            {
                if (myReflowableTextbox != null)
                {
                    myReflowableTextbox.IconSprite = value;
                    SetDirty();
                }
            }
        }
        public string TextContent
        {
            get
            {
                if (myReflowableTextbox != null)
                {
                    return myReflowableTextbox.TextContent;
                }
                else
                { 
                    return string.Empty; 
                }
            }
            set
            {
                if (myReflowableTextbox != null)
                {
                    myReflowableTextbox.TextContent = value;
                    SetDirty();
                }
            }
        }

        public Vector2 DimensionRatio
        {
            get
            {
                if (myReflowableTextbox != null)
                {
                    return myReflowableTextbox.DimensionRatio;
                }
                else
                { 
                    return Vector2.zero; 
                }
            }
            set
            {
                if (myReflowableTextbox != null)
                {
                    myReflowableTextbox.DimensionRatio = value;
                    SetDirty();
                }
            }
        }
        public float InteriorPadding
        {
            get { return interiorPadding; }
            set
            {
                interiorPadding = value;
                SetDirty();
            }
        }

        public bool HasIcon
        {
            get
            {
                if (myReflowableTextbox != null)
                {
                    return myReflowableTextbox.HasIcon;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (myReflowableTextbox != null)
                {
                    myReflowableTextbox.HasIcon = value;
                    SetDirty();
                }
            }
        }
        public Vector2 IconSize
        {
            get
            {
                if (myReflowableTextbox != null)
                {
                    return myReflowableTextbox.IconSize;
                }
                else
                {
                    return Vector2.zero;
                }
            }
            set
            {
                if (myReflowableTextbox != null)
                {
                    myReflowableTextbox.IconSize = value;
                    SetDirty();
                }
            }
        }
        public float FontSize
        {
            get
            {
                if (myReflowableTextbox != null)
                {
                    return myReflowableTextbox.FontSize;
                }
                else
                { 
                    return 0.0f; 
                }
            }
            set
            {
                if (myReflowableTextbox != null)
                {
                    myReflowableTextbox.FontSize = value;
                    SetDirty();
                }
            }
        }

        public float DistanceFromOwner
        {
            get { return distanceFromOwner; }
            set
            {
                distanceFromOwner = value;
                SetDirty();
            }
        }
        public float TailDistance
        {
            get { return tailDistance; }
            set
            {
                tailDistance = value;
                SetDirty();
            }
        }

        public Vector2 TailSize
        {
            get
            {
                if (myTailsRect != null)
                {
                    return myTailsRect.sizeDelta;
                }
                else
                {
                    return Vector2.zero;
                }
            }
            set
            {
                if (myTailsRect != null)
                {
                    myTailsRect.sizeDelta = value;
                    SetDirty();
                }
            }
        }

        public bool IsDirty { get { return isDirty; } }
        public void SetDirty() { isDirty = true; }

        public FourDirections CurrentDirection
        {
            get { return currentDirection; }
            set
            {
                currentDirection = value;
                UpdateVisualBubbleDirection(currentDirection);
            }
        }



        // METHODS
        protected void Init()
        {
            FetchComponents();
        }
        protected void FetchComponents()
        {
            myRect = (RectTransform)transform;

            if (myOwner == null && transform.parent != null && transform.parent.gameObject.GetComponent<CanvasRenderer>() != null)
            {
                // If our parent gameObject possesses a CanvasRenderer, we assume it's an UI element that can be our owner.
                Owner = transform.parent.gameObject;
            }
            if (myOwner != null)
            {
                UpdateOwnerData();
            }
            else
            {
                Debug.LogWarning($"The PopUpBubble '{this.name}' lacks a valid owner and will not function properly.");
            }

            if (myReflowableTextbox == null)
            {
                myReflowableTextbox = gameObject.GetComponentInChildren<UI_ReflowableTextbox>();
            }
            if (myReflowableTextbox != null)
            {
                myReflowableTextboxsRect = (RectTransform)myReflowableTextbox.transform;
            }
            else
            {
                Debug.LogWarning($"The PopUpBubble '{this.name}' is missing its ReflowableTextbox reference and will not function properly.");
            }

            if (myTail != null)
            {
                myTailsRect = (RectTransform)myTail.transform;
                myTailsImage = myTail.GetComponent<Image>();
            }
            else
            {
                Debug.LogWarning($"The PopUpBubble '{this.name}' is missing its Tail reference and will not function properly.");
            }
        }


        protected void UpdateOwnerData()
        {
            if (myOwner != null && myOwner.GetComponent<CanvasRenderer>() != null)
            {
                myOwnersRect = (RectTransform)myOwner.transform;

                // Make owner my parent if it's not already.
                if (transform.parent != myOwner.transform)
                {
                    transform.parent = myOwner.transform;
                    UpdateVisualBubbleDirection(currentDirection);
                }

                CalculateOwnerScreenPos();
            }
        }



        protected void CalculateOwnerScreenPos()
        {
            // Determine owner's edge positions from its size and pivot.
            float ownerSizeXLeftOfPivot = myOwnersRect.sizeDelta.x * myOwnersRect.pivot.x;
            float ownerSizeXRightOfPivot = myOwnersRect.sizeDelta.x - ownerSizeXLeftOfPivot;
            float ownerSizeYBottomOfPivot = myOwnersRect.sizeDelta.y * myOwnersRect.pivot.y;
            float ownerSizeYTopOfPivot = myOwnersRect.sizeDelta.y - ownerSizeYBottomOfPivot;

            ownerScreenPosXRightOfPivot = myOwnersRect.position.x + ownerSizeXRightOfPivot;
            ownerScreenPosXLeftOfPivot = myOwnersRect.position.x - ownerSizeXLeftOfPivot;
            ownerScreenPosYTopOfPivot = myOwnersRect.position.y + ownerSizeYTopOfPivot;
            ownerScreenPosYBottomOfPivot = myOwnersRect.position.y - ownerSizeYBottomOfPivot;
        }


        public void UpdateLayout()
        {
            if (myReflowableTextbox.IsDirty)
            {
                myReflowableTextbox.InteriorPadding = interiorPadding;
                myRect.sizeDelta = myReflowableTextbox.UpdateLayout() + new Vector2(interiorPadding, interiorPadding) * 2.0f;
            }

            UpdateVisualBubbleDirection(currentDirection);

            isDirty = false;
        }

        protected void SetBubbleDirectionFromCursorPos(Vector2 cursorPos)
        {
            (FourDirections, float) closestSideToCursor = GetClosestSideToPoint(cursorPos);

            // TODO: account for the edge of the screen?

            CurrentDirection = closestSideToCursor.Item1;
        }
        protected void UpdateVisualBubbleDirection(FourDirections direction)
        {
            switch (direction)
            {
                case FourDirections.Top:
                    {
                        myRect.pivot = new Vector2(0.5f, 0.0f);
                        myRect.anchorMin = myRect.anchorMax = new Vector2(0.5f, 1.0f);
                        myRect.anchoredPosition = new Vector2(0.0f, distanceFromOwner);

                        myTailsRect.pivot = new Vector2(0.5f, 1.0f);
                        myTailsRect.anchorMin = myTailsRect.anchorMax = new Vector2(0.5f, 0.0f);
                        myTailsRect.anchoredPosition = new Vector2(0.0f, tailDistance);

                        myTailsImage.sprite = tailSpritePointingDown;

                        break;
                    }
                case FourDirections.Bottom:
                    {
                        myRect.pivot = new Vector2(0.5f, 1.0f);
                        myRect.anchorMin = myRect.anchorMax = new Vector2(0.5f, 0.0f);
                        myRect.anchoredPosition = new Vector2(0.0f, -distanceFromOwner);

                        myTailsRect.pivot = new Vector2(0.5f, 0.0f);
                        myTailsRect.anchorMin = myTailsRect.anchorMax = new Vector2(0.5f, 1.0f);
                        myTailsRect.anchoredPosition = new Vector2(0.0f, -tailDistance);

                        myTailsImage.sprite = tailSpritePointingUp;

                        break;
                    }
                case FourDirections.Right:
                    {
                        myRect.pivot = new Vector2(0.0f, 0.5f);
                        myRect.anchorMin = myRect.anchorMax = new Vector2(1.0f, 0.5f);
                        myRect.anchoredPosition = new Vector2(distanceFromOwner, 0.0f);

                        myTailsRect.pivot = new Vector2(1.0f, 0.5f);
                        myTailsRect.anchorMin = myTailsRect.anchorMax = new Vector2(0.0f, 0.5f);
                        myTailsRect.anchoredPosition = new Vector2(tailDistance, 0.0f);

                        myTailsImage.sprite = tailSpritePointingLeft;

                        break;
                    }
                case FourDirections.Left:
                    {
                        myRect.pivot = new Vector2(1.0f, 0.5f);
                        myRect.anchorMin = myRect.anchorMax = new Vector2(0.0f, 0.5f);
                        myRect.anchoredPosition = new Vector2(-distanceFromOwner, 0.0f);

                        myTailsRect.pivot = new Vector2(0.0f, 0.5f);
                        myTailsRect.anchorMin = myTailsRect.anchorMax = new Vector2(1.0f, 0.5f);
                        myTailsRect.anchoredPosition = new Vector2(-tailDistance, 0.0f);

                        myTailsImage.sprite = tailSpritePointingRight;

                        break;
                    }
                default:
                    break;
            }
        }

        protected (FourDirections, float) GetClosestSideToPoint(Vector2 point)
        {
            (FourDirections, float) closestTopBottom = GetSideTopBottomClosestToPoint(point);
            (FourDirections, float) closestRightLeft = GetSideRightLeftClosestToPoint(point);

            if (closestTopBottom.Item2 < closestRightLeft.Item2)
            {
                return closestTopBottom;
            }
            else
            {
                return closestRightLeft;
            }
        }
        protected (FourDirections, float) GetSideTopBottomClosestToPoint(Vector2 point)
        {
            float distFromTop = Mathf.Abs(ownerScreenPosYTopOfPivot - point.y);
            float distFromBottom = Mathf.Abs(ownerScreenPosYBottomOfPivot - point.y);

            if (distFromTop < distFromBottom)
            {
                return (FourDirections.Top, distFromTop);
            }
            else
            {
                return (FourDirections.Bottom, distFromBottom);
            }
        }
        protected (FourDirections, float) GetSideRightLeftClosestToPoint(Vector2 point)
        {
            float distFromRight = Mathf.Abs(ownerScreenPosXRightOfPivot - point.x);
            float distFromLeft = Mathf.Abs(ownerScreenPosXLeftOfPivot - point.x);

            if (distFromRight < distFromLeft)
            {
                return (FourDirections.Right, distFromRight);
            }
            else
            {
                return (FourDirections.Left, distFromLeft);
            }
        }


        // EVENT LISTENERS
        public void DisplayBubble(BaseEventData eventData)
        {
            // Having to cast here as Unity's UI event thing is being a fuck.
            PointerEventData pointerEventData = (PointerEventData)eventData;
            if (pointerEventData == null)
            {
                return;
            }

            SetBubbleDirectionFromCursorPos(pointerEventData.position);
            gameObject.SetActive(true);
        }

        public void HideBubble()
        {
            gameObject.SetActive(false);
        }


        // BUILT IN

        private void Awake()
        {
            Init();
            UpdateLayout();
        }
    }

}
