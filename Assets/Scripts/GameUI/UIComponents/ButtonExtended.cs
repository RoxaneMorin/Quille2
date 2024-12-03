using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace QuilleUI
{
    public class ButtonExtended : Button
    {
        // The UnityEngine.UI.Button extended to recognize right and middle clicks, and throw distinct events for them.


        // EVENTS
        [SerializeField] private ButtonClickedEvent m_OnRightClick = new ButtonClickedEvent();
        [SerializeField] private ButtonClickedEvent m_OnMiddleClick = new ButtonClickedEvent();

        public ButtonClickedEvent onRightClick
        {
            get { return m_OnRightClick; }
            set { m_OnRightClick = value; }
        }
        public ButtonClickedEvent onMiddleClick
        {
            get { return m_OnMiddleClick; }
            set { m_OnMiddleClick = value; }
        }



        // METHODS

        // OVERRIDES
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                base.OnPointerClick(eventData);
            }
            else
            {
                AlternatePress(eventData);
            }
        }


        // REPEATS :'(
        // Some of the original button's code is private, cue the need for this.
        protected void AlternatePress(PointerEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                UISystemProfilerApi.AddMarker("Button.onRightClick", this);
                m_OnRightClick.Invoke();
            }
            else // Middle click.
            {
                UISystemProfilerApi.AddMarker("Button.onMiddleClick", this);
                m_OnMiddleClick.Invoke();
            }

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishPress());
        }

        protected IEnumerator OnFinishPress()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
    }
}

