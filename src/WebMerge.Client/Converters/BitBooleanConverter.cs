using System;
using Newtonsoft.Json;

namespace WebMerge.Client.Converters
{
    public class BitBooleanConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var text = "0";

            if (value is bool)
            {
                text = (bool) value ? "1" : "0";
            }

            writer.WriteValue(text);
        }

        public override bool CanConvert(Type objectType) => objectType == typeof (bool);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var text = reader.Value?.ToString();

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            return text.Equals("1");
        }
    }
}