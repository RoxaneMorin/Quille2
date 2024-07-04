using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;
using Quille;


// TO DO: try and hook it up.

namespace Newtonsoft.Json.UnityConverters.Quille
{
    public class PersonalityAxeSOConverter : PartialConverter<PersonalityAxeSO>
    {
        protected override void ReadValue(ref PersonalityAxeSO value, string name, JsonReader reader, JsonSerializer serializer)
        {
            value = Resources.Load<PersonalityAxeSO>("ScriptableObjects/Personality/Axes/" + reader.ReadAsString());
        }
        protected override void WriteJsonProperties(JsonWriter writer, PersonalityAxeSO value, JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }
    }
}


