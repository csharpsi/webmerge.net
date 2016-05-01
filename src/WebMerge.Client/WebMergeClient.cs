using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebMerge.Client.Enums;
using WebMerge.Client.Factory;
using WebMerge.Client.RequestModels;
using WebMerge.Client.ResponseModels;

namespace WebMerge.Client
{
    public class WebMergeClient : IWebMergeClient
    {
        private readonly HttpClient httpClient;
        private readonly IApiConfigurator configurator;
        private readonly IDocumentCreatorFactory documentCreatorFactory;

        public WebMergeClient()
        {
            httpClient = new HttpClient();
            configurator = new WebMergeConfiguration();
            documentCreatorFactory = new DocumentCreatorFactory(httpClient);
            Build();
        }

        public WebMergeClient(HttpClient httpClient, IApiConfigurator configurator, IDocumentCreatorFactory documentCreatorFactory)
        {
            this.httpClient = httpClient;
            this.configurator = configurator;
            this.documentCreatorFactory = documentCreatorFactory;
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

            // todo - not sure what the best thing to do here is when status code != 200
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

        public async Task<Document> CreateDocumentAsync(DocumentRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (!string.IsNullOrWhiteSpace(request.Html) && request.DocumentType != DocumentType.Html)
            {
                throw new WebMergeException("Html content can only be used for document type of HTML");
            }

            if (request.DocumentType != DocumentType.Html && string.IsNullOrWhiteSpace(request.FileContents))
            {
                throw new WebMergeException($"Could not create a '{request.DocumentType.ToString("G")}' because there were no file contents.");
            }

            var response = await httpClient.PostAsJsonAsync("api/documents", request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<Document>();
        }

        public Task<Document> UpdateDocumentAsync(int documentId, DocumentRequest request)
        {
            throw new NotImplementedException();
        }

        public IDocumentCreator UpdateDocument(int documentId, string name, DocumentOutputType output = DocumentOutputType.Pdf, string outputName = null, string folder = null)
        {
            throw new NotImplementedException();
        }
    }
}
