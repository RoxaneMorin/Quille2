using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class NeedController : MonoBehaviour
    {
        //public BasicNeed myNeed = new BasicNeed("Undefined", 1);


        private NeedNourishment myNourishment = new NeedNourishment();
        private BasicNeed myBasicNeed = new BasicNeed();


        public BasicNeed[] myBasicNeeds = new BasicNeed[] {  };





        private void initMyBasicNeeds()
        {
            myBasicNeeds = new BasicNeed[] { myNourishment, myBasicNeed };
        }







        private void StartBasicNeedDecay(BasicNeed myNeed)
        {
            myNeed.CurrentChangeRate = myNeed.BaseChangeRate; // is this safeguard needed?
            StartCoroutine(myNeed.AlterLevelByChangeRate());
        }
        private void StopBasicNeedDecay(BasicNeed myNeed)
        {
            StopCoroutine(myNeed.AlterLevelByChangeRate());
        }


        private void StartBasicNeedDecay(BasicNeed[] myBasicNeeds) // what would be a better name??
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






        // Start is called before the first frame update
        void Start()
        {
            initMyBasicNeeds();
            StartBasicNeedDecay(myBasicNeeds);



        } 

        // Update is called once per frame
        void Update()
        {

        }
    }
}
