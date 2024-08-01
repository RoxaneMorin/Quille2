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
            // TODO: figure out how to use ReadAsFloat directly.

            InterestSO theSO = ConstantsAndHelpers.loadInterestSO(name);
            value.Add(theSO, (float)reader.ReadAsDouble());
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