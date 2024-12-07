using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace QuilleUI
{
    public class CCUI_GenericSteppedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // A composite button that can be selected at different "fill levels".
        // Contains an icon which is always display, a "frame" which "fills" with the level of selection, and a caption visible on hover.
        // To be used as the parent class for the buttons of PersonalityTraits and Drives.

        // TODO: should right clicking refill a half full button?
        // Not sure how to handle that with the current setup :/


        // VARIABLES
        [SerializeField] protected ScriptableObject mySO;
        [SerializeField] protected bool isSelected;
        [SerializeField] protected bool isForbidden;
        [SerializeField] protected float myValue;

        [Header("References")]
        [SerializeField] protected RectTransform myRectTransform;
        [SerializeField] protected Vector2 myDefaultPosition;
        [SerializeField] protected Image myFrame;
        [SerializeField] protected Image myIcon;
        [SerializeField] protected ButtonExtended myButton;
        [SerializeField] protected TMPro.TextMeshProUGUI myCaption;

        [Header("Resources")]
        // TODO: give the option to use an array of step values instead?
        [SerializeField] protected int myStepCount = 2;
        [SerializeField] protected float myStepSize = 0.5f;
        [SerializeField] protected float myCurrentStep = 0;
        [SerializeField] protected Color myColourDefault;
        [SerializeField] protected Color myColourSelected;
        [SerializeField] protected Color myColourForbidden;
        [SerializeField] protected Color myColourSelectedeButForbidden;


        // PARAMETERS
        internal ScriptableObject MySO { get { return mySO; } set { mySO = value; } }
        internal float MyButtonValue { get { return myValue; } }
        internal bool IsSelected { get { return isSelected; } }
        internal bool IsForbidden { get { return isForbidden; } }

        internal bool IsSelectedButForbidden { get { return isSelected && isForbidden; } }
        internal UnityEngine.Vector2 MyDefaultPosition { get { return myDefaultPosition; } set { myDefaultPosition = value; } }


        // EVENTS
        public event SteppedButtonUpdate SteppedButtonUpdated;



        // METHODS

        // EVENT LISTENERS
        public virtual void OnSteppedButtonClicked()
        {
            // If the button is not selected, do so and fill it completely.
            // If it is full or mostly filled, decrease it.�
            // If it is almost empty, do so and unselect it.

            if (!isSelected)
            {
                Select();
                SteppedButtonUpdated?.Invoke(this, true);
            }
            else if (myValue > myStepSize && !isForbidden)
            {
                myCurrentStep--;
                SetValueAndFill(myCurrentStep * myStepSize);
                SteppedButtonUpdated?.Invoke(this, false);
            }
            else
            {
                Unselect();
                SteppedButtonUpdated?.Invoke(this, true);
            }
        }
        public virtual void OnSteppedButtonRightClicked()
        {
            // If the button is not selected, do so and fill it completely.
            // If the button is currently partially full, increase it.
            // If it is forbidden, unselect it.

            if (!isSelected)
            {
                Select();
                SteppedButtonUpdated?.Invoke(this, true);
            }
            else if (myCurrentStep < myStepCount && !isForbidden)
            {
                myCurrentStep++;
                SetValueAndFill(myCurrentStep * myStepSize);
                SteppedButtonUpdated?.Invoke(this, false);
            }
            else if (isForbidden)
            {
                Unselect();
                SteppedButtonUpdated?.Invoke(this, true);
            }
        }
        public virtual void OnSteppedButtonMiddleClicked()
        {
            // If the button is not selected, do so and fill it completely.
            // If it is, unselect and empty it.

            if (!isSelected)
            {
                Select();
                SteppedButtonUpdated?.Invoke(this, true);
            }
            else
            {
                Unselect();
                SteppedButtonUpdated?.Invoke(this, true);
            }
        }


        // UTILITY
        public virtual void SetValueAndFill(float value)
        {
            myValue = value;
            myFrame.fillAmount = value;
        }

        public virtual void Select()
        {
            // Select at full value.
            Select(myStepCount);
        }
        public virtual void Select(float atValue)
        {
            isSelected = true;

            // Careful, invalid value may cause errors.
            SetValueAndFill(atValue);
            myCurrentStep = atValue / myStepSize;
            myFrame.color = myColourSelected;
        }
        public virtual void Select(int atStep)
        {
            isSelected = true;
            
            SetValueAndFill(atStep * myStepSize);
            myCurrentStep = atStep;
            myFrame.color = myColourSelected;
        }

        public virtual void Unselect()
        {
            isSelected = false;

            SetValueAndFill(0f);
            myCurrentStep = 0;
            myFrame.color = myColourDefault;
        }

        public virtual void Forbid(bool isSelected)
        {
            isForbidden = true;

            if (isSelected)
            {
                // Remain interactable so the player may remove it.
                myFrame.color = myColourSelectedeButForbidden;
            }
            else
            {
                myButton.interactable = false;
                myFrame.color = myColourForbidden;
            }
        }
        public virtual void Permit(bool isSelected)
        {
            isForbidden = false;
            myButton.interactable = true;

            if (isSelected)
            {
                myFrame.color = myColourSelected;
            }
            else
            {
                myFrame.color = myColourDefault;
            }
        }

        public virtual void ChangeParentAndPosition(Transform newParent, UnityEngine.Vector2 newPosition)
        {
            transform.SetParent(newParent);
            myRectTransform.anchoredPosition = newPosition;
        }

        public virtual void RandomizeValueAndSelect()
        {
            int targetStepCount = RandomExtended.RangeInt(1, myStepCount);

            Select(targetStepCount);
        }


        // INIT
        protected virtual void FetchComponents()
        {
            myRectTransform = gameObject.GetComponent<RectTransform>();
            myDefaultPosition = new Vector2(myRectTransform.anchoredPosition.x, myRectTransform.anchoredPosition.y);

            myFrame = gameObject.GetComponent<Image>();

            myButton = gameObject.GetComponentInChildren<ButtonExtended>(true);
            myIcon = myButton.image;

            myCaption = gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
        }
        protected virtual void SetStepVariables()
        {
            myStepSize = 1.0f / myStepCount;
            myCurrentStep = 0;
        }

        public virtual void Init(ScriptableObject sourceSO)
        {
            mySO = sourceSO;

            FetchComponents();

            SetStepVariables();
            myCaption.gameObject.SetActive(false);
        }


        // BUILT IN
        void Start()
        {
            //Init(null);
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

