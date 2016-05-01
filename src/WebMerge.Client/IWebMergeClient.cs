using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebMerge.Client.Enums;
using WebMerge.Client.RequestModels;
using WebMerge.Client.ResponseModels;

namespace WebMerge.Client
{
    public interface IWebMergeClient
    {
        Task<byte[]> MergeDocumentAsync(int documentId, string documentKey, Dictionary<string, object> mergeDictionary, bool download = true, bool testMode = false);
        Task<Document> CreateDocumentAsync(DocumentRequest request);
        Task<Document> UpdateDocumentAsync(int documentId, DocumentRequest request);

        IDocumentCreator UpdateDocument(int documentId, string name, DocumentOutputType output = DocumentOutputType.Pdf, string outputName = null, string folder = null);
    }
}