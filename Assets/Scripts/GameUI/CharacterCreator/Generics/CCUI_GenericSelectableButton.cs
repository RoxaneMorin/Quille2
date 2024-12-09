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

        [Header("References")]
        [SerializeField] protected RectTransform myRectTransform;
        [SerializeField] protected Vector2 myDefaultPosition;
        [SerializeField] protected Image myIcon;
        [SerializeField] protected ButtonExtended myButton;
        [SerializeField] protected TMPro.TextMeshProUGUI myCaption;

        [Header("Resources")]
        [SerializeField] protected Color myColourDefault;
        [SerializeField] protected Color myColourSelected;


        // PROPERTIES
        internal ScriptableObject MySO { get { return mySO; } set { mySO = value; } }
        internal bool IsSelected { get { return isSelected; } }
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
        }
        public virtual void Unselect() 
        {
            isSelected = false;
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

            myButton = gameObject.GetComponent<ButtonExtended>();
            if (myButton == null)
            {
                myButton = gameObject.GetComponentInChildren<ButtonExtended>(true);
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
