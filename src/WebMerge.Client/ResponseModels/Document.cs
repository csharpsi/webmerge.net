using System.Collections.Generic;
using Newtonsoft.Json;
using WebMerge.Client.Converters;
using WebMerge.Client.Enums;

namespace WebMerge.Client.ResponseModels
{
    public class Document
    {
        [JsonProperty("id")]
        [JsonConverter(typeof (WriteToStringConverter))]
        public int Id { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof (EnumConverter))]
        public DocumentType DocumentType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("output")]
        [JsonConverter(typeof (EnumConverter))]
        public DocumentOutputType OutputType { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("size_width")]
        public string SizeWidth { get; set; }

        [JsonProperty("size_height")]
        public string SizeHeight { get; set; }

        [JsonProperty("active")]
        [JsonConverter(typeof (BitBooleanConverter))]
        public bool IsActive { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("fields")]
        public List<Field> Fields { get; set; }

        [JsonProperty("html", NullValueHandling = NullValueHandling.Ignore)]
        public string Html { get; set; }
    }
}