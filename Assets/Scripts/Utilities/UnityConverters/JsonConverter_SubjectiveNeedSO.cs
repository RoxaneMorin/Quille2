using System;
using Newtonsoft.Json.UnityConverters.Helpers;
using AYellowpaper.SerializedCollections;
using Quille;

namespace Newtonsoft.Json.UnityConverters.Quille
{
    public class JsonConverter_SubjectiveNeedSO : JsonConverter<SubjectiveNeedSO>
    {
        public override SubjectiveNeedSO ReadJson(JsonReader reader, Type objectType, SubjectiveNeedSO existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string jsonValue = (string)reader.Value;
            return ConstantsAndHelpers.loadSubjectiveNeedSO(jsonValue);
        }
        public override void WriteJson(JsonWriter writer, SubjectiveNeedSO value, JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }
    }
}

