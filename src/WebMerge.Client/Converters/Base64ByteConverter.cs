using System;
using Newtonsoft.Json;

namespace WebMerge.Client.Converters
{
    public class Base64ByteConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new System.NotImplementedException();
        }

        public override bool CanConvert(Type objectType) => objectType == typeof (byte[]);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            try
            {
                return Convert.FromBase64String(reader.Value.ToString());
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}