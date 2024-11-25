using System;
using Newtonsoft.Json.UnityConverters.Helpers;
using AYellowpaper.SerializedCollections;
using Quille;

namespace Newtonsoft.Json.UnityConverters.Quille
{
    public class JsonConverter_InterestSO : JsonConverter<InterestSO>
    {
        public override InterestSO ReadJson(JsonReader reader, Type objectType, InterestSO existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string jsonValue = (string)reader.Value;
            return ConstantsAndHelpers.loadInterestSO(jsonValue);
        }
        public override void WriteJson(JsonWriter writer, InterestSO value, JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }
    }

    public class JsonPartialConverter_InterestSOFloatDict : PartialConverter<SerializedDictionary<InterestSO, float>>
    {
        protected override void ReadValue(ref SerializedDictionary<InterestSO, float> value, string name, JsonReader reader, JsonSerializer serializer)
        {
            InterestSO theSO = ConstantsAndHelpers.loadInterestSO(name);
            float theScore = reader.ReadAsFloat() ?? 0f;

            // TODO: make sure this works on an empty dict.
            value[theSO] = theScore;
        }
        protected override void WriteJsonProperties(JsonWriter writer, SerializedDictionary<InterestSO, float> value, JsonSerializer serializer)
        {
            foreach (var pair in value)
            {
                writer.WritePropertyName(pair.Key.name);
                writer.WriteValue(pair.Value);
            }
        }
    }
}