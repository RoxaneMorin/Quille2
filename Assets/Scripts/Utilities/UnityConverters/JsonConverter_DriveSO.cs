using System;
using Newtonsoft.Json.UnityConverters.Helpers;
using AYellowpaper.SerializedCollections;
using Quille;

namespace Newtonsoft.Json.UnityConverters.Quille
{
    // JsonConverters for instances of DriveSO to be serialized as and deserialized from their ressource names, including as part of dictionaries of Drive scores.


    public class JsonConverter_DriveSO : JsonConverter<DriveSO>
    {
        public override DriveSO ReadJson(JsonReader reader, Type objectType, DriveSO existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string jsonValue = (string)reader.Value;
            return ConstantsAndHelpers.loadDriveSO(jsonValue);
        }
        public override void WriteJson(JsonWriter writer, DriveSO value, JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }
    }

    public class JsonPartialConverter_DriveSOFloatDict : PartialConverter<SerializedDictionary<DriveSO, float>>
    {
        protected override void ReadValue(ref SerializedDictionary<DriveSO, float> value, string name, JsonReader reader, JsonSerializer serializer)
        {
            DriveSO theSO = ConstantsAndHelpers.loadDriveSO(name);
            float theScore = reader.ReadAsFloat() ?? 0f;

            if (theScore > 0) // Precaution against invalid (negative or zero) drive scores.
            {
                // TODO: make sure this works on an empty dict.
                value[theSO] = theScore;
            }
            else if (value.ContainsKey(theSO))
            {
                value.Remove(theSO);
            }
        }
        protected override void WriteJsonProperties(JsonWriter writer, SerializedDictionary<DriveSO, float> value, JsonSerializer serializer)
        {
            foreach (var pair in value)
            {
                writer.WritePropertyName(pair.Key.name);
                writer.WriteValue(pair.Value);
            }
        }
    }
}

