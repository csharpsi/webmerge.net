using System;
using WebMerge.Client;

namespace WebMerge.Tests
{
    public class TestingEnabledConfiguration : IApiConfigurator
    {
        public string ApiKey { get; set; } = "API_KEY";
        public string ApiSecret { get; set; } = "API_SECRET";
        public Uri BaseUri { get; set; } = new Uri("https://test.io");
    }
}