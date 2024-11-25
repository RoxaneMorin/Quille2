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
            // TODO: check if I can nix this subclass and just serialzie the two arrays by hand.

            // VARIABLES
            [SerializeField] private BasicNeed[] myBasicNeeds;
            [SerializeField] private SubjectiveNeed[] mySubjectiveNeeds;



            // PROPERTIES & GETTERS/SETTERS
            [JsonIgnore] public BasicNeed[] MyBasicNeeds
            {
                get { return myBasicNeeds; }
                set { myBasicNeeds = value; }
            }
            [JsonIgnore] public SubjectiveNeed[] MySubjectiveNeeds
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
        }

        // LOAD
        internal void LoadFromJSON(string jsonString)
        {
            // TODO: test what happens if myNeedData doesn't already exist.
            JsonConvert.PopulateObject(jsonString, myNeedData);

            CreateAndPopulateBasicNeedsMapped();
            CreateAndPopulateSubjectiveNeedsMapped();
        }

        // UTILITY
        // Create need dictionaries from existing need arrays, as used by the JSON loader;
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