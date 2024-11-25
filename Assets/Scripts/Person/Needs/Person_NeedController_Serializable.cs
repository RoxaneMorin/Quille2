using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Quille
{
    public partial class Person_NeedController : MonoBehaviour
    {
        // SUBCLASS
        [System.Serializable]
        public class NeedController_Data
        {
            // VARIABLES
            [SerializeField] private BasicNeed[] myBasicNeeds;
            [SerializeField] private SubjectiveNeed[] mySubjectiveNeeds;



            // PROPERTIES & GETTERS/SETTERS
            public BasicNeed[] MyBasicNeeds
            {
                get { return myBasicNeeds; }
                set { myBasicNeeds = value; }
            }
            public SubjectiveNeed[] MySubjectiveNeeds
            {
                get { return mySubjectiveNeeds; }
                set { mySubjectiveNeeds = value; }
            }



            // CONSTRUCTOR
            public NeedController_Data()
            {

            }
        }



        // METHODS

        // SAVE
        internal string SaveToJSON()
        {
            return JsonConvert.SerializeObject(myNeedData, Formatting.Indented);

            // TODO: where to actually write this to a file?
        }

        // LOAD
        internal void LoadFromJSON(string jsonString)
        {
            myNeedData = JsonConvert.DeserializeObject<NeedController_Data>(jsonString);

            CreateAndPopulateBasicNeedsMapped();
            CreateAndPopulateSubjectiveNeedsMapped();
        }

        // UTILITY
        // Create need dictionaries from existing need arrays;
        private void CreateAndPopulateBasicNeedsMapped()
        {
            myBasicNeedsMapped = new SerializedDictionary<BasicNeedSO, BasicNeed>();

            foreach (BasicNeed basicNeed in myNeedData.MyBasicNeeds)
            {
                myBasicNeedsMapped.Add(basicNeed.NeedSO, basicNeed);
            }
        }
        private void CreateAndPopulateSubjectiveNeedsMapped()
        {
            mySubjectiveNeedsMapped = new SerializedDictionary<SubjectiveNeedSO, SubjectiveNeed>();

            foreach (SubjectiveNeed subjectiveNeed in myNeedData.MySubjectiveNeeds)
            {
                mySubjectiveNeedsMapped.Add(subjectiveNeed.NeedSO, subjectiveNeed);
            }
        }
    }
}