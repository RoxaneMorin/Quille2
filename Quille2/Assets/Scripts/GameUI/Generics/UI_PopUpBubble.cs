using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] protected TMP_Text myTextMain;
        [SerializeField] protected TMP_Text myTextOverflow;
        [SerializeField] protected Image myIcon;

        [SerializeField] protected RectTransform myRect;
        [SerializeField] protected RectTransform myOwnersRect;
        [SerializeField] protected RectTransform myTailsRect;
        [SerializeField] protected RectTransform myTextMainsRect;
        [SerializeField] protected RectTransform myTextOverflowsRect;
        [SerializeField] protected RectTransform myIconsRect;

        // Resources
        [SerializeField] protected Sprite tailSpritePointingDown;
        [SerializeField] protected Sprite tailSpritePointingUp;
        [SerializeField] protected Sprite tailSpritePointingLeft;
        [SerializeField] protected Sprite tailSpritePointingRight;

        // Layout
        [SerializeField] protected bool hasIcon = true;

        [SerializeField] protected float interiorPadding = 10.0f;

        [SerializeField] protected float fontSize = 20.0f;


        

        [SerializeField] protected float tailDistance = 16.0f;


        [SerializeField] protected float distanceFromOwner = 40.0f;




        [SerializeField] protected Vector2 bubbleRatio;

        // Tail size
        // Icon size

        // Icon and tail sizes, only customisable in editor?



        // Runtime
        [Header("Runtime")]
        [SerializeField] protected float ownerScreenPosXRightOfPivot;
        [SerializeField] protected float ownerScreenPosXLeftOfPivot;
        [SerializeField] protected float ownerScreenPosYTopOfPivot;
        [SerializeField] protected float ownerScreenPosYBottomOfPivot;



        [SerializeField] protected FourDirections currentDirection;



        // PROPERTIES
        public float InteriorPadding
        {
            get { return interiorPadding; }
            set
            {
                interiorPadding = value;
            }
        }


        public float TailDistance
        {
            get { return tailDistance; }
            set
            {
                tailDistance = value;
                // TODO: 
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

            if (myOwner != null)
            {
                SetOwner(myOwner);
            }
            else if (transform.parent != null && transform.parent.gameObject.GetComponent<CanvasRenderer>() != null)
            {
                // If our parent gameObject possesses a CanvasRenderer, we assume it's an UI element that can be our owner.
                SetOwner(transform.parent.gameObject);
            }
            else
            {
                Debug.LogWarning($"The PopUpBubble '{this.name}' lacks a valid owner and will not function properly.");
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

            if (myTextMain != null)
            {
                myTextMainsRect = (RectTransform)myTextMain.transform;
            }
            if (myTextOverflow != null)
            {
                myTextOverflowsRect = (RectTransform)myTextOverflow.transform;
            }
            if (myIcon!= null)
            {
                myIconsRect = (RectTransform)myIcon.transform;
            }

            // TODO: add warnings for missing refs
        }

        protected void SetOwner(GameObject newOwner)
        {
            myOwner = newOwner;
            myOwnersRect = (RectTransform)newOwner.transform;

            // Make owner my parent if it's not already.
            if (transform.parent != newOwner.transform)
            {
                transform.parent = newOwner.transform;
                SetBubbleDirection(FourDirections.Top);
            }

            CalculateOwnerScreenPos();
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





        protected void SetBubbleDirectionFromCursorPos(Vector2 cursorPos)
        {
            (FourDirections, float) closestSideToCursor = GetClosestSideToPoint(cursorPos);


            // TODO: account for the edge of the screen?






            SetBubbleDirection(closestSideToCursor.Item1);
        }
        protected void SetBubbleDirection(FourDirections direction)
        {
            currentDirection = direction;
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




        // Update text, image/icon
        // Set size from text or image




        public void SetBubbleText(string newBubbleText)
        {
            myTextMain.SetText(newBubbleText);
            UpdateBubbleDimensions();
        }

        public void SetBubbleIcon(Sprite newBubbleIcon)
        {
            if (newBubbleIcon != null)
            {
                myIcon.sprite = newBubbleIcon;
                myIcon.enabled = true;
                hasIcon = true;
            }
            else
            {
                myIcon.sprite = null;
                myIcon.enabled = false;
                hasIcon = false;
            }

            UpdateBubbleDimensions();
        }
        public void ClearBubbleIcon()
        {
            SetBubbleIcon(null);
        }


        public void ResizeBubbleIcon(Vector2 newIconSize)
        {
            if (hasIcon)
            {
                myIconsRect.sizeDelta = newIconSize;
                UpdateBubbleDimensions();
            }
        }

        
        public void UpdateBubbleDimensions()
        {
            // Adjust text flow and padding.
            Vector2 topLeftCornerPadding = new Vector2(interiorPadding, -interiorPadding);
            float bubbleWidthMinPadding = myRect.sizeDelta.x - interiorPadding * 2.0f;

            if (hasIcon)
            {
                myIcon.enabled = true;
                myIconsRect.anchoredPosition = topLeftCornerPadding;

                float paddedIconWidth = myIconsRect.sizeDelta.x + interiorPadding * 2.0f;
                float paddedIconHeight = myIconsRect.sizeDelta.y + interiorPadding * 2.0f;
                float mainTextSizeX = myRect.sizeDelta.x - paddedIconWidth - interiorPadding;

                if (myTextMain.preferredHeight > paddedIconHeight)
                {
                    myTextOverflow.enabled = true;
                    myTextMain.overflowMode = TextOverflowModes.Linked;
                    myTextMain.linkedTextComponent = myTextOverflow;

                    float halfPaddedIconHeight = myIconsRect.sizeDelta.y + interiorPadding;
                    float overflowTextSizeX = myRect.sizeDelta.x - interiorPadding * 2.0f;

                    myTextMainsRect.anchoredPosition = new Vector2(paddedIconWidth, -interiorPadding);
                    myTextMainsRect.sizeDelta = new Vector2(mainTextSizeX, halfPaddedIconHeight);
                    myTextMain.ForceMeshUpdate();

                    myTextOverflowsRect.anchoredPosition = new Vector2(interiorPadding, -paddedIconHeight);
                    myTextOverflowsRect.sizeDelta = new Vector2(overflowTextSizeX, myTextOverflow.preferredHeight);

                    float totalBubbleHeight = paddedIconHeight + myTextOverflow.preferredHeight + interiorPadding;

                    myRect.sizeDelta = new Vector2(myRect.sizeDelta.x, totalBubbleHeight);

                }
                else
                {
                    myTextOverflow.enabled = false;

                    float textPaddingY = -(paddedIconHeight - myTextMain.preferredHeight) / 2.0f;
                    
                    myTextMainsRect.anchoredPosition = new Vector2(paddedIconWidth, textPaddingY);
                    myTextMainsRect.sizeDelta = new Vector2(mainTextSizeX, myTextMain.preferredHeight);

                    myRect.sizeDelta = new Vector2(myRect.sizeDelta.x, paddedIconHeight);
                }
            }
            else
            {
                myIcon.enabled = false;

                myTextOverflow.enabled = false;
                myTextMain.overflowMode = TextOverflowModes.Overflow;

                myTextMainsRect.anchoredPosition = topLeftCornerPadding;
                //myTextMainsRect.sizeDelta = new Vector2(bubbleWidthMinPadding, myTextMain.preferredHeight);

                //myRect.sizeDelta = new Vector2(myRect.sizeDelta.x, myTextMain.preferredHeight + interiorPadding * 2.0f);



                float paramRatio = bubbleRatio.x / bubbleRatio.y;
                float mysteriousValue = Mathf.Sqrt(myTextMain.preferredWidth/(paramRatio * myTextMain.textInfo.lineInfo[0].lineHeight));
                float newWidth = myTextMain.preferredWidth / mysteriousValue;
                float newHeight = myTextMain.textInfo.lineInfo[0].lineHeight * mysteriousValue;

                myTextMainsRect.sizeDelta = new Vector2(newWidth, newHeight);
                myTextMain.ForceMeshUpdate();


                myRect.sizeDelta = new Vector2(newWidth + interiorPadding * 2.0f, myTextMain.preferredHeight + interiorPadding * 2.0f);


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
            //gameObject.SetActive(false);
        }





        [ExecuteInEditMode]
        private void Awake()
        {
            Init();

            //SetBubbleText("A longer, quite longer and rambly text like the babblering of a senile old man whose brain has been destroyed by degenerescence.");
        }
    }

}
