using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class NeedController : MonoBehaviour
    {
        // TEMP
        // Have these be loaded automatically from the resource folder instead?
        // Will have to edit the init functions.
        public BasicNeedSO[] basicNeedSOs;
        public SubjectiveNeedSO[] subjectiveNeedSOs;



        // VARIABLES
        [SerializeField]
        private BasicNeed[] myBasicNeeds;
        [SerializeField]
        private SubjectiveNeed[] mySubjectiveNeeds;

        // For use by external functions that only know of a BasicNeedSO.
        [SerializeField, HideInInspector]
        private AYellowpaper.SerializedCollections.SerializedDictionary<BasicNeedSO, BasicNeed> myBasicNeedsMapped;
        [SerializeField, HideInInspector]
        private AYellowpaper.SerializedCollections.SerializedDictionary<SubjectiveNeedSO, SubjectiveNeed> mySubjectiveNeedsMapped;


        // General need-related parameters.

        // Percentile thresholds at which the character AI will consider a need to require its attention.
        private float noticeBasicNeed = Constants.DEFAULT_NOTICE_BASIC_NEED;
        private float noticeSubjectiveNeed = Constants.DEFAULT_NOTICE_SUBJECTIVE_NEED;
        // Should these be stored instead in the PersonAI class?



        // PROPERTIES & GETTERS/SETTERS
        public BasicNeed GetBasicNeed(BasicNeedSO basicNeedSO)
        {
            try // including this just to be safe.
            {
                return myBasicNeedsMapped[basicNeedSO];
            }
            catch
            {
                return null;
            }
        }
        public SubjectiveNeed GetSubjectiveNeed(SubjectiveNeedSO subjectiveNeedSO)
        {
            try // including this just to be safe.
            {
                return mySubjectiveNeedsMapped[subjectiveNeedSO];
            }
            catch
            {
                return null;
            }
        }
        
        // Option to add or remove a need?;

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

        // Coroutine management.

        // Individual needs.
        private void StartBasicNeedDecay(BasicNeed myNeed)
        {
            //myNeed.CurrentChangeRate = myNeed.BaseChangeRate; // is this safeguard needed?
            StartCoroutine(myNeed.AlterLevelByChangeRate());
        }
        private void StopBasicNeedDecay(BasicNeed myNeed)
        {
            StopCoroutine(myNeed.AlterLevelByChangeRate());
        }
        private void StartSubjectiveNeedDecay(SubjectiveNeed myNeed)
        {
            //myNeed.CurrentChangeRate = myNeed.BaseChangeRate; // is this safeguard needed?
            StartCoroutine(myNeed.AlterLevelByChangeRate());
        }
        private void StopSubjectiveNeedDecay(SubjectiveNeed myNeed)
        {
            StopCoroutine(myNeed.AlterLevelByChangeRate());
        }

        // Array of needs.
        private void StartBasicNeedDecay(BasicNeed[] myBasicNeeds)
        {
            foreach (BasicNeed myNeed in myBasicNeeds)
            {
                StartBasicNeedDecay(myNeed);
            }
        }
        private void StopBasicNeedDecay(BasicNeed[] myBasicNeeds) 
        {
            foreach (BasicNeed myNeed in myBasicNeeds)
            {
                StopBasicNeedDecay(myNeed);
            }
        }
        private void StartSubjectiveNeedDecay(SubjectiveNeed[] myBasicNeeds)
        {
            foreach (SubjectiveNeed myNeed in mySubjectiveNeeds)
            {
                StartSubjectiveNeedDecay(myNeed);
            }
        }
        private void StopSubjectiveNeedDecay(BasicNeed[] SubjectiveNeed)
        {
            foreach (SubjectiveNeed myNeed in mySubjectiveNeeds)
            {
                StopSubjectiveNeedDecay(myNeed);
            }
        }


        // Init.
        private void Init()
        {
            InitBasicNeeds();
            InitSubjectiveNeeds();
        }
        private void InitBasicNeeds()
        {
            
            
            myBasicNeeds = new BasicNeed[basicNeedSOs.Length];
            for (int i = 0; i < basicNeedSOs.Length; i++)
            {
                myBasicNeeds[i] = new BasicNeed(basicNeedSOs[i]);
                myBasicNeedsMapped.Add(basicNeedSOs[i], myBasicNeeds[i]);

                Debug.Log(myBasicNeeds[i].ToString());
            }

            StartBasicNeedDecay(myBasicNeeds);
        }
        private void InitSubjectiveNeeds()
        {
            mySubjectiveNeeds = new SubjectiveNeed[subjectiveNeedSOs.Length];
            for (int i = 0; i < subjectiveNeedSOs.Length; i++)
            {
                mySubjectiveNeeds[i] = new SubjectiveNeed(subjectiveNeedSOs[i]);
                mySubjectiveNeedsMapped.Add(subjectiveNeedSOs[i], mySubjectiveNeeds[i]);

                Debug.Log(mySubjectiveNeeds[i].ToString());
            }

            StartSubjectiveNeedDecay(mySubjectiveNeeds);
        }


        // Testing.
        private void RandomizeNeedChangeRates(float min, float max)
        {
            foreach (BasicNeed need in myBasicNeeds)
            {
                float randomRate = Random.Range(min, max);
                need.CurrentChangeRate = randomRate;
            }

            foreach (SubjectiveNeed need in mySubjectiveNeeds)
            {
                float randomRateLeft = Random.Range(min, max);
                float randomRateRight = Random.Range(min, max);
                need.CurrentChangeRate = (randomRateLeft, randomRateRight);

                need.AverageLocalAiPriorityWeighting();
            }
        }
        private void SortAndPrintNeedInfo()
        {
            BasicNeed.SortByFulfillmentDelta(myBasicNeeds, true, true);
            SubjectiveNeed.SortByFulfillmentDeltaofNeediest(mySubjectiveNeeds, true, true);

            foreach (BasicNeed need in myBasicNeeds)
            {
                Debug.Log(need.ToString());
            }

            foreach (SubjectiveNeed need in mySubjectiveNeeds)
            {
                Debug.Log(need.ToString());
            }
        }

        public void ModulateSubjectiveNeeds(BasePerson sourceBasePerson)
        {
            foreach (SubjectiveNeed need in mySubjectiveNeeds)
            {
                need.Init(sourceBasePerson);
            }
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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                RandomizeNeedChangeRates(-0.005f, 0.001f);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                SortAndPrintNeedInfo();
            }
        }
    }
}
