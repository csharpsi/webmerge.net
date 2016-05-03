using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebMerge.Client.ResponseModels
{
    public class MultipleFileRouteRequestState : RequestState
    {
        [JsonProperty("files")]
        public List<DataRouteFile> Files { get; set; }
    }
}