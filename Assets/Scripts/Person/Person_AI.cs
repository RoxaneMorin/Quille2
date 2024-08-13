using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class Person_AI : MonoBehaviour
    {
        // VARIABLES

        // References
        [SerializeField, InspectorReadOnly] private Person myBasePerson;
        // Do we need additional controller references here?

        // Values.
        // Percentile thresholds at which the character AI will consider a need to require its attention.
        [SerializeField] private float noticeBasicNeed = Constants.DEFAULT_NOTICE_BASIC_NEED;
        [SerializeField] private float noticeSubjectiveNeed = Constants.DEFAULT_NOTICE_SUBJECTIVE_NEED;

        // Checks/bools.
        [SerializeField] private bool inAction; // Is the character currently absorbed in something? 
        // Info about the  action: reference, potentially its interuptability level.

        // PROPERTIES
        public float NoticeBasicNeed
        {
            get { return noticeBasicNeed; }
            set
            {
                if (value > Constants.MAX_THRESHOLD_NEGATIVE + 0.05f)
                {
                    noticeBasicNeed = Constants.MAX_THRESHOLD_NEGATIVE + 0.05f;
                }
                else if (value < Constants.MIN_THRESHOLD_NEGATIVE + 0.1f)
                {
                    noticeBasicNeed = Constants.MIN_THRESHOLD_NEGATIVE + 0.1f;
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
                if (value > Constants.MAX_THRESHOLD_NEGATIVE + 0.05f)
                {
                    noticeSubjectiveNeed = Constants.MAX_THRESHOLD_NEGATIVE + 0.05f;
                }
                else if (value < Constants.MIN_THRESHOLD_NEGATIVE + 0.1f)
                {
                    noticeSubjectiveNeed = Constants.MIN_THRESHOLD_NEGATIVE + 0.1f;
                }
                else
                {
                    noticeSubjectiveNeed = value;
                }
            }
        }



        // METHODS

        // AI Logic
        void NeedMonitorLoop()
        {
            //Debug.Log("In NeedMonitorLoop");

            // Even X seconds, verify if:

            //-> Any basic need is at or below notice. If so, do action unless otherwise occupied.
            (BasicNeedSO, float) neediestBasicNeed = myBasePerson.MyNeedController.PerformBasicNeedCheck();
            if (neediestBasicNeed.Item2 <= noticeBasicNeed)
            {
                Debug.Log(string.Format("Something must be done about our {0} need.", neediestBasicNeed.Item1.NeedName));

                // Try to fullfil the need, unless the character is engaged in an action nuninterrupted by Notice.
            }
            // Else. Will it need to be an actual else clause?

            //-> Any subjective need is at or below notice. If so, do action unless otherwise occupied.
            (SubjectiveNeedSO, bool, float) neediestSubjectiveNeed = myBasePerson.MyNeedController.PerformSubjectiveNeedCheck();
            if (neediestSubjectiveNeed.Item3 <+ noticeSubjectiveNeed)
            {
                Debug.Log(string.Format("Something must be done about our {0} need.", (neediestSubjectiveNeed.Item2 ? neediestSubjectiveNeed.Item1.NeedNameRight : neediestSubjectiveNeed.Item1.NeedNameLeft)));

                // Try to fullfil the need, unless the character is engaged in an action nuninterrupted by Notice.
            }

            // Once the action is done, check again in case multiple needs were at or below notice.
            // Should it reinvoke itself here, or run as a coroutine?

            // Receiving a need Warning, Critical or Failure event can overwrite other actions depending on the importance of said need.
        }



        // Init.
        void Init()
        {
            // Fetch our various components.
            myBasePerson = GetComponent<Person>();
        }


        // Built in.

        // Start is called before the first frame update
        void Start()
        {
            Init();

            InvokeRepeating("NeedMonitorLoop", 5f, 5f);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
