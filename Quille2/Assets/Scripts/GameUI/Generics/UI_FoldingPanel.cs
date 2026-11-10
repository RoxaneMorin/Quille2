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
        [SerializeField] protected bool isRightSidePanel;
        [SerializeField] protected float interiorPadding;
        [SerializeField] protected float dragGripDistance;

        [SerializeField] protected float minOpenPanelWidth;
        [SerializeField] protected float maxOpenPanelWidth;

        // Runtime
        [SerializeField] protected float openPanelWidth;
        [SerializeField] protected float closedPanelWidth;
        [SerializeField] protected float contentWidth;
        [SerializeField] protected float contentHeight;
        [SerializeField] protected bool isOpen = true;

        [SerializeField] protected bool isDirty;


        // TODO:
        // - drag grip only appears (or fully appears) on hover?
        // - update layout on resolution change
        // - Add IsDirty, generally refresh the editor-side handling to better match newer UI items.


        // PROPERTIES
        public bool IsRightSidePanel 
        { 
            get { return isRightSidePanel; }
            set
            {
                isRightSidePanel = value;
                UpdatePanelSide();
            }
        }
        public float InteriorPadding 
        { 
            get { return interiorPadding; } 
            set
            {
                if (value >= 0.0f)
                {
                    interiorPadding = value;
                    SetDirty();
                }
            }
        }
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
                if (myButtonsRect != null && value > 0.0f)
                {
                    myButtonsRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
                    SetDirty();
                }
            }
        }
        public float DragGripDistance 
        { 
            get 
            { 
                if (myDragGripsRect != null)
                {
                    return Mathf.Abs(myDragGripsRect.anchoredPosition.x);
                }
                else
                {
                    return 0.0f;
                }
            }
            set
            {
                if (myDragGripsRect != null)
                {
                    myDragGripsRect.anchoredPosition = new Vector2(isRightSidePanel ? -value : value, myDragGripsRect.anchoredPosition.y);
                }
            }
        }

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
        public float OpenPanelWidth 
        { 
            get { return openPanelWidth; } 
            set
            {
                if (value >= minOpenPanelWidth && value <=  maxOpenPanelWidth)
                {
                    openPanelWidth = value;
                    SetDirty();
                }

            }
        }
        public float ClosedPanelWidth { get { return closedPanelWidth; } }

        public bool IsOpen { get { return isOpen; } }

        public bool IsDirty { get { return isDirty; } }
        public void SetDirty() { isDirty = true; }



        // METHODS
        protected void Init()
        {
            FetchComponents();
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
        public void RecalculateLayout()
        {
            contentWidth = openPanelWidth - (myButtonsRect.sizeDelta.x + interiorPadding * 2.5f);
            contentHeight = Screen.currentResolution.height - interiorPadding * 2.0f;

            closedPanelWidth = myButtonsRect.sizeDelta.x + interiorPadding * 2.0f;
        }
        public void UpdatePanelLayout(bool updatePadding = false)
        {
            RecalculateLayout();

            myRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, openPanelWidth);
            myContentsRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentWidth);

            if (updatePadding)
            {
                myButtonsRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);
                myContentsRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);

                if (isRightSidePanel)
                {
                    myButtonsRect.anchoredPosition = new Vector2(interiorPadding, myButtonsRect.anchoredPosition.y);
                    myContentsRect.anchoredPosition = new Vector2(-interiorPadding, myContentsRect.anchoredPosition.y);
                }
                else
                {
                    myButtonsRect.anchoredPosition = new Vector2(-interiorPadding, myButtonsRect.anchoredPosition.y);
                    myContentsRect.anchoredPosition = new Vector2(interiorPadding, myContentsRect.anchoredPosition.y);
                }
            }

            isDirty = false;
        }
        // TODO: review where we do what, when is what updated.
        public void UpdatePanelSide()
        {
            // anchorInteriorStuff
            // anchorOutsideStuff

            // 

            if (isRightSidePanel)
            {
                myRect.anchorMin = myRect.anchorMax = myRect.pivot = new Vector2(1.0f, 0.5f);
                myContentsRect.anchorMin = myContentsRect.anchorMax = myContentsRect.pivot = new Vector2(1.0f, 0.5f);

                myButtonsRect.anchorMin = myButtonsRect.anchorMax = myButtonsRect.pivot = new Vector2(0.0f, 0.5f);
                myDragGripsRect.anchorMin = myDragGripsRect.anchorMax = myDragGripsRect.pivot = new Vector2(0.0f, 0.5f);

                myContentsRect.anchoredPosition = new Vector2(-interiorPadding, myContentsRect.anchoredPosition.y);
                myButtonsRect.anchoredPosition = new Vector2(interiorPadding, myButtonsRect.anchoredPosition.y);
                myDragGripsRect.anchoredPosition = new Vector2(-dragGripDistance, myDragGripsRect.anchoredPosition.y);

                myDragGripImage.sprite = dragGripSpriteRight;
            }
            else
            {
                myRect.anchorMin = myRect.anchorMax = myRect.pivot = new Vector2(0.0f, 0.5f);
                myContentsRect.anchorMin = myContentsRect.anchorMax = myContentsRect.pivot = new Vector2(0.0f, 0.5f);

                myButtonsRect.anchorMin = myButtonsRect.anchorMax = myButtonsRect.pivot = new Vector2(1.0f, 0.5f);
                myDragGripsRect.anchorMin = myDragGripsRect.anchorMax = myDragGripsRect.pivot = new Vector2(1.0f, 0.5f);

                myContentsRect.anchoredPosition = new Vector2(interiorPadding, myContentsRect.anchoredPosition.y);
                myButtonsRect.anchoredPosition = new Vector2(-interiorPadding, myButtonsRect.anchoredPosition.y);
                myDragGripsRect.anchoredPosition = new Vector2(dragGripDistance, myDragGripsRect.anchoredPosition.y);

                myDragGripImage.sprite = dragGripSpriteLeft;
            }

            myRect.anchoredPosition = new Vector2(0.0f, myRect.anchoredPosition.y);
        }



        




        




        

        




        // EVENT LISTENERS
        public void ToggleOpen()
        {
            if (isOpen) // Close the panel.
            {
                myContent.SetActive(false);
                myDragGrip.SetActive(false);

                myRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, closedPanelWidth);

                myButtonText.text = "open";
            }
            else // Open the panel.
            {
                myContent.SetActive(true);
                myDragGrip.SetActive(true);

                myRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, openPanelWidth);

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
                OpenPanelWidth = openPanelWidth + (isRightSidePanel ? -pointerEventData.delta.x : pointerEventData.delta.x);
                UpdatePanelLayout();
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
