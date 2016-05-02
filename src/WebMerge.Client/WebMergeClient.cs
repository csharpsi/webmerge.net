using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebMerge.Client.Enums;
using WebMerge.Client.RequestModels;
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

        public WebMergeClient(HttpClient httpClient, IApiConfigurator configurator)
        {
            this.httpClient = httpClient;
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

            // todo - not sure what the best thing to do here is when status code != 200
            response.EnsureSuccessStatusCode();

            if (download)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            var result = await response.Content.ReadAsAsync<RequestState>();

            if (!result.Success)
            {
                throw new WebMergeException($"Document merge result did not indicate success. [DocumentId]: {documentId} / [Document Key]: {documentKey}");
            }

            return null;
        }

        public async Task<Document> CreateDocumentAsync(DocumentRequest request)
        {
            CheckRequest(request);

            var response = await httpClient.PostAsJsonAsync("api/documents", request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<Document>();
        }

        public async Task<Document> UpdateDocumentAsync(int documentId, DocumentUpdateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.DocumentType.HasValue)
            {
                throw new WebMergeException("You cannot change the type of the document via the API");
            }

            var response = await httpClient.PutAsJsonAsync($"api/documents/{documentId}", request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<Document>();
        }

        public async Task<List<Document>> GetDocumentListAsync(string search = null, string folder = null)
        {
            var endpoint = "api/documents";
            var args = new List<string>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                args.Add($"search={search.Trim()}");
            }

            if (!string.IsNullOrWhiteSpace(folder))
            {
                args.Add($"folder={folder.Trim()}");
            }

            if (args.Any())
            {
                endpoint += $"?{string.Join("&", args)}";
            }

            var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<List<Document>>();
        }

        public async Task<Document> GetDocumentAsync(int documentId)
        {
            var response = await httpClient.GetAsync($"api/documents/{documentId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<Document>();
        }

        public async Task<List<DocumentField>> GetDocumentFieldsAsync(int documentId)
        {
            var response = await httpClient.GetAsync($"api/documents/{documentId}/fields");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<List<DocumentField>>();
        }

        public async Task<DocumentFile> GetFileForDocumentAsync(int documentId)
        {
            var response = await httpClient.GetAsync($"api/documents/{documentId}/file");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<DocumentFile>();
        }

        public async Task<Document> CopyDocument(int documentId, string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var content = new StringContent(JsonConvert.SerializeObject(new {name}), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"api/documents/{documentId}/copy", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<Document>();
        }

        public async Task<RequestState> DeleteDocument(int documentId)
        {
            var response = await httpClient.DeleteAsync($"api/documents/{documentId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<RequestState>();
        }

        private void CheckRequest(DocumentRequest request)
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
                throw new WebMergeException($"Could not create a '{request.DocumentType?.ToString("G")}' because there were no file contents.");
            }
        }

        public void Dispose() => httpClient?.Dispose();
    }
}
