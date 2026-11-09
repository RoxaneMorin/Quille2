using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace QuilleUI
{
    // Generic UI container that can be opened, closed and resized by the user.
    public class UI_FoldingPanel : MonoBehaviour
    {
        // VARIABLES
        // References
        [SerializeField] protected GameObject myDragGrip;
        [SerializeField] protected Image myDragGripImage;
        [SerializeField] protected Button myButton;
        [SerializeField] protected TMP_Text myButtonText;
        [SerializeField] protected GameObject myContent;

        [SerializeField] protected RectTransform myRect;
        [SerializeField] protected RectTransform myDragGripsRect;
        [SerializeField] protected RectTransform myButtonsRect;
        [SerializeField] protected RectTransform myContentsRect;

        // Resources
        [SerializeField] protected Sprite dragGripSpriteLeft;
        [SerializeField] protected Sprite dragGripSpriteRight;

        // Layout
        [SerializeField] protected float minOpenPanelWidth;
        [SerializeField] protected float maxOpenPanelWidth;
         
        [SerializeField] protected float currentOpenPanelWidth;
        [SerializeField] protected float currentClosedPanelWidth;
        
        [SerializeField] protected float currentInteriorPadding;
        [SerializeField] protected float currentButtonWidth;
        [SerializeField] protected float currentContentWidth;
        [SerializeField] protected float currentContentHeight;
        [SerializeField] protected float currentDragGripDistance;

        [SerializeField] protected bool isRightSidePanel;
        [SerializeField] protected bool isOpen = true;

        [SerializeField] protected bool isDirty;


        // TODO:
        // - drag grip only appears (or fully appears) on hover?
        // - update layout on resolution change
        // - Add IsDirty, generally refresh the editor-side handling to better match newer UI items.


        // PROPERTIES
        public float MinOpenPanelWidth 
        { 
            get { return minOpenPanelWidth; } 
            set
            {
                if (value < 0 || value > maxOpenPanelWidth)
                {
                    return;
                }
                minOpenPanelWidth = value;
            }
        }
        public float MaxOpenPanelWidth 
        { 
            get { return maxOpenPanelWidth; }
            set
            {
                if (value < minOpenPanelWidth || value > Screen.currentResolution.width)
                {
                    return;
                }
                maxOpenPanelWidth = value;
            }
        }
        public float CurrentOpenPanelWidth { get { return currentOpenPanelWidth; } }
        public float CurrentClosedPanelWidth { get { return currentClosedPanelWidth; } }
        public float CurrentInteriorPadding { get { return currentInteriorPadding; } }
        public float ButtonWidth 
        {
            get
            {
                if (myButtonsRect != null)
                {
                    return myButtonsRect.sizeDelta.x;
                }
                else
                {
                    return 0.0f;
                }
            }
            set
            {
                if (myButtonsRect != null)
                {
                    myButtonsRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
                    SetDirty();
                }
            }
        }
        public float CurrentDragGripDistance { get { return currentDragGripDistance; } }
        public bool IsRightSidePanel { get { return isRightSidePanel; } }
        public bool IsOpen { get { return isOpen; } }

        public bool IsDirty { get { return isDirty; } }
        public void SetDirty() { isDirty = true; }



        // METHODS
        protected void Init()
        {
            FetchComponents();

            UpdatePadding(currentInteriorPadding);
            UpdatePanelWidth(currentOpenPanelWidth);
        }
        protected void FetchComponents()
        {
            myRect = (RectTransform)transform;

            if (myDragGrip != null)
            {
                myDragGripImage = myDragGrip.GetComponent<Image>();
                myDragGripsRect = (RectTransform)myDragGrip.transform;
            }
            else
            {
                Debug.LogWarning($"The FoldingPanel '{this.name}' is missing its DragGrip reference and may not function properly.");
            }

            myButton = GetComponentInChildren<Button>();
            if (myButton != null)
            {
                myButtonText = myButton.GetComponentInChildren<TMP_Text>();
                myButtonsRect = (RectTransform)myButton.transform;
            }
            else
            {
                Debug.LogWarning($"The FoldingPanel '{this.name}' is missing its Button reference and may not function properly.");
            }

            if (myContent != null)
            {
                myContentsRect = (RectTransform)myContent.transform;
            }
            else
            {
                Debug.LogWarning($"The FoldingPanel '{this.name}' is missing its Content reference and may not function properly.");
            }

        }


        // LAYOUT ADJUSTMENTS
        public void SetPanelSide(bool isRightSide)
        {
            isRightSidePanel = isRightSide;

            if (isRightSidePanel)
            {
                myRect.anchorMin = myRect.anchorMax = myRect.pivot = new Vector2(1.0f, 0.5f);
                myContentsRect.anchorMin = myContentsRect.anchorMax = myContentsRect.pivot = new Vector2(1.0f, 0.5f);

                myButtonsRect.anchorMin = myButtonsRect.anchorMax = myButtonsRect.pivot = new Vector2(0.0f, 0.5f);
                myDragGripsRect.anchorMin = myDragGripsRect.anchorMax = myDragGripsRect.pivot = new Vector2(0.0f, 0.5f);

                myContentsRect.anchoredPosition = new Vector2(-currentInteriorPadding, myContentsRect.anchoredPosition.y);
                myButtonsRect.anchoredPosition = new Vector2(currentInteriorPadding, myButtonsRect.anchoredPosition.y);
                myDragGripsRect.anchoredPosition = new Vector2(-currentDragGripDistance, myDragGripsRect.anchoredPosition.y);

                myDragGripImage.sprite = dragGripSpriteRight;
            }
            else
            {
                myRect.anchorMin = myRect.anchorMax = myRect.pivot = new Vector2(0.0f, 0.5f);
                myContentsRect.anchorMin = myContentsRect.anchorMax = myContentsRect.pivot = new Vector2(0.0f, 0.5f);

                myButtonsRect.anchorMin = myButtonsRect.anchorMax = myButtonsRect.pivot = new Vector2(1.0f, 0.5f);
                myDragGripsRect.anchorMin = myDragGripsRect.anchorMax = myDragGripsRect.pivot = new Vector2(1.0f, 0.5f);

                myContentsRect.anchoredPosition = new Vector2(currentInteriorPadding, myContentsRect.anchoredPosition.y);
                myButtonsRect.anchoredPosition = new Vector2(-currentInteriorPadding, myButtonsRect.anchoredPosition.y);
                myDragGripsRect.anchoredPosition = new Vector2(currentDragGripDistance, myDragGripsRect.anchoredPosition.y);

                myDragGripImage.sprite = dragGripSpriteLeft;
            }

            myRect.anchoredPosition = new Vector2(0.0f, myRect.anchoredPosition.y);
        }
        public void UpdatePanelWidth(float newPanelWidth)
        {
            if (newPanelWidth < minOpenPanelWidth || newPanelWidth > maxOpenPanelWidth)
            {
                newPanelWidth = Mathf.Clamp(newPanelWidth, minOpenPanelWidth, maxOpenPanelWidth);
            }

            currentOpenPanelWidth = newPanelWidth;
            myRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentOpenPanelWidth);

            UpdateContentWidth();
        }
        protected void UpdateContentWidth()
        {
            RecalculateClosedPanelWidth();

            currentContentWidth = currentOpenPanelWidth - (myButtonsRect.sizeDelta.x + currentInteriorPadding * 2.5f);
            myContentsRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentContentWidth);
        }
        protected void RecalculateClosedPanelWidth()
        {
            currentClosedPanelWidth = myButtonsRect.sizeDelta.x + currentInteriorPadding * 2.0f;
        }

//#if (UNITY_EDITOR)
        public void UpdatePadding(float newPadding)
        {
            if (newPadding < 0)
            {
                newPadding = 0.0f;
            }

            currentInteriorPadding = newPadding;
            currentClosedPanelWidth = myButtonsRect.sizeDelta.x + currentInteriorPadding * 2.0f;
            currentContentHeight = Screen.currentResolution.height - currentInteriorPadding * 2.0f;

            myButtonsRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentContentHeight);
            myContentsRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentContentHeight);

            if (isRightSidePanel)
            {
                myButtonsRect.anchoredPosition = new Vector2(currentInteriorPadding, myButtonsRect.anchoredPosition.y);
                myContentsRect.anchoredPosition = new Vector2(-currentInteriorPadding, myContentsRect.anchoredPosition.y);
            }
            else
            {
                myButtonsRect.anchoredPosition = new Vector2(-currentInteriorPadding, myButtonsRect.anchoredPosition.y);
                myContentsRect.anchoredPosition = new Vector2(currentInteriorPadding, myContentsRect.anchoredPosition.y);
            }

            UpdateContentWidth();
        }
        public void UpdateDragGripDistance(float newDragGripDistance)
        {
            if (newDragGripDistance < 0.0f)
            {
                newDragGripDistance = 0.0f;
            }

            currentDragGripDistance = newDragGripDistance;
            myDragGripsRect.anchoredPosition = new Vector2(isRightSidePanel ? -currentDragGripDistance : currentDragGripDistance, myDragGripsRect.anchoredPosition.y);
        }
//#endif


        // EVENT LISTENERS
        public void ToggleOpen()
        {
            if (isOpen) // Close the panel.
            {
                myContent.SetActive(false);
                myDragGrip.SetActive(false);

                myRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentClosedPanelWidth);

                myButtonText.text = "open";
            }
            else // Open the panel.
            {
                myContent.SetActive(true);
                myDragGrip.SetActive(true);

                myRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentOpenPanelWidth);

                myButtonText.text = "close";
            }

            isOpen = !isOpen;
        }

        public void DragResize(BaseEventData eventData)
        {
            // Having to cast here as Unity's UI event thing is being a fuck.
            PointerEventData pointerEventData = (PointerEventData)eventData;
            if (pointerEventData == null)
            {
                return;
            }

            if (isOpen)
            {
                float newPanelWidth = currentOpenPanelWidth;
                newPanelWidth += isRightSidePanel ? -pointerEventData.delta.x : pointerEventData.delta.x;
                UpdatePanelWidth(newPanelWidth);
            }
        }


        // BUILT IN
        [ExecuteInEditMode]
        private void Awake()
        {
            Init();
        }
    }
}
