using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace QuilleUI
{
    public class CCUI_GenericSelectableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // A button that is (un)selected via click, potentially moving it between areas.
        // Will display a caption upon hover.


        // VARIABLES
        [SerializeField] protected ScriptableObject mySO;
        [SerializeField] protected bool isSelected;
        [SerializeField] protected bool isForbidden;

        [Header("References")]
        [SerializeField] protected RectTransform myRectTransform;
        [SerializeField] protected Vector2 myDefaultPosition;
        [SerializeField] protected Image myIcon;
        [SerializeField] protected Button myButton;
        [SerializeField] protected TMPro.TextMeshProUGUI myCaption;

        [Header("Resources")]
        [SerializeField] protected Color myColourDefault;
        [SerializeField] protected Color myColourSelected;
        [SerializeField] protected Color myColourForbidden;
        [SerializeField] protected Color myColourSelectedeButForbidden;
        [SerializeField] protected Vector2 myCaptionPositionUnselected;
        [SerializeField] protected Vector2 myCaptionPositionSelected;


        // PROPERTIES
        internal ScriptableObject MySO { get { return mySO; } set { mySO = value; } }
        internal bool IsSelected { get { return isSelected; } }
        internal bool IsForbidden { get { return isForbidden; } }
        internal bool IsSelectedButForbidden { get { return isSelected && isForbidden; } }
        internal UnityEngine.Vector2 MyDefaultPosition { get { return myDefaultPosition; } set { myDefaultPosition = value; } }

        // EVENTS
        public virtual event SelectableButtonUpdate SelectableButtonUpdated;


        // METHODS

        // EVENT LISTENERS
        public virtual void OnButtonClicked()
        {
            if (!isSelected)
            {
                Select();
            }
            else
            {
                Unselect();
            }
        }
        public virtual void OnButtonRightClicked()
        {
            OnButtonClicked();
        }
        public virtual void OnButtonMiddleClicked()
        {
            OnButtonClicked();
        }


        // UTILITY
        public virtual void Select() 
        {
            isSelected = true;
            myCaption.rectTransform.anchoredPosition = myCaptionPositionSelected;
        }
        public virtual void Unselect() 
        {
            isSelected = false;
            myCaption.rectTransform.anchoredPosition = myCaptionPositionUnselected;
        }

        public virtual void Forbid()
        {
            isForbidden = true;
            myButton.interactable = false;
        }
        public virtual void Permit()
        {
            isForbidden = false;
            myButton.interactable = true;
        }

        public virtual void ChangeParentAndPosition(Transform newParent, UnityEngine.Vector2 newPosition)
        {
            transform.SetParent(newParent);
            myRectTransform.anchoredPosition = newPosition;
        }


        // INIT
        protected virtual void FetchComponents()
        {
            myRectTransform = gameObject.GetComponent<RectTransform>();
            myDefaultPosition = new Vector2(myRectTransform.anchoredPosition.x, myRectTransform.anchoredPosition.y);

            myButton = gameObject.GetComponent<Button>();
            if (myButton == null)
            {
                myButton = gameObject.GetComponentInChildren<Button>(true);
            }
            myIcon = myButton.image;

            myCaption = gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
        }

        public virtual void Init(ScriptableObject sourceSO)
        {
            mySO = sourceSO;

            FetchComponents();

            myCaption.gameObject.SetActive(false);
        }


        // BUILT IN
        void Start()
        {
            //Init();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            myCaption.gameObject.SetActive(true);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            myCaption.gameObject.SetActive(false);
        }
    }
}
