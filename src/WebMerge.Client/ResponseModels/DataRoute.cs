using Newtonsoft.Json;
using WebMerge.Client.Converters;

namespace WebMerge.Client.ResponseModels
{
    public class DataRoute
    {
        [JsonProperty("id")]
        [JsonConverter(typeof (WriteToStringConverter))]
        public int Id { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}