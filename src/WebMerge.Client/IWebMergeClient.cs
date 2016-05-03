using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebMerge.Client.RequestModels;
using WebMerge.Client.ResponseModels;

namespace WebMerge.Client
{
    public interface IWebMergeClient : IDisposable
    {
        #region Documents

        Task<byte[]> MergeDocumentAsync(int documentId, string documentKey, Dictionary<string, object> mergeDictionary, bool download = true, bool testMode = false);
        Task<Document> CreateDocumentAsync(DocumentRequest request);
        Task<Document> UpdateDocumentAsync(int documentId, DocumentUpdateRequest request);
        Task<List<Document>> GetDocumentListAsync(string search = null, string folder = null);
        Task<Document> GetDocumentAsync(int documentId);
        Task<List<DocumentField>> GetDocumentFieldsAsync(int documentId);
        Task<DocumentFile> GetFileForDocumentAsync(int documentId);
        Task<Document> CopyDocument(int documentId, string name);
        Task<RequestState> DeleteDocument(int documentId);

        #endregion

        #region Data Routes

        Task<Stream> MergeDataRouteWithSingleDownloadAsync(int documentId, string documentKey, object mergeObject, bool testMode = false);
        Task<RequestState> MergeDataRouteAsync(int documentId, string documentKey, object mergeObject, bool testMode = false);
        Task<MultipleFileRouteRequestState> MergeDataRouteWithMultipleDownloadAsync(int documentId, string documentKey, object mergeObject, bool testMode = false);

        #endregion
    }
}