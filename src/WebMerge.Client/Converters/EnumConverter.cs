using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebMerge.Client.Converters
{
    public class EnumConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var e = (Enum) value;
            var name = e.ToString("G").ToLower();

            writer.WriteValue(name);
        }
    }
}