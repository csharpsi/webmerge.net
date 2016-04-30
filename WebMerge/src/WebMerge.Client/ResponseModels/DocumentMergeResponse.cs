using Newtonsoft.Json;

namespace WebMerge.Client.ResponseModels
{
    public class DocumentMergeResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; } 
    }
}