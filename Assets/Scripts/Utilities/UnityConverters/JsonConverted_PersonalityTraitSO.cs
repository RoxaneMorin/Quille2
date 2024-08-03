using System;
using Newtonsoft.Json.UnityConverters.Helpers;
using AYellowpaper.SerializedCollections;
using Quille;

namespace Newtonsoft.Json.UnityConverters.Quille
{
    public class JsonConverter_PersonalityTraitSO : JsonConverter<PersonalityTraitSO>
    {
        public override PersonalityTraitSO ReadJson(JsonReader reader, Type objectType, PersonalityTraitSO existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string jsonValue = (string)reader.Value;
            return ConstantsAndHelpers.loadPersonalityTraitSO(jsonValue);
        }
        public override void WriteJson(JsonWriter writer, PersonalityTraitSO value, JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }
    }

    public class JsonPartialConverter_PersonalityTraitSOFloatDict : PartialConverter<SerializedDictionary<PersonalityTraitSO, float>>
    {
        protected override void ReadValue(ref SerializedDictionary<PersonalityTraitSO, float> value, string name, JsonReader reader, JsonSerializer serializer)
        {
            PersonalityTraitSO theSO = ConstantsAndHelpers.loadPersonalityTraitSO(name);
            float theScore = reader.ReadAsFloat() ?? 0f;

            if (theScore > 0) // Precaution against invalid negative trait scores.
            {
                value.Add(theSO, (float)reader.ReadAsDouble());
            }
        }
        protected override void WriteJsonProperties(JsonWriter writer, SerializedDictionary<PersonalityTraitSO, float> value, JsonSerializer serializer)
        {
            foreach (var pair in value)
            {
                writer.WritePropertyName(pair.Key.name);
                writer.WriteValue(pair.Value);
            }
        }
    }
}

