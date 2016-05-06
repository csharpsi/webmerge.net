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

        Task<Stream> MergeDocumentAndDownloadAsync(int documentId, string documentKey, object mergeObject, bool testMode = false);
        Task<ActionResponse> MergeDocumentAsync(int documentId, string documentKey, object mergeObject, bool testMode = false);
        Task<Document> CreateDocumentAsync(DocumentRequest request);
        Task<Document> UpdateDocumentAsync(int documentId, DocumentUpdateRequest request);
        Task<List<Document>> GetDocumentListAsync(string search = null, string folder = null);
        Task<Document> GetDocumentAsync(int documentId);
        Task<List<Field>> GetDocumentFieldsAsync(int documentId);
        Task<DocumentFile> GetFileForDocumentAsync(int documentId);
        Task<Document> CopyDocumentAsync(int documentId, string name);
        Task<ActionResponse> DeleteDocumentAsync(int documentId);

        #endregion

        #region Data Routes

        Task<Stream> MergeDataRouteWithSingleDownloadAsync(int documentId, string documentKey, object mergeObject, bool testMode = false);
        Task<ActionResponse> MergeDataRouteAsync(int documentId, string documentKey, object mergeObject, bool testMode = false);
        Task<MultipleFileRouteRequestState> MergeDataRouteWithMultipleDownloadAsync(int documentId, string documentKey, object mergeObject, bool testMode = false);

        Task<List<DataRoute>> GetDataRouteListAsync();
        Task<DataRoute> GetDataRouteAsync(int dataRouteId);
        Task<List<Field>> GetDataRouteFieldsAsync(int dataRouteId);
        Task<ActionResponse> DeleteDataRouteAsync(int dataRouteId);

        #endregion
    }
}