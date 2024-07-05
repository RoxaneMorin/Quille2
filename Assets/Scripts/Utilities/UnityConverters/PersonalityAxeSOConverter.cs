using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;
using Quille;


// TO DO: try and hook it up.

namespace Newtonsoft.Json.UnityConverters.Quille
{
    public class PersonalityAxeSOConverter : JsonConverter<PersonalityAxeSO>
    {
        public override PersonalityAxeSO ReadJson(JsonReader reader, Type objectType, PersonalityAxeSO existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string jsonValue = (string)reader.Value;
            Debug.Log(jsonValue);

            string path = "ScriptableObjects/Personality/Axes/" + jsonValue;
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


