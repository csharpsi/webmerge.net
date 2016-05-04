using Newtonsoft.Json;
using WebMerge.Client.Converters;
using WebMerge.Client.Enums;

namespace WebMerge.Client.RequestModels
{
    public abstract class DocumentRequest
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof (EnumConverter))]
        public DocumentType? DocumentType { get; set; }

        [JsonProperty("output", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof (EnumConverter))]
        public DocumentOutputType? OutputType { get; set; }

        [JsonProperty("output_name", NullValueHandling = NullValueHandling.Ignore)]
        public string OutputName { get; set; }

        [JsonProperty("folder", NullValueHandling = NullValueHandling.Ignore)]
        public string Folder { get; set; }

        [JsonProperty("html", NullValueHandling = NullValueHandling.Ignore)]
        public string Html { get; set; }

        [JsonProperty("size_width", NullValueHandling = NullValueHandling.Ignore)]
        public double? SizeWidth { get; set; }

        [JsonProperty("size_height", NullValueHandling = NullValueHandling.Ignore)]
        public double? SizeHeight { get; set; }

        [JsonProperty("file_contents", NullValueHandling = NullValueHandling.Ignore)]
        public string FileContents { get; set; }

        protected DocumentRequest()
        {
        }

        protected DocumentRequest(string name)
        {
            Name = name;
        }
    }
}