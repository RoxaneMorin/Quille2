using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.UnityConverters.Helpers;
using AYellowpaper.SerializedCollections;
using Quille;

namespace Newtonsoft.Json.UnityConverters.Quille
{
    public class JsonConverter_BasicNeedSO : JsonConverter<BasicNeedSO>
    {
        public override BasicNeedSO ReadJson(JsonReader reader, Type objectType, BasicNeedSO existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string jsonValue = (string)reader.Value;
            return ConstantsAndHelpers.loadBasicNeedSO(jsonValue);
        }
        public override void WriteJson(JsonWriter writer, BasicNeedSO value, JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }
    }

    public class JsonConverter_BasicNeedSOBasicNeedDict : JsonConverter<SerializedDictionary<BasicNeedSO, BasicNeed>>
    {
        public override bool CanRead => true;

        public override SerializedDictionary<BasicNeedSO, BasicNeed> ReadJson(JsonReader reader, Type objectType, SerializedDictionary<BasicNeedSO, BasicNeed> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            SerializedDictionary<BasicNeedSO, BasicNeed> reconstructedDict = new SerializedDictionary<BasicNeedSO, BasicNeed>();

            var theArray = Newtonsoft.Json.Linq.JArray.Load(reader);
            foreach (var item in theArray)
            {
                BasicNeed theNeed = JsonConvert.DeserializeObject<BasicNeed>(item.ToString());
                BasicNeedSO theSO = theNeed.NeedSO;

                reconstructedDict.Add(theSO, theNeed);
            }

            return reconstructedDict;
        }
        public override void WriteJson(JsonWriter writer, SerializedDictionary<BasicNeedSO, BasicNeed> value, JsonSerializer serializer)
        {
            // We assume the Key will match the Value's internal BasicNeedSO, and therefore discard it.
            string jsonArray = JsonConvert.SerializeObject(value.Values, Formatting.Indented);
            writer.WriteRawValue(jsonArray);
        }
    }
}

