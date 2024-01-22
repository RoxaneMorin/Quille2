using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class PersonAI : MonoBehaviour
    {
        // VARIABLES

        // References
        [SerializeField, InspectorReadOnly] private BasePerson myBasePerson;
        // Do we need additional controller references here?

        // Values.
        // Percentile thresholds at which the character AI will consider a need to require its attention.
        [SerializeField, InspectorReadOnly] private float noticeBasicNeed = Constants.DEFAULT_NOTICE_BASIC_NEED;
        [SerializeField, InspectorReadOnly] private float noticeSubjectiveNeed = Constants.DEFAULT_NOTICE_SUBJECTIVE_NEED;



        // PROPERTIES
        public float NoticeBasicNeed
        {
            get { return noticeBasicNeed; }
            set
            {
                if (value > Constants.MAX_NOTICE_NEED)
                {
                    noticeBasicNeed = Constants.MAX_NOTICE_NEED;
                }
                else if (value < Constants.MIN_NOTICE_NEED)
                {
                    noticeBasicNeed = Constants.MIN_NOTICE_NEED;
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
                if (value > Constants.MAX_NOTICE_NEED)
                {
                    noticeSubjectiveNeed = Constants.MAX_NOTICE_NEED;
                }
                else if (value < Constants.MIN_NOTICE_NEED)
                {
                    noticeSubjectiveNeed = Constants.MIN_NOTICE_NEED;
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
            // Even X seconds, verify if:

            //-> Any basic need is at or below notice. If so, do action unless otherwise occupied.
            (BasicNeedSO, float) neediestBasicNeed = myBasePerson.MyNeedController.PerformBasicNeedCheck();
            if (neediestBasicNeed.Item2 <= noticeBasicNeed)
            {
                Debug.Log(string.Format("Something must be done about our {0} need.", neediestBasicNeed.Item1.NeedName));
            }
            // Else. Will it need to be an actual else clause?

            //-> Any subjective need is at or below notice. If so, do action unless otherwise occupied.
            (SubjectiveNeedSO, bool, float) neediestSubjectiveNeed = myBasePerson.MyNeedController.PerformSubjectiveNeedCheck();
            if (neediestSubjectiveNeed.Item3 <+ noticeSubjectiveNeed)
            {
                Debug.Log(string.Format("Something must be done about our {0} need.", (neediestSubjectiveNeed.Item2 ? neediestSubjectiveNeed.Item1.NeedNameRight : neediestSubjectiveNeed.Item1.NeedNameLeft)));
            }

            // Once the action is done, check again in case multiple needs were at or below notice.
            // Should it reinvoke itself here, or run as a coroutine?

            // Receiving a need Warning, Critical or Failure event can overwrite other actions depending on the importance of said need.
        }



        // Init.
        void Init()
        {
            // Fetch our various components.
            myBasePerson = GetComponent<BasePerson>();
        }


        // Built in.

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
