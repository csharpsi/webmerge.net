using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WebMerge.Client.Enums;
using WebMerge.Client.RequestModels;
using WebMerge.Client.ResponseModels;
using WebMerge.Client.Utils.FileSystem;

namespace WebMerge.Client
{
    internal class DocumentCreator : IDocumentCreator
    {
        private readonly DocumentCreateRequest request;
        private readonly HttpClient httpClient;
        private readonly IFileHelper fileHelper;

        internal DocumentCreator(DocumentCreateRequest request, HttpClient httpClient)
            : this(request, httpClient, new FileHelper())
        {
            
        }

        internal DocumentCreator(DocumentCreateRequest request, HttpClient httpClient, IFileHelper fileHelper)
        {
            this.request = request;
            this.httpClient = httpClient;
            this.fileHelper = fileHelper;
        }

        public async Task<Document> FromHtml(string html)
        {
            request.DocumentType = DocumentType.Html;
            request.Html = html;
            request.FileContents = null;

            return await SendRequest();
        }

        public async Task<Document> FromFile(string fullyQualifiedPathToFile, DocumentType documentType)
        {
            if (!fileHelper.Exists(fullyQualifiedPathToFile))
            {
                throw new WebMergeException($"File at {fullyQualifiedPathToFile} cannot be found");
            }

            var fileBytes = fileHelper.ReadAllBytes(fullyQualifiedPathToFile);

            return await FromFile(fileBytes, documentType);
        }

        public async Task<Document> FromFile(byte[] fileBytes, DocumentType documentType)
        {
            request.DocumentType = documentType;
            request.Html = null;
            request.FileContents = Convert.ToBase64String(fileBytes);

            return await SendRequest();
        }
        
        private async Task<Document> SendRequest()
        {
            var response = await httpClient.PostAsJsonAsync("api/documents", request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<Document>();
        }
    }
}