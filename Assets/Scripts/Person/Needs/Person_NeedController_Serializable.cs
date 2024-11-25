using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Quille
{
    public partial class Person_NeedController : MonoBehaviour
    {
        // METHODS

        // SAVE
        internal string SaveToJSON()
        {
            JObject jsonNeedController = new JObject();

            string jsonBasicNeeds = JsonConvert.SerializeObject(myBasicNeeds, Formatting.Indented);
            string jsonSubjectiveNeeds = JsonConvert.SerializeObject(mySubjectiveNeeds, Formatting.Indented);

            jsonNeedController.Add("myBasicNeeds", JToken.Parse(jsonBasicNeeds));
            jsonNeedController.Add("mySubjectiveNeeds", JToken.Parse(jsonSubjectiveNeeds));

            return jsonNeedController.ToString(Formatting.Indented);
        }

        // LOAD
        internal void LoadFromJSON(string sourceJSON)
        {
            JObject sourceJSONObject = JObject.Parse(sourceJSON);

            string jsonBasicNeeds = sourceJSONObject.GetValue("myBasicNeeds").ToString(Formatting.Indented);
            string jsonSubjectiveNeeds = sourceJSONObject.GetValue("mySubjectiveNeeds").ToString(Formatting.Indented);

            myBasicNeeds = JsonConvert.DeserializeObject<BasicNeed[]>(jsonBasicNeeds);
            mySubjectiveNeeds = JsonConvert.DeserializeObject<SubjectiveNeed[]>(jsonSubjectiveNeeds);

            CreateAndPopulateBasicNeedsMapped();
            CreateAndPopulateSubjectiveNeedsMapped();
        }

        // UTILITY
        // Create need dictionaries from existing need arrays, as used by the JSON loader;
        private void CreateAndPopulateBasicNeedsMapped()
        {
            myBasicNeedsMapped = new SerializedDictionary<BasicNeedSO, BasicNeed>();

            foreach (BasicNeed basicNeed in MyBasicNeeds)
            {
                myBasicNeedsMapped.Add(basicNeed.NeedSO, basicNeed);
            }
        }
        private void CreateAndPopulateSubjectiveNeedsMapped()
        {
            mySubjectiveNeedsMapped = new SerializedDictionary<SubjectiveNeedSO, SubjectiveNeed>();

            foreach (SubjectiveNeed subjectiveNeed in MySubjectiveNeeds)
            {
                mySubjectiveNeedsMapped.Add(subjectiveNeed.NeedSO, subjectiveNeed);
            }
        }
    }
}