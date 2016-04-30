using System;
using System.Configuration;

namespace WebMerge.Client
{
    public class WebMergeConfiguration : IApiConfigurator
    {
        public string ApiKey { get; } = ConfigurationManager.AppSettings["WebMerge.ApiKey"];
        public string ApiSecret { get; } = Environment.GetEnvironmentVariable("WebMerge.ApiSecret");
        public Uri BaseUri { get; } = new Uri("https://www.webmerge.me");
    }
}