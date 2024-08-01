using System;
using Newtonsoft.Json.UnityConverters.Helpers;
using AYellowpaper.SerializedCollections;
using Quille;

namespace Newtonsoft.Json.UnityConverters.Quille
{
    public class JsonConverter_SubjectiveNeedSO : JsonConverter<SubjectiveNeedSO>
    {
        public override SubjectiveNeedSO ReadJson(JsonReader reader, Type objectType, SubjectiveNeedSO existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string jsonValue = (string)reader.Value;
            return ConstantsAndHelpers.loadSubjectiveNeedSO(jsonValue);
        }
        public override void WriteJson(JsonWriter writer, SubjectiveNeedSO value, JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }
    }

    public class JsonConverter_SubjectiveNeedSOSubjectiveNeedDict : JsonConverter<SerializedDictionary<SubjectiveNeedSO, SubjectiveNeed>>
    {
        public override bool CanRead => true;

        public override SerializedDictionary<SubjectiveNeedSO, SubjectiveNeed> ReadJson(JsonReader reader, Type objectType, SerializedDictionary<SubjectiveNeedSO, SubjectiveNeed> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            SerializedDictionary<SubjectiveNeedSO, SubjectiveNeed> reconstructedDict = new SerializedDictionary<SubjectiveNeedSO, SubjectiveNeed>();

            var theArray = Newtonsoft.Json.Linq.JArray.Load(reader);
            foreach (var item in theArray)
            {
                SubjectiveNeed theNeed = JsonConvert.DeserializeObject<SubjectiveNeed>(item.ToString());
                SubjectiveNeedSO theSO = theNeed.NeedSO;

                reconstructedDict.Add(theSO, theNeed);
            }

            return reconstructedDict;
        }
        public override void WriteJson(JsonWriter writer, SerializedDictionary<SubjectiveNeedSO, SubjectiveNeed> value, JsonSerializer serializer)
        {
            // We assume the Key will match the Value's internal SubjectiveNeedSO, and therefore discard it.
            string jsonArray = JsonConvert.SerializeObject(value.Values, Formatting.Indented);
            writer.WriteRawValue(jsonArray);
        }
    }
}

