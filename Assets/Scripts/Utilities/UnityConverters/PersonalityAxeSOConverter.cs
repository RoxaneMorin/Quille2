using System;
using UnityEngine;
using Quille;

namespace Newtonsoft.Json.UnityConverters.Quille
{
    public class PersonalityAxeSOConverter : JsonConverter<PersonalityAxeSO>
    {
        public override PersonalityAxeSO ReadJson(JsonReader reader, Type objectType, PersonalityAxeSO existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string jsonValue = (string)reader.Value;
            string path = Constants.PATH_PERSONALITYAXES + jsonValue;

            PersonalityAxeSO theSO = Resources.Load<PersonalityAxeSO>(path);
            return theSO;
        }
        public override void WriteJson(JsonWriter writer, PersonalityAxeSO value, JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }
    }
}


