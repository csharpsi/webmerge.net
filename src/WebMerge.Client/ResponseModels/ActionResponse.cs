using Newtonsoft.Json;
using WebMerge.Client.Converters;

namespace WebMerge.Client.ResponseModels
{
    public class ActionResponse
    {
        [JsonProperty("success")]
        [JsonConverter(typeof (BitBooleanConverter))]
        public bool Success { get; set; }
    }
}