using System;
using UnityEngine;
using Quille;

namespace Newtonsoft.Json.UnityConverters.Quille
{
    public class PersonalityAxeSOConverter : JsonConverter<PersonalityAxeSO>
    {
        public override PersonalityAxeSO ReadJson(JsonReader reader, Type objectType, PersonalityAxeSO existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            Debug.Log("In ReadJSON");

            string jsonValue = (string)reader.Value;
            Debug.Log(jsonValue);

            // Remove the  (Quille.PersonalityAxeSO) if needed.

            string path = Constants.PATH_PERSONALITYAXES + jsonValue;
            Debug.Log(path);

            PersonalityAxeSO theSO = Resources.Load<PersonalityAxeSO>(path);

            Debug.Log(theSO);

            return theSO;
        }
        public override void WriteJson(JsonWriter writer, PersonalityAxeSO value, JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }
    }
}


