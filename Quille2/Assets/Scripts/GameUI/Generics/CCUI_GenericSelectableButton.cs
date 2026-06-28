using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Quille;

namespace QuilleUI
{
    public class CCUI_GenericSelectableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // A button that is (un)selected via click, potentially moving it between areas.
        // Will display a caption (upon hover?).
        // Can be filtered by an associated domain.


        // VARIABLES
        [SerializeField] protected PersonalityItemSO mySO;
        [SerializeField] protected bool isSelected;
        [SerializeField] protected bool isForbidden;
        [SerializeField] protected bool isExlcudedByCurrentFilter;
        //[SerializeField] protected List<CCUI_DomainFilter> filteredBy;

        [Header("References")]
        [SerializeField] protected RectTransform myRectTransform;
        [SerializeField] protected Vector2 myDefaultPosition;
        [SerializeField] protected Image myIcon;
        [SerializeField] protected Button myButton;
        [SerializeField] protected TMPro.TextMeshProUGUI myCaption;
        protected string myDefaultCaption;

        [Header("Resources")]
        [SerializeField] protected Color myColourDefault;
        [SerializeField] protected Color myColourSelected;
        [SerializeField] protected Color myColourForbidden;
        [SerializeField] protected Color myColourSelectedeButForbidden;
        [SerializeField] protected bool alwaysShowCaption = true;
        [SerializeField] protected Vector2 myCaptionPositionUnselected;
        [SerializeField] protected Vector2 myCaptionPositionSelected;


        // PROPERTIES
        internal PersonalityItemSO MySO { get { return mySO; } set { mySO = value; } }
        internal bool IsSelected { get { return isSelected; } }
        internal bool IsForbidden { get { return isForbidden; } }
        internal bool IsSelectedButForbidden { get { return isSelected && isForbidden; } }
        internal bool IsExlcudedByCurrentFilter { get { return isExlcudedByCurrentFilter; } }
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

        public virtual void OnDomainFilterUpdated(CCUI_DomainFilter theDomainFilter)
        {
            if (theDomainFilter)
            {
                isExlcudedByCurrentFilter = !((IUseDomains)mySO).InDomain(theDomainFilter.MyDomain);
            }
            else
            {
                isExlcudedByCurrentFilter = false;
            }

            SetActiveFromStates();
        }


        // UTILITY
        public virtual void Select() 
        {
            isSelected = true;

            myCaption.rectTransform.anchoredPosition = myCaptionPositionSelected;
            RegenerateCaption();
        }
        public virtual void Unselect() 
        {
            isSelected = false;

            myCaption.rectTransform.anchoredPosition = myCaptionPositionUnselected;
            RegenerateCaption();
        }

        public virtual void Forbid()
        {
            isForbidden = true;
            SetActiveFromStates();
        }
        public virtual void Permit()
        {
            isForbidden = false;
            SetActiveFromStates();
        }
        public virtual void PermitIfValid(Quille.Person theTargetPerson, bool selectionBoxAtCapacity)
        {
            if (isSelected || !selectionBoxAtCapacity)
            {
                Permit();
            }
            else
            {
                Forbid();
            }
        }

        public virtual void Activate()
        {
            myButton.interactable = true;
            myCaption.gameObject.SetActive(true);
        }
        public virtual void Deactivate()
        {
            myButton.interactable = false;
            myCaption.gameObject.SetActive(false);
        }
        protected virtual void SetActiveFromStates()
        {
            if (!isSelected && (isForbidden || isExlcudedByCurrentFilter))
            {
                Deactivate();
            }
            else
            {
                Activate();
            }
        }

        public virtual void ChangeParentAndPosition(Transform newParent, UnityEngine.Vector2 newPosition)
        {
            transform.SetParent(newParent);
            myRectTransform.anchoredPosition = newPosition;
        }

        public void RegenerateCaption() 
        {
            myCaption.text = isSelected ? MakeNewCaption() : myDefaultCaption;
        }
        protected virtual string MakeNewCaption()
        {
            return myDefaultCaption;
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

        public virtual void Init(PersonalityItemSO sourceSO)
        {
            mySO = sourceSO;

            FetchComponents();

            myDefaultCaption = myCaption.text;
            if (!alwaysShowCaption)
            {
                myCaption.gameObject.SetActive(false);
            }
        }


        // BUILT IN
        void Start()
        {
            //Init();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (!alwaysShowCaption)
            {
                myCaption.gameObject.SetActive(true);
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (!alwaysShowCaption)
            {
                myCaption.gameObject.SetActive(false);
            }
        }
    }
}
