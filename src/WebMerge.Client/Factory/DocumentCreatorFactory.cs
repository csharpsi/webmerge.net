using System.Net.Http;
using WebMerge.Client.Enums;
using WebMerge.Client.RequestModels;
using WebMerge.Client.Utils.FileSystem;

namespace WebMerge.Client.Factory
{
    public class DocumentCreatorFactory : IDocumentCreatorFactory
    {
        private readonly HttpClient httpClient;
        private readonly IFileHelper fileHelper;

        public DocumentCreatorFactory(HttpClient httpClient)
            : this(httpClient, new FileHelper())
        {
            
        }

        public DocumentCreatorFactory(HttpClient httpClient, IFileHelper fileHelper)
        {
            this.httpClient = httpClient;
            this.fileHelper = fileHelper;
        }

        public IDocumentCreator Build(string name, DocumentOutputType outputType, string outputName, string folder)
        {
            var request = new DocumentCreateRequest
            {
                Name = name,
                Folder = folder,
                OutputName = outputName,
                OutputType = outputType
            };

            return new DocumentCreator(request, httpClient, fileHelper);
        }
    }
}