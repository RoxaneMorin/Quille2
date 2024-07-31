using System;
using UnityEngine;
using Newtonsoft.Json;
using AYellowpaper.SerializedCollections;
using Quille;

namespace Newtonsoft.Json.UnityConverters.Quille
{
    public class PersonalityAxesDictConverter : JsonConverter<SerializedDictionary<PersonalityAxeSO, float>>
    {
        public override SerializedDictionary<PersonalityAxeSO, float> ReadJson(JsonReader reader, Type objectType, SerializedDictionary<PersonalityAxeSO, float> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            SerializedDictionary<PersonalityAxeSO, float> newDict = new SerializedDictionary<PersonalityAxeSO, float>();

            string currentSOname = null;
            Double currentValue = 0;

            //int i = 0;

            while (reader.Read())
            {
                //Debug.Log(i + ": " + reader.TokenType);
                //i++;

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    switch (reader.Value)
                    {
                        case "Key":
                            currentSOname = reader.ReadAsString();
                            break;
                        case "Value":
                            currentValue = reader.ReadAsDouble() ?? 0; // TODO: Find why I can't simply use "ReadAsFloat()" here :/
                            break;
                    }
                }
                if (reader.TokenType == JsonToken.EndObject)
                {
                    // TODO: Figure out how to return & deserialize with the existing converter.
                    string path = Constants.PATH_PERSONALITYAXES + currentSOname;
                    PersonalityAxeSO currentSO = Resources.Load<PersonalityAxeSO>(path);
                    newDict.Add(currentSO, (float)currentValue);
                }
                if (reader.TokenType == JsonToken.EndArray)
                {
                    return newDict;
                }
            }
            return null;
        }
        public override void WriteJson(JsonWriter writer, SerializedDictionary<PersonalityAxeSO, float> value, JsonSerializer serializer)
        {
            //writer.WritePropertyName(nameof(value));

            writer.WriteStartArray();
            foreach (var pair in value)
            {
                writer.WriteStartObject();

                // Write SO type.
                writer.WritePropertyName("Key");
                writer.WriteValue(pair.Key.name);

                // Write float value.
                writer.WritePropertyName("Value");
                writer.WriteValue(pair.Value);

                writer.WriteEndObject();
            }
            writer.WriteEnd();

            //writer.WriteValue(value.name);
        }
    }
}

