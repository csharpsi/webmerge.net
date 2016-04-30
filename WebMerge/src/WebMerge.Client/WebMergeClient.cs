using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebMerge.Client.ResponseModels;

namespace WebMerge.Client
{
    public class WebMergeClient : IWebMergeClient
    {
        private readonly HttpClient httpClient;
        private readonly IApiConfigurator configurator;

        public WebMergeClient()
        {
            httpClient = new HttpClient();
            configurator = new WebMergeConfiguration();
            Build();
        }

        public WebMergeClient(HttpMessageHandler messageHandler, IApiConfigurator configurator)
        {
            httpClient = new HttpClient(messageHandler);
            this.configurator = configurator;
            Build();
        }

        private void Build()
        {
            AddAuthentication();
            httpClient.BaseAddress = configurator.BaseUri;
        }

        private void AddAuthentication()
        {
            if (string.IsNullOrWhiteSpace(configurator.ApiKey))
            {
                throw new ArgumentException("Missing Api Key value. Make sure the 'WebMerge.ApiKey' app setting contains your WebMerge API Key");
            }

            if (string.IsNullOrWhiteSpace(configurator.ApiSecret))
            {
                throw new ArgumentException("Missing Api Secret value. Make sure there is an environment variable with the key 'WebMerge.ApiSecret' and your WebMerge API Secret value");
            }

            var authBytes = Encoding.UTF8.GetBytes($"{configurator.ApiKey}:{configurator.ApiSecret}");
            var authToken = Convert.ToBase64String(authBytes);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        }

        public async Task<byte[]> MergeDocumentAsync(int documentId, string documentKey, Dictionary<string, object> mergeDictionary, bool download = true, bool testMode = false)
        {
            var endpoint = $"merge/{documentId}/{documentKey}";
            
            var args = new List<string>();

            if (download)
            {
                args.Add("download=1");
            }

            if (testMode)
            {
                args.Add("test=1");
            }

            if (args.Any())
            {
                endpoint += $"?{string.Join("&", args)}";
            }

            var response = await httpClient.PostAsJsonAsync(endpoint, mergeDictionary);

            response.EnsureSuccessStatusCode();

            if (download)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            var result = await response.Content.ReadAsAsync<DocumentMergeResponse>();

            if (!result.Success)
            {
                throw new WebMergeException($"Document merge result did not indicate success. [DocumentId]: {documentId} / [Document Key]: {documentKey}");
            }

            return null;
        }
    }
}
