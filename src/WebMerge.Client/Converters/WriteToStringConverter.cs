using System;
using Newtonsoft.Json;

namespace WebMerge.Client.Converters
{
    public class WriteToStringConverter : JsonConverter
    {
        public override bool CanRead { get; } = false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }

        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new System.NotImplementedException();
        }
    }
}