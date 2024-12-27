using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World;

namespace Quille
{
    // The monoBehaviour driving characters' actual behaviours.
    // At the moment, need monitoring is handled by repeated method invokation. This may change in the future.
    // So far, JSON serialization has not been implemented, though it should be once basic interactions are implemented.
    // (I'm unsure whether systems for characters remembering past actions should be implemented here or elsewhere.)


    public class Person_AI : MonoBehaviour
    {
        // VARIABLES

        // References
        [SerializeField, InspectorReadOnly] private Person myBasePerson;
        [SerializeField, InspectorReadOnly] private Person_NeedController myNeedController;
        // Do we need additional controller references here?


        // Values.
        // Percentile thresholds at which the character AI will consider a need to require its attention.
        [SerializeField] private float noticeBasicNeed = Constants_Quille.DEFAULT_NOTICE_BASIC_NEED;
        [SerializeField] private float noticeSubjectiveNeed = Constants_Quille.DEFAULT_NOTICE_SUBJECTIVE_NEED;

        // TODO: modulate notices based on personality.


        // Checks/bools.
        [SerializeField] private bool inAction; // Is the character currently absorbed in something? 
        // Current interaction;
        // TODO: keep this in the interaction instead?
        // TODO: set on the basis of need/interaction type?
        [SerializeField] private bool reactToNotice;
        [SerializeField] private bool reactToWarning;
        [SerializeField] private bool reactToCritical;


        // Temp
        [SerializeField] private World_Area currentArea;
        // TODO: how will a person know the area they are in?



        // PROPERTIES
        public float NoticeBasicNeed
        {
            get { return noticeBasicNeed; }
            set
            {
                if (value > Constants_Quille.MAX_THRESHOLD_NEGATIVE + 0.05f)
                {
                    noticeBasicNeed = Constants_Quille.MAX_THRESHOLD_NEGATIVE + 0.05f;
                }
                else if (value < Constants_Quille.MIN_THRESHOLD_NEGATIVE + 0.1f)
                {
                    noticeBasicNeed = Constants_Quille.MIN_THRESHOLD_NEGATIVE + 0.1f;
                }
                else
                {
                    noticeBasicNeed = value;
                }
            }
        }
        public float NoticeSubjectiveNeed
        {
            get { return noticeSubjectiveNeed; }
            set
            {
                if (value > Constants_Quille.MAX_THRESHOLD_NEGATIVE + 0.05f)
                {
                    noticeSubjectiveNeed = Constants_Quille.MAX_THRESHOLD_NEGATIVE + 0.05f;
                }
                else if (value < Constants_Quille.MIN_THRESHOLD_NEGATIVE + 0.1f)
                {
                    noticeSubjectiveNeed = Constants_Quille.MIN_THRESHOLD_NEGATIVE + 0.1f;
                }
                else
                {
                    noticeSubjectiveNeed = value;
                }
            }
        }



        // METHODS

        // TODO: how will a person know they are too busy to notice a new need?
        // Should notice be able to interupt any interaction at all?

        // Once an action is done, check again in case multiple needs were at or below notice.
        // Should the loop reinvoke itself here, or run as a coroutine?

        // Receiving a need Warning, Critical or Failure event can overwrite other actions depending on the importance of said need.



        // AI Logic
        protected void NeedMonitorLoop()
        {
            // Even X seconds,

            //-> Collect basic needs at or below notice. 
            BasicNeedSO[] needyBasicNeeds = myNeedController.PerformBasicNeedCheck(noticeBasicNeed);
            // If any were found,
            foreach(BasicNeedSO need in needyBasicNeeds)
            {
                Debug.Log(string.Format("Something must be done about our {0} need.", need.NeedName));

                // Look for a fitting interaction to fulfill this need, or the next in line.
                FindBestLocalInteractionFor(need, currentArea);

                // Break away toward the interaction.
            }
            // Else...


            //-> Collect subjective needs at or below notice. 
            BasicNeedSO[] needySubjectiveNeeds = myNeedController.PerformSubjectiveNeedCheck(noticeSubjectiveNeed);
            // If any were found,
            foreach (BasicNeedSO need in needySubjectiveNeeds)
            {
                Debug.Log(string.Format("Something must be done about our {0} need.", need.NeedName));

                // Look for a fitting interaction to fulfill this need, or the next in line.
                FindBestLocalInteractionFor(need, currentArea);

                // Break away toward the interaction.
            }
            // Else...


            // Look for an interaction that fulfills a drive, etc.
        }


        // UTILITY
        protected LocalInteraction FindBestLocalInteractionFor(BasicNeedSO thisNeed, World_Area inThisArea)
        {
            List<LocalInteraction> relevantInteractions = inThisArea.LocalInteractionsFor(thisNeed);

            if (relevantInteractions.Count == 0)
            {
                return null;
            }
            else if (relevantInteractions.Count == 1)
            {
                return relevantInteractions[0];
            }

            LocalInteraction currentBestInteraction = null;
            float currentBestInteractionScore = float.MinValue;
            foreach (LocalInteraction interaction in relevantInteractions)
            {
                if (interaction != null && interaction.ValidateFor(myBasePerson))
                {
                    float interactionScore = interaction.ScoreFor(myBasePerson);
                    Debug.Log(string.Format("{0}'s score for the {1}: {2}", myBasePerson.MyPersonCharacter.FirstAndLastName, interaction, interactionScore));

                    if (interactionScore > currentBestInteractionScore)
                    {
                        currentBestInteractionScore = interactionScore;
                        currentBestInteraction = interaction;
                    }
                }
            }

            Debug.Log(string.Format("The best match found for {0}'s {1} need is the {2}.", myBasePerson.MyPersonCharacter.FirstAndLastName, thisNeed.NeedName, currentBestInteraction));
            return currentBestInteraction;
        }



        // INIT
        public void Init()
        {
            // Fetch our various components.
            myBasePerson = GetComponent<Person>();
            myNeedController = GetComponent<Person_NeedController>();
        }


        // BUILT IN
        void Start()
        {
            Init();

            InvokeRepeating("NeedMonitorLoop", 5f, 5f);
            // TODO: do as a coroutine instead? Call it from itself/after an interaction is done?
        }

        void Update()
        {

        }
    }
}
