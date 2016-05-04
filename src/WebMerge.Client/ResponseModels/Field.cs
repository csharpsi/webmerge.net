using Newtonsoft.Json;

namespace WebMerge.Client.ResponseModels
{
    public class Field
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public Field()
        {
        }

        public Field(string key, string name)
        {
            Key = key;
            Name = name;
        }
    }
}