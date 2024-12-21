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

        // AI Logic
        protected void NeedMonitorLoop()
        {
            //Debug.Log("In NeedMonitorLoop");

            // Even X seconds, verify if:
            // TODO: return all above the given notice ? So we can move on to the next one if no viable interaction is found.
            // TODO: else, make the need check more efficient by removing the sorting.

            // TODO: how will a person know the area they are in?

            // TODO: how will a person know they are too busy to notice a new need?

            //-> Any basic need is at or below notice. If so, do action unless otherwise occupied.
            (BasicNeedSO, float) neediestBasicNeed = myNeedController.PerformBasicNeedCheck();
            if (neediestBasicNeed.Item2 <= noticeBasicNeed)
            {
                Debug.Log(string.Format("Something must be done about our {0} need.", neediestBasicNeed.Item1.NeedName));

                // Try to fullfil the need, unless the character is engaged in an action nuninterrupted by Notice.
            }
            // Else. Will it need to be an actual else clause?

            //-> Any subjective need is at or below notice. If so, do action unless otherwise occupied.
            (SubjectiveNeedSO, BasicNeedSO, float) neediestSubjectiveNeed = myNeedController.PerformSubjectiveNeedCheck();
            if (neediestSubjectiveNeed.Item3 <= noticeSubjectiveNeed)
            {
                Debug.Log(string.Format("Something must be done about our {0} ({1}) need.", neediestSubjectiveNeed.Item2.NeedName, neediestSubjectiveNeed.Item1.NeedName));

                // Try to fullfil the need, unless the character is engaged in an action nuninterrupted by Notice.
            }

            // Once the action is done, check again in case multiple needs were at or below notice.
            // Should it reinvoke itself here, or run as a coroutine?

            // Receiving a need Warning, Critical or Failure event can overwrite other actions depending on the importance of said need.
        }


        // UTILITY
        protected LocalInteraction FindBestLocalInteractionFor(BasicNeedSO thisNeed, World_Area currentArea)
        {
            List<LocalInteraction> relevantInteractions = currentArea.LocalInteractionsFor(thisNeed);
            LocalInteraction currentBestInteraction = null;
            float currentBestInteractionScore = float.MinValue;

            foreach (LocalInteraction interaction in relevantInteractions)
            {
                if (interaction != null && interaction.ValidateFor(myBasePerson))
                {
                    float interactionScore = interaction.ScoreFor(myBasePerson);

                    if (interactionScore > currentBestInteractionScore)
                    {
                        currentBestInteractionScore = interactionScore;
                        currentBestInteraction = interaction;
                    }
                }
            }

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
