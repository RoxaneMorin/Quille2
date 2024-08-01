using System;
using Newtonsoft.Json.UnityConverters.Helpers;
using AYellowpaper.SerializedCollections;
using Quille;

namespace Newtonsoft.Json.UnityConverters.Quille
{
    public class JsonConverter_PersonalityAxeSO : JsonConverter<PersonalityAxeSO>
    {
        public override PersonalityAxeSO ReadJson(JsonReader reader, Type objectType, PersonalityAxeSO existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string jsonValue = (string)reader.Value;
            return ConstantsAndHelpers.loadPersonalityAxeSO(jsonValue);
        }
        public override void WriteJson(JsonWriter writer, PersonalityAxeSO value, JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }
    }

    public class JsonPartialConverter_PersonalityAxeSOFloatDict : PartialConverter<SerializedDictionary<PersonalityAxeSO, float>>
    {
        protected override void ReadValue(ref SerializedDictionary<PersonalityAxeSO, float> value, string name, JsonReader reader, JsonSerializer serializer)
        {
            // TODO: figure out how to use ReadAsFloat directly.

            PersonalityAxeSO theSO = ConstantsAndHelpers.loadPersonalityAxeSO(name);
            value.Add(theSO, (float)reader.ReadAsDouble());
        }
        protected override void WriteJsonProperties(JsonWriter writer, SerializedDictionary<PersonalityAxeSO, float> value, JsonSerializer serializer)
        {
            foreach (var pair in value)
            {
                writer.WritePropertyName(pair.Key.name);
                writer.WriteValue(pair.Value);
            }
        }
    }
}


