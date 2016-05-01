using Newtonsoft.Json;

namespace WebMerge.Client.ResponseModels
{
    public class DocumentField
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public DocumentField()
        {
            
        }

        public DocumentField(string key, string name)
        {
            Key = key;
            Name = name;
        }
    }
}